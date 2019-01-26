using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class AssignmentConfiguration : IEntityTypeConfiguration<Assignment>
    {
        public void Configure(EntityTypeBuilder<Assignment> builder)
        {
            //Table
            builder.ToTable("Assignment");
            builder.HasKey(t => t.Id);

            //Relations
//            builder.HasOne(t => t.Exhibitor);
            // .WithMany(t => t.Questions)
            // .OnDelete(DeleteBehavior.Restrict);

            //Props
            builder.Property(t => t.Id).ValueGeneratedOnAdd();
            builder.Property(t => t.Notes)
                .HasColumnName("Notes");
            builder.Property(t => t.Photo); // wordt apart opgeslaan (ik heb van ignore naar Property gezet)
            builder.HasOne(t => t.Question);
            builder.Property(t => t.Answer).HasColumnName("Answer");
            builder.Property(t => t.Submitted).HasColumnName("Submitted");
        }
    }
}