﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LiftingLibraryAPI.Models
{
    public class WorkoutDetail
    {
        [Key]
        public int DetailId { get; set; }
        [ForeignKey("WorkoutId")]
        public int WorkoutId { get; set; }
        public int UserId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Category { get; set; }
        public int Sets { get; set; }
        public int Reps { get; set; }
        public int Weight { get; set; }

        [ForeignKey("WorkoutID")]
        public Workout Workout { get; set; }
        [ForeignKey("UserID")]
        public User User { get; set; }
    }
}