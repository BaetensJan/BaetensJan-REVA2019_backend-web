using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class GroupConfiguration : IEntityTypeConfiguration<Group>
    {
        public void Configure(EntityTypeBuilder<Group> builder)
        {
            //Table
            builder.ToTable("Group");
            builder.HasKey(t => t.Id);

            //Prop
            builder.Property(t => t.Id).ValueGeneratedOnAdd();
            builder.Property(t => t.MembersAsString) // leden
                .HasColumnName("Members");

            builder.Property(t => t.Name)
                .HasColumnName("GroupName")
                .HasMaxLength(35)
                .IsRequired();
//            builder.Property(t => t.Password);
            //builder.Property(t => t.School).IsRequired().HasMaxLength(100);
            builder.Property(t => t.CreationDate);
            builder.Property(g => g.ApplicationUserId);
        }
    }
}