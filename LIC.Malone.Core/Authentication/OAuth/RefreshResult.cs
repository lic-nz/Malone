using DotNetOpenAuth.OAuth2;

namespace LIC.Malone.Core.Authentication.OAuth
{
	public class RefreshResult
	{
		public IAuthorizationState AuthorizationState { get; set; }
		public string Error { get; set; }

		public bool HasError
		{
			get { return Error != null; }
		}
	}
}
