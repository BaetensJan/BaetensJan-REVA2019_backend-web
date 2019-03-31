using System.Threading.Tasks;
using ApplicationCore.Entities;
using ApplicationCore.Interfaces;

namespace ApplicationCore.Services
{
    public class AssignmentManager
    {
        private readonly IAssignmentRepository _assignmentRepository;

        public AssignmentManager(IAssignmentRepository assignmentRepository)
        {
            _assignmentRepository = assignmentRepository;
        }

        public async Task<Assignment> CreateAssignment(Question question, bool isExtraRound, Group group)
        {
            // Create assignment and Add to the groups assignments.
            var assignment = new Assignment(question, isExtraRound);
            group.AddAssignment(assignment);

            var exhibitor = assignment.Question.CategoryExhibitor.Exhibitor;
            exhibitor.GroupsAtExhibitor++;
            exhibitor.TotalNumberOfVisits++;

            question.Answered++;

            await _assignmentRepository.SaveChanges();

            return assignment;
        }
    }
}