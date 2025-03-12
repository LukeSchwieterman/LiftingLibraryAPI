using LiftingLibrary.Security;
using LiftingLibrary.Security.Models;
using LiftingLibraryAPI.Context;
using LiftingLibraryAPI.Models;
using LiftingLibraryAPI.Security.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace LiftingLibraryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ILogger<UsersController> _logger;
        private readonly ITokenGenerator _tokenGenerator;

        public UsersController(ApplicationDbContext context, IPasswordHasher passwordHasher, ITokenGenerator tokenGenerator, ILogger<UsersController> logger)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _tokenGenerator = tokenGenerator;
            _logger = logger;          
        }

        [HttpGet("/Email/{email}")]
        public async Task<ActionResult<UserContext>> GetUser(string email)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(user => user.Email == email);

                if (user == null)
                {
                    // Log that the user with the given email was not found
                    _logger.LogInformation("User with email {Email} not found.", email);
                    return NotFound($"User with email {email} not found.");
                }
                else
                {
                    return Ok(new UserContext(user)); // Return the user as UserContext if the user was found.
                }
            }
            catch (Exception ex)
            {
                // Log the exception with user email
                _logger.LogWarning(ex, "User with email {Email} not found.", email);
                return StatusCode(500, "A database error occurred while trying to return the user.");
            }
        }

        [HttpGet("/UserId/{id}")]
        public async Task<ActionResult<UserContext>> GetUser(int id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);

                // Check if user exists
                if (user == null)
                {
                    // Log that the user with the given ID was not found
                    _logger.LogInformation($"User with ID {id} not found.", id);
                    return NotFound($"User with ID {id} not found.");
                }
                else
                {
                    return Ok(new UserContext(user)); // Return the user as UserContext if the user was found.
                }
            }
            catch (Exception ex)
            {
                // Log the exception with user id
                _logger.LogError(ex, $"User with ID {id} not found.", id);
                return StatusCode(500, "A database error occurred while trying to return the user.");
            }
        }

        [HttpPost("Login")]
        public async Task<ActionResult<LoginResponse>> LoginUser([FromBody] LoginUser loginUser)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(user => user.Email == loginUser.Email);

                // Check if user exists and verify password hash
                if (user != null && _passwordHasher.VerifyHashMatch(user.PasswordHash, loginUser.Password, user.Salt))
                {
                    // Creates JWT for secure login
                    var token = _tokenGenerator.GenerateToken(user);                   

                    return Ok(new LoginResponse(new UserContext(user), token)); // Return the user and token as LoginResponse if login is successful
                }
                else
                {
                    // Do not reveal if email or password was wrong to prevent security risks
                    return BadRequest("Wrong Email or Password.");
                }
            }
            catch (Exception ex)
            {
                // Log the exception with user email (but not the password)
                _logger.LogError(ex, "A database error occurred while trying to log you in with email {Email}.", loginUser.Email);
                return StatusCode(500, "A database error occurred while trying to log you in.");
            }
        }

        [HttpPost]
        public async Task<ActionResult<UserContext>> AddUser([FromBody] RegisterUser registerUser)
        {
            try
            {
                // Make sure that the given email isn't already tied to an account.
                var existingUser = await _context.Users.SingleOrDefaultAsync(user => user.Email == registerUser.Email);

                if (existingUser != null)
                {
                    // Log that a user already exists.
                    _logger.LogInformation("A user with the given email already exisits {Email}.", registerUser.Email);
                    return BadRequest("Email already in use.");
                }

                // Encrypt the given user password to save in the database.
                PasswordHash passwordHash = _passwordHasher.ComputeHash(registerUser.Password);

                User user = new User(registerUser, passwordHash);

                _context.Users.Add(user);

                await _context.SaveChangesAsync();

                return Ok(new UserContext(user)); // Return the added user data as a UserContext
            }
            catch (Exception ex)
            {
                // Log the exception with user email (but not the password)
                _logger.LogError(ex, "A database error occurred while trying to add the user {Email}.", registerUser.Email);
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult> RemoveUser(int id)
        {
            try
            {
                // Find if user with the given Id is in the database
                var userToDelete = await _context.Users.FindAsync(id);

                if (userToDelete != null)
                {
                    // Remove the User from the database
                    _context.Users.Remove(userToDelete);

                    await _context.SaveChangesAsync();

                    _logger.LogInformation($"Removed user with id: {id}");

                    return NoContent();
                }
                else
                {
                    // Log that the user was not found
                    _logger.LogInformation($"User was not found with given id: {id}");
                    return NotFound("User was not deleted because the id was not found.");
                }
            }
            catch (Exception ex)
            {
                // Log the exception with user id
                _logger.LogError(ex, $"A database error occurred while trying to delete the user {id}.", id);
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

        [Authorize]
        [HttpPut]
        public async Task<ActionResult<UserContext>> UpdateUser([FromBody] UserContext user)
        {
            try
            {
                // Find the user by the given UserId
                var userToUpdate = await _context.Users.FindAsync(user.UserId);

                if (userToUpdate == null)
                {
                    // Log and return a response if the user was not found
                    _logger.LogWarning($"User with ID {user.UserId} not found in the database.");
                    return NotFound("User ID not in Database.");
                }
                else
                {
                    userToUpdate.Name = user.Name;
                    userToUpdate.Email = user.Email;

                    await _context.SaveChangesAsync();

                    return Ok(new UserContext(userToUpdate)); // Return the updated user data as a UserContext
                }
            }
            catch (Exception ex)
            {
                // Log the exception with user id
                _logger.LogError(ex, $"A database error occurred while updating user with ID {user.UserId}.");
                return StatusCode(500, "An unexpected error occurred.");
            }
        }
    }
}