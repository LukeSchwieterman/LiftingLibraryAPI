using LiftingLibrary.Security.Models;
using System.ComponentModel.DataAnnotations;

namespace LiftingLibraryAPI.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string PasswordHash { get; set; }
        [Required]
        public string Salt { get; set; }
        public DateTime CreatedAt { get; set; }

        public User() { }

        public User(RegisterUser registerUser, PasswordHash passwordHash)
        {
            Name = registerUser.Name;
            Email = registerUser.Email;
            PasswordHash = passwordHash.Password;
            Salt = passwordHash.Salt;
            CreatedAt = DateTime.Now;
        }

        public List<Workout> Workouts { get; set; } = new List<Workout>();
        public List<WorkoutDetail> WorkoutDetails { get; set; } = new List<WorkoutDetail>();
    }

    public class RegisterUser
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
    }

    public class LoginUser
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class UserContext
    {
        public int UserId { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }

        public UserContext() { }

        public UserContext(User user)
        {
            UserId = user.UserId;
            Name = user.Name;
            Email = user.Email;
            CreatedAt = user.CreatedAt;
        }
    }
}