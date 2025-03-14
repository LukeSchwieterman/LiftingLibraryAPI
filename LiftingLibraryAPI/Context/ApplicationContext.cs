using LiftingLibraryAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace LiftingLibraryAPI.Context
{
	public class ApplicationDbContext : DbContext
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

		// Creates Database sets for each of the tables in the database
		public DbSet<User> Users { get; set; }
		public DbSet<Workout> Workouts { get; set; }
		public DbSet<WorkoutDetail> WorkoutDetails { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			// One Workout has many WorkoutDetails
			modelBuilder.Entity<WorkoutDetail>().HasOne(wd => wd.Workout).WithMany(w => w.WorkoutDetails).HasForeignKey(wd => wd.WorkoutId);

			// One user has many Workouts
			modelBuilder.Entity<Workout>().HasOne(w => w.User).WithMany(u => u.Workouts).HasForeignKey(w => w.UserId);
		}
	}
}