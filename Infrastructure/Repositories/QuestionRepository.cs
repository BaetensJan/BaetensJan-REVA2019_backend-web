using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class QuestionRepository : IQuestionRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly DbSet<Question> _questions;

        public QuestionRepository(ApplicationDbContext _context)
        {
            _dbContext = _context;
            _questions = _context.Questions;
        }

        /**
         * Gets the question.
         */
        public Task<List<Question>> GetBasic() //TODO schoolID meegeven of list van questionIds.
        {
            return _questions.ToListAsync();
        }

        /**
        * Checks if there exists a question for the categoryId & exhibitorId combination.
        * A question is based upon a certain Category and Exhibitor combination.
        */
        public Task<Question> GetQuestion(int categoryId, int exhibitorId)
        {
            //TODO there are more than 1 questions related to a specific CategoryExhibitor (which shouldn't be, but is...)
            //TODO normally this should be done with SingleOrDefaultAsync
            return _questions.FirstOrDefaultAsync(q =>
                q.CategoryExhibitor.CategoryId == categoryId && q.CategoryExhibitor.ExhibitorId
                == exhibitorId);
        }

        public Task<List<Question>> GetAll()
        {
            var questions = _questions.Include(q => q.CategoryExhibitor)
                .ThenInclude(catExh => catExh.Exhibitor)
                .Include(q => q.CategoryExhibitor)
                .ThenInclude(catExh => catExh.Category)
                .ToListAsync();

            return questions;
        }

        public Task<List<Question>> GetAllLight()
        {
            var questions = _questions.Include(q => q.CategoryExhibitor)
                .ThenInclude(catExh => catExh.Exhibitor)
                .Include(q => q.CategoryExhibitor).ThenInclude(catExh => catExh.Category)
                .Select(q => MapQuestion(q))
                .ToListAsync();

            return questions;
        }

        private Question MapQuestion(Question question)
        {
            var q = new Question
            {
                Id = question.Id,
                Answer = question.Answer,
                QuestionText = question.QuestionText,
                CategoryExhibitor = new CategoryExhibitor
                {
                    CategoryId = question.CategoryExhibitor.CategoryId,
                    Category = new Category
                    {
                        Id = question.CategoryExhibitor.Category.Id,
                        Name = question.CategoryExhibitor.Category.Name,
                        Photo = question.CategoryExhibitor.Category.Photo,
                        Description = question.CategoryExhibitor.Category.Description
                    },
                    ExhibitorId = question.CategoryExhibitor.ExhibitorId,
                    Exhibitor = new Exhibitor
                    {
                        Id = question.CategoryExhibitor.Exhibitor.Id,
                        Name = question.CategoryExhibitor.Exhibitor.Name,
                        X = question.CategoryExhibitor.Exhibitor.X,
                        Y = question.CategoryExhibitor.Exhibitor.Y,
                        GroupsAtExhibitor = question.CategoryExhibitor.Exhibitor.GroupsAtExhibitor,
                        ExhibitorNumber = question.CategoryExhibitor.Exhibitor.ExhibitorNumber
                    }
                }
            };
            return q;
        }

        public Task<Question> GetById(int id)
        {
            var question = _questions.Include(q => q.CategoryExhibitor)
                .ThenInclude(catExh => catExh.Exhibitor)
                .Include(q => q.CategoryExhibitor)
                .ThenInclude(catExh => catExh.Category).SingleOrDefaultAsync(c => c.Id == id);
            return question;
        }

        public async Task<Question> GetByIdLight(int id)
        {
            var question = await _questions.SingleOrDefaultAsync(c => c.Id == id);
            return MapQuestion(question);
        }

        public Task Add(Question question)
        {
            return _questions.AddAsync(question);
        }

        public async Task<Question> EditQuestion(int questionId, string questionText, string answerText,
            CategoryExhibitor ce)
        {
            var question = await GetById(questionId);
            if (question == null) return null;
            question.Answer = answerText;
            question.QuestionText = questionText;
            question.CategoryExhibitor = ce;
            return _questions.Update(question).Entity;
        }

        public void Remove(Question question)
        {
            _questions.Remove(question);
        }
        
        public void RemoveAllQuestions(IEnumerable<Question> questions)
        {
            _questions.RemoveRange(questions);
        }
        
        public Task SaveChanges()
        {
            return _dbContext.SaveChangesAsync();
        }
    }
}