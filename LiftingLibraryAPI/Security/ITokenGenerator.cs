using LiftingLibraryAPI.Models;

namespace LiftingLibrary.Security
{
	public interface ITokenGenerator
	{
		/// <summary>
		/// Generates a new authentication token.
		/// </summary>
		/// <param name="username">The user's username</param>
		/// <returns></returns>
		string GenerateToken(User user);

		/// <summary>
		/// Generates a new authentication token.
		/// </summary>
		/// <param name="username">The user's username</param>
		/// <param name="role">The user's role</param>
		/// <returns></returns>
		string GenerateToken(User user, string role);
	}
}
