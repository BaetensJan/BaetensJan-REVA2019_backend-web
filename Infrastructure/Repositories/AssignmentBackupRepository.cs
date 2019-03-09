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
        private readonly DbSet<AssignmentBackup> _backupAssignments;

        public AssignmentBackupRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            _backupAssignments = dbContext.BackupAssignments;
        }

        public async Task Add(Assignment assignment, string schoolName, string groupName, bool createdExhibitor)
        {
            var backupAssignment = new AssignmentBackup
            {
                Extra = assignment.Extra,
                Notes = assignment.Notes,
                Photo = assignment.Photo,
                Answer = assignment.Answer,
                QuestionText = assignment.Question.QuestionText,
                CreatedExhibitor = createdExhibitor,
                GroupName = groupName,
                SchoolName = schoolName,
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