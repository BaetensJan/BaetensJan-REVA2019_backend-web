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
        private readonly ICategoryRepository _categoryRepository;
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
            _categoryRepository = categoryRepository;
            _imageWriter = imageWriter;
        }

        [HttpGet("[Action]")]
        public IActionResult GetAmountOfAssignments()
        {
            return Ok(_configuration["AmountOfQuestions"]);
        }

        [HttpGet("[Action]")]
        public IActionResult GetStartDateOfApplication()
        {
            return Ok(new
            {
                StartDate = _configuration["StartDate"]
            });
        }

        /**
        * Gets the closest exhibitor with a certain category, starting from previous exhibitor. (Normal Tour)
        * 
        * parameter1: string previousExhibitorId - the id of the previous exhibitor, from which
        * the assignment has already been submitted.
        * parameter2: string category - the current, by the students chosen, category
        * parameter3: bool isExtraRound - if the group is doing an Assignment in an Extra Round.
         * 
        * RETURN an Assignment, containing an Exhibitor object that represent the current, newly assigned Exhibitor.
        */
        [HttpGet("[Action]/{categoryId}/{previousExhibitorId}/{isExtraRound}")]
        public async Task<IActionResult> CreateAssignment(int categoryId, int previousExhibitorId, bool isExtraRound)
        {
            // FindNextExhibitor already checks if the categoryId and ExhibitorId combo has a question in the DB.
            var exhibitor = await _exhibitorManager.FindNextExhibitor(previousExhibitorId, categoryId);

            exhibitor.GroupsAtExhibitor++;

            var question = await _questionRepository.GetQuestion(categoryId, exhibitor.Id);

            var assignment = await CreateAssignment(question, isExtraRound);
            assignment =
                await _assignmentRepository
                    .GetByIdLight(assignment.Id); //Todo temporary, otherwise we have recursive catExh data

            return Ok(assignment);
        }

        /**
        * A group has created a new Exhibitor in the Extra Tour, as they didn't find the Exhibitor in the list.
        */
        [HttpGet("[Action]")]
        public async Task<IActionResult> CreateAssignmentNewExhibitor([FromBody] CreatedExhibitorDTO createdExhibitor)
        {
            //We take a specific question for this assignment, as an assignment needs a question
            //and we dont want to create new (random) question in the database each time a group start an extra tour.
            var question = await _questionRepository.GetById(127);

            // get group object via schoolId and groupName
            var group = await _groupRepository.GetById(Convert.ToInt32(User.Claims.ElementAt(5).Value));

            // Create assignment and Add to the groups assignments.
            var assignment = new Assignment(question, true);

            // We add the exhibitor information from the group to the Assignment.
            assignment.Answer += $"Exposant naam: {createdExhibitor.Name}";
            if (createdExhibitor.categoryId != -1)
            {
                var category = await _categoryRepository.GetById(createdExhibitor.categoryId);
                assignment.Answer += $", met Categorie: {category.Name}";
            }

            if (!string.IsNullOrEmpty(createdExhibitor.BoothNumber))
            {
                assignment.Answer += $" en Standnummer: {createdExhibitor.BoothNumber}";
            }

            group.AddAssignment(assignment);
            await _assignmentRepository.SaveChanges();

            assignment =
                await _assignmentRepository
                    .GetByIdLight(assignment.Id); //Todo temporary, otherwise we have recursive catExh data

            assignment.Answer = ""; // empty answer for android (we will re-add the exhibitor information @ submit)

            return Ok(assignment);
        }

        /**
        * Gets an assignment when a group runs an Extra Round.
        * The Group has chosen a specific Exhibitor for a specific Category.
        * 
        * Creates an assignment related to an exhibitor with exhibitorId equal to parameter exhibitorId and
        * category with a categoryId equal to the parameter exhibitorId
        * parameter1: string exhibitorId - the id of the exhibitor of which a question should be the subject.
        * parameter2: string category - the, by the students chosen, category
        * 
        * RETURN an Assignment, containing an Exhibitor object that represent the current, newly assigned Exhibitor.
        */
        [HttpGet("[Action]/{categoryId}/{exhibitorId}")]
        public async Task<IActionResult> CreateAssignmentOfExhibitorAndCategory(int categoryId, int exhibitorId)
        {
            var question = await _questionRepository.GetQuestion(categoryId, exhibitorId);

            var assignment = await CreateAssignment(question, true);

            assignment =
                await _assignmentRepository
                    .GetByIdLight(assignment.Id); //Todo temporary, otherwise we have recursive catExh data

            return Ok(assignment);
        }

        private async Task<Assignment> CreateAssignment(Question question, bool isExtraRound)
        {
            // get group object via schoolId and groupName
            var group = await _groupRepository.GetById(Convert.ToInt32(User.Claims.ElementAt(5).Value));

            // Create assignment and Add to the groups assignments.
            var assignment = new Assignment(question, isExtraRound);
            group.AddAssignment(assignment);

            var exhibitor = assignment.Question.CategoryExhibitor.Exhibitor;
            exhibitor.GroupsAtExhibitor++;
            exhibitor.TotalNumberOfVisits++;

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
                var answer = "";

                // Group created Exhibitor in Extra Round
                if (assignment.WithCreatedExhibitor(_configuration.GetValue<int>("CreatedExhibitorQuestionId")))
                {
                    answer = $"{assignment.Answer}. Antwoord groep: {model.Answer}";
                }
                else // assignment of normal tour
                {
                    answer = model.Answer;

                    var exhibitor =
                        await _exhibitorRepository.GetById(assignment.Question.CategoryExhibitor.ExhibitorId);
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