using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class AssignmentBackupConfiguration : IEntityTypeConfiguration<Assignment>
    {
        public void Configure(EntityTypeBuilder<Assignment> builder)
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
            builder.Property(t => t.Question.QuestionText)
                .HasColumnName("QuestionText");
            builder.Property(t => t.Answer)
                .HasColumnName("Answer");
            builder.Property(t => t.Extra);
            builder.Property(t => t.Submitted)
                .HasColumnName("Submitted");
            builder.Property(t => t.CreationDate);
            builder.Property(t => t.SubmissionDate);
        }
    }
}