using System;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class AssignmentBackupRepository : IAssignmentBackupRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly DbSet<Assignment> _backupAssignments;

        public AssignmentBackupRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            _backupAssignments = dbContext.BackupAssignments;
        }

        public async Task Add(Assignment assignment, string schoolName, string groupName)
        {
            var question = new Question
            {
                QuestionText =$"{assignment.Question} schoolName: {schoolName}, groupName: {groupName}"
            };
            var backupAssignment = new Assignment
            {
                Extra = assignment.Extra,
                Notes = assignment.Notes,
                Photo = assignment.Photo,
                Answer = assignment.Answer,
                Question = question, 
                Submitted = assignment.Submitted,
                CreationDate = assignment.CreationDate,
                SubmissionDate = assignment.SubmissionDate
            };

            await _backupAssignments.AddAsync(backupAssignment);
            await SaveChanges();
        }

        public Task SaveChanges()
        {
            return _dbContext.SaveChangesAsync();
        }
    }
}