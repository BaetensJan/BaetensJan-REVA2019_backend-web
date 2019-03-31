using System.Collections.Generic;
using System.Threading.Tasks;
using ApplicationCore.Entities;

namespace ApplicationCore.Interfaces
{
    public interface IQuestionRepository
    {
        Task<Question> GetQuestion(int categoryId, int exhibitorId, IEnumerable<Question> questions);
        Task<List<Question>> GetQuestions(int categoryId, int exhibitorId);
        Task<List<Question>> GetQuestions(int categoryId, int exhibitorId, List<Assignment> assignments);
        Task<List<Question>> GetQuestions(int categoryId);
        Task<List<Question>> GetQuestions(int categoryId, List<Assignment> assignments);
        Task<List<Question>> GetAll();
        Task<List<Question>> GetAllLight();
        Task<Question> GetById(int QuestionId);
        Task Add(Question Question);
        Task<Question> EditQuestion(int questionId, string questionText, string answerText, CategoryExhibitor ce);
        void Remove(Question Question);
        void RemoveAllQuestions(IEnumerable<Question> questions);
        Task SaveChanges();
    }
}