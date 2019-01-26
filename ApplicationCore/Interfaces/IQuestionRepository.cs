using System.Collections.Generic;
using System.Threading.Tasks;
using ApplicationCore.Entities;

namespace ApplicationCore.Interfaces
{
    public interface IQuestionRepository
    {
        Task<List<Question>> GetAll();
        Task<List<Question>> GetBasic();
        Task<List<Question>> GetAllLight();
        Task<Question> GetById(int QuestionId);
        Task<Question> GetByIdLight(int QuestionId);
        Question Add(Question Question);
        Task<Question> EditQuestion(int questionId, string questionText, string answerText, CategoryExhibitor ce);
        Question Remove(Question Question);
        Task<int> SaveChanges();
    }
}