using LiftingLibraryAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace LiftingLibraryAPI.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Workout> Workouts { get; set; }
        public DbSet<WorkoutDetail> WorkoutDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WorkoutDetail>().HasOne(wd => wd.Workout).WithMany(w => w.WorkoutDetails).HasForeignKey(wd => wd.WorkoutId);
            modelBuilder.Entity<WorkoutDetail>().HasOne(wd => wd.User).WithMany(u => u.WorkoutDetails).HasForeignKey(wd => wd.UserId);
            modelBuilder.Entity<Workout>().HasOne(w => w.User).WithMany(u => u.Workouts).HasForeignKey(w => w.UserId);
        }
    }
}