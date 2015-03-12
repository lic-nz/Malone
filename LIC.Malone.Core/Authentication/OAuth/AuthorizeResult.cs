using DotNetOpenAuth.OAuth2;

namespace LIC.Malone.Core.Authentication.OAuth
{
	public class AuthorizeResult
	{
		public IAuthorizationState AuthorizationState { get; set; }
		public string Error { get; set; }

		public bool HasError
		{
			get { return Error != null; }
		}
	}
}
