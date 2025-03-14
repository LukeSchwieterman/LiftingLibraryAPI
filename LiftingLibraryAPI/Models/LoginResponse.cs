namespace LiftingLibraryAPI.Models
{
	public class LoginResponse
	{
		public UserContext UserContext { get; set; }
		public string Token { get; set; }

		public LoginResponse(UserContext userContext, string token)
		{
			UserContext = userContext;
			Token = token;
		}
	}
}
