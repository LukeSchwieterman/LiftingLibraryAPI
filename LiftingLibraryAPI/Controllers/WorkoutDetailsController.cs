using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LiftingLibraryAPI.Context;
using LiftingLibraryAPI.Models;

namespace LiftingLibraryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkoutDetailsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public WorkoutDetailsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/WorkoutDetails
        [HttpGet]
        public async Task<ActionResult<IEnumerable<WorkoutDetail>>> GetWorkoutDetails()
        {
            return await _context.WorkoutDetails.ToListAsync();
        }

        // GET: api/WorkoutDetails/5
        [HttpGet("{id}")]
        public async Task<ActionResult<WorkoutDetail>> GetWorkoutDetail(int id)
        {
            var workoutDetail = await _context.WorkoutDetails.FindAsync(id);

            if (workoutDetail == null)
            {
                return NotFound();
            }

            return workoutDetail;
        }

        // PUT: api/WorkoutDetails/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutWorkoutDetail(int id, WorkoutDetail workoutDetail)
        {
            if (id != workoutDetail.DetailId)
            {
                return BadRequest();
            }

            _context.Entry(workoutDetail).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WorkoutDetailExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/WorkoutDetails
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<WorkoutDetail>> PostWorkoutDetail(WorkoutDetail workoutDetail)
        {
            _context.WorkoutDetails.Add(workoutDetail);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetWorkoutDetail", new { id = workoutDetail.DetailId }, workoutDetail);
        }

        // DELETE: api/WorkoutDetails/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWorkoutDetail(int id)
        {
            var workoutDetail = await _context.WorkoutDetails.FindAsync(id);
            if (workoutDetail == null)
            {
                return NotFound();
            }

            _context.WorkoutDetails.Remove(workoutDetail);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool WorkoutDetailExists(int id)
        {
            return _context.WorkoutDetails.Any(e => e.DetailId == id);
        }
    }
}
