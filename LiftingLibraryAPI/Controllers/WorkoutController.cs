using LiftingLibrary.Security;
using LiftingLibrary.Security.Models;
using LiftingLibraryAPI.Context;
using LiftingLibraryAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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

        public WorkoutController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<Workout>> GetWorkout(int id)
        {
            try
            {
                var workout = await _context.Workouts.FindAsync(id);

                if (workout == null)
                {
                    return NotFound("Unable to find Workout");
                }
                else
                {
                    return Ok(workout);
                }
            }
            catch (Exception)
            {
                return StatusCode(500, $"A database error occurred while trying to find the workout with WorkoutId {id}.");
            }
        }
    }
}
