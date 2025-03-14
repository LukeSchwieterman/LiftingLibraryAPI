using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LiftingLibraryAPI.Models
{
	public class Workout
	{
		[Key]
		public int WorkoutId { get; set; }
		public int UserId { get; set; }
		[Required]
		public DateTime Date { get; set; }
		public string? Notes { get; set; }

		public List<WorkoutDetail> WorkoutDetails { get; set; } = new List<WorkoutDetail>();

		[ForeignKey("UserId")]
		public User User { get; set; }

		public Workout() { }

		public Workout(CreatedWorkout createdWorkout, User user)
		{
			UserId = createdWorkout.UserId;
			Date = createdWorkout.Date;
			Notes = createdWorkout.Notes;
			User = user;
		}
	}

    public class CreatedWorkout
    {
		public int WorkoutId { get; set; }
		public int UserId { get; set; }
        public DateTime Date { get; set; }
        public string? Notes { get; set; }
    }
}