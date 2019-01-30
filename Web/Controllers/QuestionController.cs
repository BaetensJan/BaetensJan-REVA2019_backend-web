using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Web.DTOs;

namespace Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionController : Controller
    {
        private readonly IQuestionRepository _questionRepository;

        private readonly ICategoryExhibitorRepository _categoryExhibitorRepository;
//        private readonly UserManager<ApplicationUser> _userManager;

        public QuestionController(IQuestionRepository repository,
            ICategoryExhibitorRepository categoryExhibitorRepository
            /*, UserManager<ApplicationUser> userManager*/)
        {
//            _userManager = userManager;
            _questionRepository = repository;
            _categoryExhibitorRepository = categoryExhibitorRepository;
        }

        [HttpGet("[action]")]
        public async Task<IEnumerable<Question>> Questions()
        {
            return await _questionRepository.GetAllLight();
        }

        [HttpPost("CreateQuestion")]
        public async Task<ActionResult> CreateQuestion([FromBody] QuestionDTO model)
        {
            if (ModelState.IsValid)
            {
                var ce = await GetOrCreateCategoryExhibitor(model.CategoryId, model.ExhibitorId);
                var question = new Question()
                {
                    QuestionText = model.QuestionText,
                    Answer = model.AnswerText,
                    CategoryExhibitor = ce
                };

                question = _questionRepository.Add(question);
                //_categoryExhibitorRepository.Add(question.CategoryExhibitor);
                await _questionRepository.SaveChanges();
                return Ok(question);
            }

            return Ok(
                new
                {
                    Message = "Zorg dat naam ingevuld is."
                });
        }

        [HttpPost("[Action]")]
        public async Task<ActionResult> EditQuestion([FromBody] QuestionUpdateDTO model)
        {
            var ce = await GetOrCreateCategoryExhibitor(model.CategoryId, model.ExhibitorId);
            var q =  await _questionRepository.EditQuestion(model.QuestionId, model.QuestionText, model.AnswerText, ce);

            if (q == null)
            {
                return Ok(
                    new
                    {
                        Message = "Zorg dat questionId correct is."
                    });
            }
            await _questionRepository.SaveChanges();
            return Ok(q);
        }

        public async Task<CategoryExhibitor> GetOrCreateCategoryExhibitor(int categoryId, int exhibitorId)
        {
            var ce = await _categoryExhibitorRepository.GetByCategoryAndExhibitorId(categoryId, exhibitorId);
            if (ce == null)
                ce = new CategoryExhibitor
                {
                    CategoryId = categoryId,
                    ExhibitorId = exhibitorId
                };
            return ce;
        }

        [HttpDelete("DeleteQuestion/{QuestionId}")]
        public async Task<ActionResult> DeleteQuestion(int QuestionId)
        {
            var question = await _questionRepository.GetById(QuestionId);
            if (question != null)
            {
                _questionRepository.Remove(question);
                await _questionRepository.SaveChanges();
            }

            return Ok(
                new
                {
                    Id = question.Id,
                });
        }
    }
}