using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class QuestionConfiguration : IEntityTypeConfiguration<Question>
    {
        public void Configure(EntityTypeBuilder<Question> builder)
        {
            builder.ToTable("Question");
            builder.HasKey(q => q.Id);
            builder.Property(q => q.Id).ValueGeneratedOnAdd();
            builder.Property(q => q.QuestionText);
            builder.Property(q => q.Answer);
            builder.HasOne(q => q.CategoryExhibitor); //Todo In commentaar?
            builder.Property(q => q.CreationDate);
        }
    }
}