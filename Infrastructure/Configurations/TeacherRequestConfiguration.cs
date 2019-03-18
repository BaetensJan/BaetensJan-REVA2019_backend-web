using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class TeacherRequestConfiguration : IEntityTypeConfiguration<TeacherRequest>
    {
        public void Configure(EntityTypeBuilder<TeacherRequest> builder)
        {
            //Table
            builder.ToTable("TeacherRequest");
            builder.HasKey(t => t.Id);

            //Props
            builder.Property(t => t.Id).ValueGeneratedOnAdd();
            builder.Property(t => t.Note);
            builder.Property(t => t.Name);
            builder.Property(t => t.Surname);
            builder.Property(t => t.SchoolName);
            builder.Property(t => t.Email);
            builder.Property(t => t.CreationDate);
            builder.Property(t => t.Accepted);
        }
    }
}