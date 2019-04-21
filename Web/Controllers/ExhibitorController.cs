using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Services;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.DTOs;

namespace Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExhibitorController : Controller
    {
        private readonly ExhibitorManager _exhibitorManager;
        private readonly IExhibitorRepository _exhibitorRepository;
        private readonly GroupManager _groupManager;
        private readonly IQuestionRepository _questionRepository;

        public ExhibitorController(IExhibitorRepository exhibitorRepository,
            IGroupRepository groupRepository,
            IQuestionRepository questionRepository)
        {
            _exhibitorRepository = exhibitorRepository;
            _questionRepository = questionRepository;
            _exhibitorManager = new ExhibitorManager(exhibitorRepository);
            _groupManager = new GroupManager(groupRepository); // todo mag via ServiceManager geinjecteerd worden.
        }

        [HttpGet("[action]")]
        [Authorize]
        public async Task<IActionResult> Exhibitors()
        {
//            var exhbs = _exhibitorManager.ExhibitorsLight();
            return Ok(await _exhibitorRepository.All());
        }

        /**
         * Returns a list of Exhibitors of which not all questions have been
         * answered by a Group.
         */
        [HttpGet("[action]")]
        [Authorize]
        public async Task<IActionResult> ExhibitorsWithUnansweredQuestions()
        {
            var group = await _groupManager.GetGroup(User.Claims);
            if (group == null)
            {
                return NotFound("groupId not found in token.");
            }

            var assignments = group.Assignments;
            var questions = await _questionRepository.GetAllLight();

            //todo: exhibitor should have a relation with Question, in order to
            //todo e.g. loop over every assignment and take the assignment.question.categoryExhibitor.exhibitor
            //todo: and temporary remove that question from that exhibitor.questions and immediately check
            //todo: if exhibitor.questions.size < 1 (then you know the group has done all the questions of
            //that particular exhibitor (and thus you should not return it).

            foreach (var assignment in assignments)
            {
                    //todo fix
                    var question = questions.Single(q => q.Id == assignment.Question.Id);
                    questions.Remove(question);
            }

            //todo: this might be faster than loop above.
//            var x = assignments.Select(a => a.Question.Id);
//            questions.RemoveAll(q => x.Contains(q.Id));

            /**
             * Selecting the entire Exhibitor object rather than the id would not remove the duplicates,
             * I suspect this has something to do with the fact that these Exhibitor objects were already
             * incomplete by the ExhibitorMap in the Select() in the QuestionRepository.
             */
            var exhibs = questions.Select(q => q.CategoryExhibitor.Exhibitor);
            var exhibIdsHashSet = new HashSet<int>(exhibs.Select(exh => exh.Id));

            var exhibitors = new List<Exhibitor>();
            exhibIdsHashSet.ToList().ForEach(exhId => exhibitors.Add(exhibs.First(exh => exh.Id == exhId)));

            return Ok(exhibitors);
        }

        /**
         * returns exhibitor with name equal to parameter exhibitorname.
         */
        [HttpGet("[action]/{exhibitorName}")]
        [Authorize]
        public async Task<IActionResult> ExhibitorByName(string exhibitorName)
        {
            return Ok(await _exhibitorRepository.GetByName(exhibitorName));
        }

        [HttpGet("[action]")]
        [Authorize]
        public async Task<IActionResult> ExhibitorsLight()
        {
            return Ok(await _exhibitorManager.ExhibitorsLight());
        }

        [HttpPut("[action]/{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateExhibitor([FromRoute] int id, [FromBody] ExhibitorDTO exhibitordto)
        {
            var exhibitor = new Exhibitor
            {
                Name = exhibitordto.Name,
                ExhibitorNumber = exhibitordto.ExhibitorNumber,
                X = exhibitordto.X,
                Y = exhibitordto.Y,
                GroupsAtExhibitor = 0,
                Categories = CreateCategories(exhibitordto.CategoryIds)
            };
            return Ok(await _exhibitorManager.UpdateExhibitor(id, exhibitor));
        }

        [HttpDelete("[action]/{id}")]
        [Authorize]
        public async Task<IActionResult> RemoveExhibitor([FromRoute] int id)
        {
            return Ok(await _exhibitorManager.RemoveExhibitor(id));
        }

        [HttpDelete("RemoveExhibitors")]
        [Authorize]
        public async Task<ActionResult> RemoveExhibitors()
        {
            IEnumerable<Exhibitor> exhibitors = await _exhibitorRepository.All();
            if (exhibitors != null)
            {
                _exhibitorRepository.RemoveAllExhibitors(exhibitors);
                await _exhibitorRepository.SaveChanges();
            }

            return Ok();
        }

        [HttpPost("[action]")]
        [Authorize]
        public async Task<IActionResult> AddExhibitor([FromBody] ExhibitorDTO exhibitordto)
        {
            var exhibitor = new Exhibitor
            {
                Name = exhibitordto.Name,
                ExhibitorNumber = exhibitordto.ExhibitorNumber,
                X = exhibitordto.X,
                Y = exhibitordto.Y,
                GroupsAtExhibitor = 0,
                Categories = CreateCategories(exhibitordto.CategoryIds)
            };

            //_exhibitorManager.AddExhibitor(exhibitor);
            await _exhibitorRepository.Add(exhibitor);
            await _exhibitorRepository.SaveChanges();

            return Ok(exhibitor);
        }

        [HttpPost("[action]")]
        [Authorize]
        public async Task<IActionResult> UpdateExhibitor([FromBody] ExhibitorDTO exhibitordto)
        {
            var e = await _exhibitorRepository.GetById(exhibitordto.Id);
            e.Id = exhibitordto.Id;
            e.Name = exhibitordto.Name;
            e.ExhibitorNumber = exhibitordto.ExhibitorNumber;
            e.X = exhibitordto.X;
            e.Y = exhibitordto.Y;
            e.GroupsAtExhibitor = 0;
//            var lcatexb = CreateCategories(exhibitordto.CategoryIds);
            var exh = await _exhibitorManager.UpdateExhibitor(e);
            return Ok(exh);
        }

        private static IEnumerable<CategoryExhibitor> CreateCategories(IReadOnlyCollection<int> categoryIdList)
        {
            var categoryExhibitorList = new List<CategoryExhibitor>();

            if (categoryIdList != null)
            {
                categoryExhibitorList.AddRange(categoryIdList.Select(id => new CategoryExhibitor() {CategoryId = id}));
            }

            return categoryExhibitorList;
        }
    }
}