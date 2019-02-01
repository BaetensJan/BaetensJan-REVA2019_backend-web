using System;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
        private readonly IAssignmentRepository _assignmentRepository;
        private readonly IExhibitorRepository _exhibitorRepository;
        private readonly IGroupRepository _groupRepository;
        private readonly IQuestionRepository _questionRepository;
        private readonly ExhibitorManager _exhibitorManager;
        private readonly IImageWriter _imageWriter;
        private readonly IConfiguration _configuration;

        public AssignmentController(UserManager<ApplicationUser> userManager,
            IConfiguration configuration,
            IAssignmentRepository assignmentRepository,
            IExhibitorRepository exhibitorRepository,
            IGroupRepository groupRepository,
            ICategoryRepository categoryRepository,
            IImageWriter imageWriter,
            IQuestionRepository questionRepository)
        {
            _configuration = configuration;
            _groupRepository = groupRepository;
            _userManager = userManager;
            _assignmentRepository = assignmentRepository;
            _exhibitorRepository = exhibitorRepository;
            _exhibitorManager =
                new ExhibitorManager(exhibitorRepository, categoryRepository, questionRepository);
            _questionRepository = questionRepository;
            _imageWriter = imageWriter;
        }
        [HttpGet("[Action]")]
        public IActionResult GetAmountOfAssignments()
        {
            return Ok(_configuration["AmountOfQuestions"]);
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

            // get group object via schoolId and groupName
            var group = await _groupRepository.GetById(Convert.ToInt32(User.Claims.ElementAt(5).Value));

            // Create assignment and Add to the groups assignments.
            var assignment = new Assignment(question);
            group.AddAssignment(assignment);
            await _assignmentRepository.SaveChanges();

            return assignment;
        }

        /**
         * When a group submits an Assignment in the application, this controller method will be called.
         */
        [HttpPost("SubmitAssignment")]
        public async Task<IActionResult> Submit([FromBody] AssignmentDTO model)
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
                if (!string.IsNullOrEmpty(model.Photo)) assignment.Photo = _imageWriter.WriteBase64ToFile(model.Photo);
                assignment.Submitted = true;
                assignment.SubmissionDate = DateTime.Now;

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