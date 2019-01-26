using ApplicationCore.Entities;
using Infrastructure.Configurations;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Group> Groups { get; set; }
        public DbSet<Exhibitor> Exhibitors { get; set; }
        public DbSet<Assignment> Assignments { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<CategoryExhibitor> CategoryExhibitors { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<School> Schools { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfiguration(new CategoryConfiguration());
            builder.ApplyConfiguration(new GroupConfiguration());
            builder.ApplyConfiguration(new ExhibitorConfiguration());
            builder.ApplyConfiguration(new AssignmentConfiguration());
            builder.ApplyConfiguration(new CategoryExhibitorConfiguration());
            builder.ApplyConfiguration(new QuestionConfiguration());
            builder.ApplyConfiguration(new SchoolConfiguration());
        }
    }
}