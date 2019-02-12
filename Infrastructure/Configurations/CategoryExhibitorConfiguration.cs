using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class CategoryExhibitorConfiguration : IEntityTypeConfiguration<CategoryExhibitor>
    {
        public void Configure(EntityTypeBuilder<CategoryExhibitor> builder)
        {
            builder.ToTable("CategoryExhibitor");
            builder
                .HasKey(ce => new {ce.CategoryId, ce.ExhibitorId});

            builder
                .HasOne(ce => ce.Category)
                .WithMany(x => x.Exhibitors)
                .HasForeignKey(ce => ce.CategoryId);

            builder
                .HasOne(ce => ce.Exhibitor)
                .WithMany(c => c.Categories);
//                .HasForeignKey(ce => ce.ExhibitorId);
        }
    }
}