using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class ExhibitorConfiguration : IEntityTypeConfiguration<Exhibitor>
    {
        public void Configure(EntityTypeBuilder<Exhibitor> builder)
        {
            //Table
            builder.ToTable("Exhibitor");
            builder.HasKey(t => t.Id);

            //Props
            builder.Property(t => t.Id).ValueGeneratedOnAdd();
            builder.Property(t => t.Name)
                .HasColumnName("Name")
                .HasMaxLength(30)
                .IsRequired();
//            builder.Property(t => t.Question); //TODO Question is een Object die een associatie heeft met Table CategoryExhibitor
            builder.Property(t => t.X);
            builder.Property(t => t.Y);
            builder.Property(t => t.ExhibitorNumber);
            builder.Property(t => t.GroupsAtExhibitor);
            builder.Property(t => t.CreationDate);
        }
    }
}