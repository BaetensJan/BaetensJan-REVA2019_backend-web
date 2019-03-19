using System.Collections.Generic;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Authorization;
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

        public QuestionController(IQuestionRepository repository,
            ICategoryExhibitorRepository categoryExhibitorRepository)
        {
            _questionRepository = repository;
            _categoryExhibitorRepository = categoryExhibitorRepository;
        }

        [HttpGet("[action]")]
        [Authorize]
        public async Task<IActionResult> Questions()
        {
            return Ok(await _questionRepository.GetAllLight());
        }

        [HttpPost("CreateQuestion")]
        [Authorize]
        public async Task<IActionResult> CreateQuestion([FromBody] QuestionDTO model)
        {
            if (ModelState.IsValid)
            {
                return BadRequest("Zorg dat naam ingevuld is.");
            }

            var ce = await GetOrCreateCategoryExhibitor(model.CategoryId, model.ExhibitorId);
            var question = new Question()
            {
                QuestionText = model.QuestionText,
                Answer = model.AnswerText,
                CategoryExhibitor = ce
            };
            await _questionRepository.Add(question);
            await _questionRepository.SaveChanges();
            return Ok(question);
        }

        /*public void TruncateTable()
        {
            string sqlTrunc = "TRUNCATE TABLE Question";
            SqlCommand cmd = new SqlCommand(sqlTrunc);
            cmd.ExecuteNonQuery();
        }*/

        [HttpPut("[action]/{id}")]
        [Authorize]
        public async Task<IActionResult> EditQuestion([FromRoute] int id, [FromBody] QuestionUpdateDTO model)
        {
            var ce = await GetOrCreateCategoryExhibitor(model.CategoryId, model.ExhibitorId);
            var q = await _questionRepository.EditQuestion(id, model.QuestionText, model.AnswerText, ce);

            if (q == null)
            {
                NotFound("Zorg dat questionId correct is.");
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
        [Authorize]
        public async Task<IActionResult> DeleteQuestion(int QuestionId)
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

        [HttpDelete("DeleteQuestions")]
        [Authorize]
        public async Task<ActionResult> DeleteQuestions()
        {
            var questions = await _questionRepository.GetAll();
            if (questions != null)
            {
                _questionRepository.RemoveAllQuestions(questions);
                await _questionRepository.SaveChanges();
            }

            return Ok();
        }
    }
}