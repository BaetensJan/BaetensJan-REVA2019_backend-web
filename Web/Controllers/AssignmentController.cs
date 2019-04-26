using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Services;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
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
        private readonly IAssignmentRepository _assignmentRepository;
        private readonly IAssignmentBackupRepository _assignmentBackupRepository;
        private readonly IExhibitorRepository _exhibitorRepository;
        private readonly IQuestionRepository _questionRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ExhibitorManager _exhibitorManager;
        private readonly IImageWriter _imageWriter;
        private readonly IConfiguration _configuration;
        private readonly AssignmentManager _assignmentManager;
        private readonly GroupManager _groupManager;
        private readonly IAuthenticationManager _authenticationManager;

        public AssignmentController(IConfiguration configuration,
            IAssignmentRepository assignmentRepository,
            IAssignmentBackupRepository assignmentBackupRepository,
            IExhibitorRepository exhibitorRepository,
            IAuthenticationManager authenticationManager,
            IGroupRepository groupRepository,
            ICategoryRepository categoryRepository,
            IImageWriter imageWriter,
            IQuestionRepository questionRepository)
        {
            _configuration = configuration;
            // todo mag via ServiceManager geinjecteerd worden.
            _groupManager = new GroupManager(configuration, groupRepository);
            _assignmentRepository = assignmentRepository;
            _assignmentBackupRepository = assignmentBackupRepository;
            _exhibitorRepository = exhibitorRepository;
            _exhibitorManager = new ExhibitorManager(exhibitorRepository);
            _questionRepository = questionRepository;
            _categoryRepository = categoryRepository;
            _authenticationManager = authenticationManager;
            _imageWriter = imageWriter;
            _assignmentManager = new AssignmentManager(assignmentRepository);
        }

        [HttpGet("[Action]")]
        [Authorize]
        public IActionResult GetAmountOfAssignments()
        {
            return Ok(_configuration["AmountOfQuestions"]);
        }

        [HttpGet("[Action]")]
        [Authorize]
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
        [Authorize]
        //todo role group
        public async Task<IActionResult> CreateAssignment(int categoryId, int previousExhibitorId, bool isExtraRound)
        {
            var group = await _groupManager.GetGroup(User.Claims);
            if (group == null)
            {
                return NotFound("groupId not found in token.");
            }

            var assignments = group.Assignments;

            List<Question> questions;
            //todo make new relation in db where Category knows its Questions.
            //Only get Exhibitors of which there exists a Question with given categoryId. 
            var extraRound = assignments?.Count >= _configuration.GetValue<int>("AmountOfQuestions") && isExtraRound;
            if (previousExhibitorId != -1) // Check if Group has already visited an Exhibitor.
            {
                // we check on extraRound, because then groups can re-chose a category,
                // and we have to avoid that a group gets the same question again.
                if (extraRound)
                {
                    questions = await _questionRepository.GetQuestions(categoryId, previousExhibitorId,
                        assignments);
                }
                else
                {
                    questions = await _questionRepository.GetQuestions(categoryId, previousExhibitorId);
                }
            }
            else
            {
                // we check on extraRound, because then groups can re-chose a category,
                // and we have to avoid that a group gets the same question again.
                if (extraRound)
                {
                    questions = await _questionRepository.GetQuestions(categoryId, assignments);
                }
                else
                {
                    questions = await _questionRepository.GetQuestions(categoryId);
                }
            }

            if (questions.Count < 1)
            {
                return StatusCode(500, "Alle vragen voor deze Categorie zijn al beantwoord. " +
                                       "Deze Categorie mocht niet worden weergegeven in de CategoryChoiceFragment");
            }

            var potentialExhibitors = questions.Select(e => e.CategoryExhibitor.Exhibitor).ToList();

            var exhibitor = await _exhibitorManager.FindNextExhibitor(previousExhibitorId, potentialExhibitors);

            var question = questions.Find(q => q.CategoryExhibitor.ExhibitorId == exhibitor.Id);

            var newAssignment = await _assignmentManager.CreateAssignment(question, isExtraRound, group);

            //Todo temporary, otherwise we have recursive catExh data.
            newAssignment = await _assignmentRepository.GetByIdLight(newAssignment.Id);

            return Ok(newAssignment);
        }

        /**
        * A group has created a new Exhibitor in the Extra Tour, as they didn't find the Exhibitor in the list.
        */
        [HttpPost("[Action]")]
        [Authorize]
        //todo role group
        public async Task<IActionResult> CreateAssignmentNewExhibitor([FromBody] CreatedExhibitorDto createdExhibitor)
        {
            //We take a specific question for this assignment, as an assignment needs a question
            //and we dont want to create new (random) question in the database each time a group start an extra tour.
//            var question = await _questionRepository.GetById(127);

            // get group object via schoolId and groupName
            var group = await _groupManager.GetGroup(User.Claims);
            if (group == null)
            {
                return NotFound("groupId not found in token.");
            }

            // Create assignment and Add to the groups assignments.
            var assignment = new Assignment(true);

            // We add the exhibitor information from the group to the Assignment.
            assignment.Answer += $"Exposant naam: {createdExhibitor.Name}";

            if (createdExhibitor.CategoryId != -1)
            {
                var category = await _categoryRepository.GetById(createdExhibitor.CategoryId);
                assignment.Answer += $", met Categorie: {category.Name}";
            }

            if (!string.IsNullOrEmpty(createdExhibitor.BoothNumber))
            {
                assignment.Answer += $" en Standnummer: {createdExhibitor.BoothNumber}";
            }

//            group.AddAssignment(assignment); //todo: need fix for this, we dont want a group
//                todo:  with question null (nor a fake question => getCategories, getExhibitors, ...), but group
//                todo:  needs to remember his assignments for when he logs out (and logs back in => getGroupInfo())

            var backupAssignment = await CreateBackupAssignment(assignment, true, ""); // todo, when getting
            assignment.Id = backupAssignment.Id;
            //todo assignments for school via web -> also get the assignment from backup (where we temporarely
            //todo save the assignments for - by group created exhibitor- assignments.

            /**
             * Empty answer for android (we will re-add the exhibitor information @ submit)
             */
            assignment.Answer = "";
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
        [Authorize]
        //todo role group
        public async Task<IActionResult> CreateAssignmentOfExhibitorAndCategory(int categoryId, int exhibitorId)
        {
            var group = await _groupManager.GetGroup(User.Claims);
            if (group == null)
            {
                return NotFound("groupId not found in token.");
            }

            var question = await _questionRepository.GetQuestion(categoryId, exhibitorId,
                group.Assignments.Select(a => a.Question));

            var assignment = await _assignmentManager.CreateAssignment(question, true, group);

            //Todo temporary, otherwise we have recursive catExh data.
            assignment = await _assignmentRepository.GetByIdLight(assignment.Id);

            return Ok(assignment);
        }

        /**
        * When a group submits an Assignment in the application, this controller method will be called.
        */
        [HttpPost("SubmitAssignment")]
        [Authorize]
        //todo role group
        public async Task<IActionResult> Submit([FromBody] AssignmentDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Error, the submitted assignment is not valid.");
            }

            if (model.CreatedExhibitor)
            {
                var backupAssignment = await _assignmentBackupRepository.GetById(model.Id);
                backupAssignment.Answer = $"{backupAssignment.Answer}. Antwoord groep: {model.Answer}";
                //todo assignment and backupassignment need to implement interface in order
                // to do use both of them in same kind of way. (super class)
//                UpdateAssignment(new Assignment(), model.Notes, model.Photo);

                backupAssignment.Notes = model.Notes;
                if (!string.IsNullOrEmpty(model.Photo))
                {
                    backupAssignment.Photo = model.Photo;
                }

                backupAssignment.Submitted = true;
                backupAssignment.SubmissionDate = DateTime.Now;
                await _assignmentBackupRepository.SaveChanges();
            }
            else // assignment of normal tour
            {
                var assignment = await _assignmentRepository.GetById(model.Id);
                assignment.Answer = model.Answer;

                var exhibitor = await _exhibitorRepository.GetById(assignment.Question.CategoryExhibitor.ExhibitorId);
                exhibitor.GroupsAtExhibitor--;

                UpdateAssignment(assignment, model.Notes, model.Photo);
                await _assignmentRepository.SaveChanges();

                await CreateBackupAssignment(assignment, model.CreatedExhibitor, model.Photo);
            }

            return Ok( /*assignment*/);
        }

        private void UpdateAssignment(Assignment assignment, string notes, string photo)
        {
            assignment.Notes = notes;
            if (!string.IsNullOrEmpty(photo))
            {
                assignment.Photo = _imageWriter.WriteBase64ToFile(photo);
            }

            assignment.Submitted = true;
            assignment.SubmissionDate = DateTime.Now;
        }

        /**
         * Make backup of assignment.
         */
        private async Task<AssignmentBackup> CreateBackupAssignment(Assignment assignment, bool isCreatedExhibitor,
            string photo)
        {
            var group = await _groupManager.GetGroup(User.Claims);
            var groupAppUser = await _authenticationManager.GetAppUserWithSchoolIncludedViaId(group.ApplicationUserId);

            var schoolName = groupAppUser.School.Name;
            return await _assignmentBackupRepository.Add(assignment, schoolName, group.Name, isCreatedExhibitor,
                "" /*photo*/);
        }
    }
}