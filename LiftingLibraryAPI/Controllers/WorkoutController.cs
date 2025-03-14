using LiftingLibraryAPI.Context;
using LiftingLibraryAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LiftingLibraryAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class WorkoutController : ControllerBase
	{
		private readonly ApplicationDbContext _context;
		private readonly ILogger<WorkoutController> _logger;

		public WorkoutController(ApplicationDbContext context, ILogger<WorkoutController> logger)
		{
			_context = context;
			_logger = logger;
		}

        // GET a specific workout by ID
        [HttpGet("{id}")]
		public async Task<ActionResult<Workout>> GetWorkout(int id)
		{
			try
			{
				var workout = await _context.Workouts.FindAsync(id);

				if (workout == null)
				{
                    _logger.LogInformation($"Workout with ID {id} not found.", id);
                    return NotFound("Unable to find Workout");
				}
				else
				{
					return Ok(workout);
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"An error occurred while trying to find the workout with WorkoutId {id}.");
				return StatusCode(500, $"An error occurred while trying to find the workout with WorkoutId {id}.");
			}
		}

        // GET workouts by user ID
        [HttpGet("by-userid/{id}")]
		public async Task<ActionResult<List<Workout>>> GetWorkoutsFromUser(int id)
		{
			try
			{
				List<Workout> workouts = await _context.Workouts.Where(workout => workout.UserId == id).ToListAsync();

				if (!workouts.Any())
				{
                    _logger.LogInformation($"User with ID {id} not found.", id);
                    return NotFound("The user has no workouts");
				}
				else
				{
					return Ok(workouts);
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"An error occurred while trying to find the workouts with the UserId {id}.");
				return StatusCode(500, $"An error occurred while trying to find the workouts with the UserId {id}.");
			}
			
		}

        // GET workouts by date for a specific user
        [HttpGet("by-date-userid/{userId}/{date}")]
		public async Task<ActionResult<List<Workout>>> GetWorkoutsByDate(int userId, string date)
		{
			try
			{
				if (!DateTime.TryParse(date, out var parsedDate))
				{
					return BadRequest("Invalid date format.");
				}
				List<Workout> workouts = await _context.Workouts.Where(workout => workout.Date.Date == parsedDate.Date && workout.UserId == userId).ToListAsync();

				if (!workouts.Any())
				{
                    _logger.LogInformation($"User with ID {userId} and workouts at date {date} was not found.", userId, date);
                    return NotFound($"No workouts found with given date and userId: {date}, {userId}");
				}
				else
				{
					return Ok(workouts);
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"An error occurred while trying to find the workouts with the Date {date} and UserId {userId}.");
				return StatusCode(500, $"An error occurred while trying to find the workouts with the Date {date} and UserId {userId}.");
			}    
		}

        // POST a new workout
        [HttpPost]
		public async Task<ActionResult<Workout>> AddWorkout([FromBody] CreatedWorkout createdWorkout)
		{
			try
			{
                var user = await _context.Users.FindAsync(createdWorkout.UserId);

                if (user == null)
                {
                    return NotFound("No UserId was associated with the workout.");
                }
                
                var workout = new Workout(createdWorkout, user);
				
				_context.Workouts.Add(workout);

				await _context.SaveChangesAsync();

				return CreatedAtAction(nameof(GetWorkout), new { id = workout.WorkoutId }, workout);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"An error occurred while trying to add the workout {createdWorkout}.");
				return StatusCode(500, "An error occurred while trying to add the workout.");
			}
		}

        // PUT to update an existing workout
        [HttpPut]
		public async Task<ActionResult<Workout>> UpdateWorkout([FromBody] CreatedWorkout workout)
		{
			try
			{
				var workoutToUpdate = await _context.Workouts.FindAsync(workout.WorkoutId);

				if (workoutToUpdate == null)
				{
					return NotFound($"Workout was not found with the given ID: {workout.WorkoutId}");
				}
				else
				{
					workoutToUpdate.Date = workout.Date;
					workoutToUpdate.Notes = workout.Notes;

					await _context.SaveChangesAsync();

					return Ok(workoutToUpdate);
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"An error occurred while trying to update the workout at ID {workout.WorkoutId}");
				return StatusCode(500, $"An error occurred while trying to update the workout at ID {workout.WorkoutId}");
			}
		}

        // DELETE a workout by ID
        [HttpDelete("{id}")]
		public async Task<ActionResult> DeleteWorkout(int id)
		{
			try
			{
				var workoutToDelete = await _context.Workouts.FindAsync(id);

				if (workoutToDelete == null)
				{
					return NotFound($"Workout was not found with the given ID: {id}");
				}
				else
				{
					_context.Workouts.Remove(workoutToDelete);

					await _context.SaveChangesAsync();

					return NoContent();
				}
			}
			catch (Exception ex)
			{
                _logger.LogError(ex, $"An error occurred while trying to delete the workout at ID {id}");
                return StatusCode(500, $"An error occurred while trying to delete the workout at ID {id}");
            }
		}
	}
}
