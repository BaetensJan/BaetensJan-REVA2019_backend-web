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

        public async Task<AssignmentBackup> GetById(int id)
        {
            var backupAssignment = await _backupAssignments
                .SingleOrDefaultAsync(c => c.Id == id);
            return backupAssignment;
        }
        
        public async Task<AssignmentBackup> Add(Assignment assignment, string schoolName, string groupName, bool createdExhibitor)
        {
            var backupAssignment = new AssignmentBackup
            {
                Extra = assignment.Extra,
                Notes = assignment.Notes,
                Photo = assignment.Photo,
                Answer = assignment.Answer,
                QuestionText = createdExhibitor ?
                    "Neem een foto van de stand (een selfie van de groep met exposant op de achtergrond is ook goed)." 
                : assignment.Question.QuestionText,
                CreatedExhibitor = createdExhibitor,
                GroupName = groupName,
                SchoolName = schoolName,
                Submitted = assignment.Submitted,
                CreationDate = assignment.CreationDate,
                SubmissionDate = assignment.SubmissionDate
            };

            await _backupAssignments.AddAsync(backupAssignment);
            await SaveChanges();

            return backupAssignment;
        }

        public Task SaveChanges()
        {
            return _dbContext.SaveChangesAsync();
        }
    }
}