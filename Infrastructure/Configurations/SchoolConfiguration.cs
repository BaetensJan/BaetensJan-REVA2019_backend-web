using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class SchoolConfiguration : IEntityTypeConfiguration<School>
    {
        public void Configure(EntityTypeBuilder<School> builder)
        {
            builder.ToTable("School");
            builder.Property(s => s.Name);
            builder.Property(s => s.Password);
            builder.Property(s => s.Start);
            builder.HasMany(s => s.Groups);
            builder.Property(s => s.CreationDate);
        }
    }
}