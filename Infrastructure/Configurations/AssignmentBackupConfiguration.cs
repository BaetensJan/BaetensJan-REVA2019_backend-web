using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class AssignmentBackupConfiguration : IEntityTypeConfiguration<AssignmentBackup>
    {
        public void Configure(EntityTypeBuilder<AssignmentBackup> builder)
        {
            //Table
            builder.ToTable("AssignmentBackup");
            builder.HasKey(t => t.Id);

            //Props
            builder.Property(t => t.Id)
                .ValueGeneratedOnAdd();
            builder.Property(t => t.Notes)
                .HasColumnName("Notes");
            builder.Property(t => t.Photo);
            builder.Property(t => t.QuestionText)
                .HasColumnName("QuestionText");
            builder.Property(t => t.Answer)
                .HasColumnName("Answer");
            builder.Property(t => t.Extra);
            builder.Property(t => t.SchoolName);
            builder.Property(t => t.GroupName);
            builder.Property(t => t.CreatedExhibitor);
            builder.Property(t => t.Submitted)
                .HasColumnName("Submitted");
            builder.Property(t => t.CreationDate);
            builder.Property(t => t.SubmissionDate);
        }
    }
}