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

        public List<WorkoutDetail> WorkoutDetails = new List<WorkoutDetail>();
        [ForeignKey("UserId")]
        public User User { get; set; }
    }
}