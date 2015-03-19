using DotNetOpenAuth.OAuth2;
using Newtonsoft.Json;

namespace LIC.Malone.Core.Authentication.OAuth
{
	public class NamedAuthorizationState
	{
		public string Name { get; set; }
		public IAuthorizationState AuthorizationState { get; set; }

		public NamedAuthorizationState(string name, IAuthorizationState authorizationState)
		{
			Name = name;
			AuthorizationState = authorizationState;
		}

		[JsonConstructor]
		public NamedAuthorizationState(string name, AuthorizationState authorizationState)
		{
			Name = name;
			AuthorizationState = authorizationState;
		}
	}
}