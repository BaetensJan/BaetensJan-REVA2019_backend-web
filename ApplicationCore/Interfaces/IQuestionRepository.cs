using System.Collections.Generic;
using System.Threading.Tasks;
using ApplicationCore.Entities;

namespace ApplicationCore.Interfaces
{
    public interface IQuestionRepository
    {
        Task<List<Question>> GetAll();
        Task<List<Question>> GetAllLight();
        Task<Question> GetById(int QuestionId);
        Task<Question> GetQuestion(int categoryId, int exhibitorId);
        Task Add(Question Question);
        Task<Question> EditQuestion(int questionId, string questionText, string answerText, CategoryExhibitor ce);
        void Remove(Question Question);
        void RemoveAllQuestions(IEnumerable<Question> questions);
        Task SaveChanges();
    }
}