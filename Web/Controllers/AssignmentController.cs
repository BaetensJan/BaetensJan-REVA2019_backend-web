using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Web.DTOs;

namespace Web.Controllers
{
/**
* Controller that handles all API calls concerning the assignment that the students have.
*/
    [Route("api/[controller]")]
    [ApiController]
    public class AssignmentController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IAssignmentRepository _assignmentRepository;
        private readonly IExhibitorRepository _exhibitorRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IGroupRepository _groupRepository;
        private readonly IQuestionRepository _questionRepository;
        private readonly ExhibitorManager _exhibitorManager;
        private readonly IImageWriter _imageWriter;

        public AssignmentController(UserManager<ApplicationUser> userManager,
            IConfiguration configuration,
            IAssignmentRepository assignmentRepository,
            IExhibitorRepository exhibitorRepository,
            IGroupRepository groupRepository,
            ICategoryRepository categoryRepository,
            IImageWriter imageWriter,
            IQuestionRepository questionRepository)
        {
            _groupRepository = groupRepository;
            _userManager = userManager;
            _configuration = configuration;
            _assignmentRepository = assignmentRepository;
            _exhibitorRepository = exhibitorRepository;
            _categoryRepository = categoryRepository;
            _exhibitorManager =
                new ExhibitorManager(exhibitorRepository, categoryRepository, questionRepository);
            _questionRepository = questionRepository;
            _imageWriter = imageWriter;
        }

        /**
        * Gets the closest exhibitor with a certain category, starting from previous exhibitor.
        * parameter1: string previousExhibitorId - the id of the previous exhibitor, from which
        * the assigment has already been submitted.
        * parameter2: string category - the current, by the students chosen, category
        * 
        * RETURN an Assignment, containing an Exhibitor object that represent the current, newly assigned Exhibitor.
        */
        [HttpGet("[Action]/{categoryId}/{previousExhibitorId}")]
        public async Task<IActionResult> GetAssignment(int categoryId, int previousExhibitorId)
        {
            Question question;
            if (categoryId == -1) // Group created a new exhibitor in the Extra Tour phase 
            {
                //We take a specific question for this assignment, as an assignment needs a question
                //and we dont want to create new (random) question in the database each time a group start an extra tour.
                question = await _questionRepository.GetById(795);
            }
            else
            {
                var exhibitor = await _exhibitorManager.FindNextExhibitor(previousExhibitorId, categoryId);
                question = await GetQuestion(exhibitor, categoryId);
            }

            var assignment = await CreateAssignment(question);
            assignment =
                await _assignmentRepository
                    .GetByIdLight(assignment.Id); //Todo temporary, otherwise we have recursive catExh data

            return Ok(assignment);
        }

        /**
        * Creates an assignment related to an exhibitor with exhibitorId equal to parameter exhibitorId and
         * category with a categoryId equal to the parameter exhibitorId
        * parameter1: string exhibitorId - the id of the exhibitor of which a question should be the subject.
        * parameter2: string category - the, by the students chosen, category
        * 
        * RETURN an Assignment, containing an Exhibitor object that represent the current, newly assigned Exhibitor.
        */
        [HttpGet("[Action]/{categoryId}/{exhibitorId}")]
        public async Task<IActionResult> GetAssignmentOfExhibitorAndCategory(int categoryId, int exhibitorId)
        {
            var exhibitor = await _exhibitorRepository.GetById(exhibitorId);
            var question = await GetQuestion(exhibitor, categoryId);

            var assignment = await CreateAssignment(question);
            assignment =
                await _assignmentRepository
                    .GetByIdLight(assignment.Id); //Todo temporary, otherwise we have recursive catExh data

            return Ok(assignment);
        }

        private async Task<Question> GetQuestion(Exhibitor exhibitor, int categoryId)
        {
            var questions = await _questionRepository.GetAll();
            //Todo: methode in repo maken die via beide id's (als parameter meegegeven) de question ophaalt.
            var question = questions.First(q => q.CategoryExhibitor.CategoryId == categoryId
                                                && q.CategoryExhibitor.ExhibitorId ==
                                                exhibitor.Id);
            return question;
        }

        private async Task<Assignment> CreateAssignment(Question question)
        {
            question.CategoryExhibitor.Exhibitor.GroupsAtExhibitor++;

            // get username from jwt token.
            var username = User.Claims.ElementAt(3).Value;

            // get ApplicationUser via username
            var user = _userManager.Users.Include(u => u.School).SingleOrDefault(u => u.UserName == username);

            // get groupName out of the username (username is a concat of schooName + groupName)
            var groupName = username.Substring(user.School.Name.Length);

            //TODO user should have a groupId, and get group via user and not as done below:
            // get group object via schoolId and groupName
            var group = await _groupRepository.GetBySchoolIdAndGroupName(user.School.Id, groupName);

            // Create assignment and Add to the groups assignments.
            var assignment = new Assignment(question);
            group.AddAssignment(assignment);
            assignment = /*await*/_assignmentRepository.Add(assignment); //TODO nog niet async.
            await _assignmentRepository.SaveChanges();

            return assignment;
        }

        /**
         * When a group submits an Assignment in the application, this controller method will be called.
         */
        [HttpPost("SubmitAssignment")]
        public async Task<IActionResult> Submit([FromBody] AssignmentViewModel model)
        {
            if (ModelState.IsValid)
            {
                var assignment = await _assignmentRepository.GetById(model.Id);
                var answer = model.Answer;

                // assignment of Extra Round
                if (assignment.Extra)
                {
                    answer = $"{model.Answer}. Standnummer van exposant: {model.Question.Exhibitor.ExhibitorNumber}," +
                             $"Exposantnaam: {model.Question.Exhibitor.Name}";
                }
                else // assignment of normal tour
                {
                    var question = await _questionRepository.GetById(assignment.Question.Id);
                    var exhibitor = await _exhibitorRepository.GetById(question.CategoryExhibitor.ExhibitorId);
                    exhibitor.GroupsAtExhibitor--;
                }

                assignment.Answer = answer;
                assignment.Notes = model.Notes;
                if (!String.IsNullOrEmpty(model.Photo)) assignment.Photo = _imageWriter.WriteBase64ToFile(model.Photo);
                assignment.Submitted = true;
                await _assignmentRepository.SaveChanges();

//                if (result.Succeeded)
                //TODO: temporary, recursive loop anders:
                assignment = await _assignmentRepository.GetByIdLight(assignment.Id);
                return Ok(assignment);
            }

            return Ok(
                new
                {
                    Message = "Error, the submitted assignment is not valid."
                });
        }

/**
 * If a group decides to cancel the Assignment in the application.
 */
        [HttpDelete("RemoveAssignment/{assignmentId}")]
        public async Task<IActionResult> Remove(int assignmentId)
        {
            var assignment = await _assignmentRepository.GetById(assignmentId);
            _assignmentRepository.Remove(assignment);
            await _assignmentRepository.SaveChanges();
            return Ok(new
            {
                //Message = "Assignment with assignmentId " + assignmentId + " was successfully deleted."
                AssignmentId = assignment.Id
            });
        }

//        /**
//         * Gets all the Assignment objects from the database.
//         */
//        [HttpGet("[action]")]
//        public IEnumerable<Assignment> Assignments()
//        {
//            return _assignmentRepository.All();
//        }
    }
}