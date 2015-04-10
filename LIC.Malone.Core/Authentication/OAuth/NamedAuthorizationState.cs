using System;
using DotNetOpenAuth.OAuth2;
using Newtonsoft.Json;

namespace LIC.Malone.Core.Authentication.OAuth
{
	public class NamedAuthorizationState
	{
		public Guid Guid { get; set; }
		public string Name { get; set; }
		public IAuthorizationState AuthorizationState { get; set; }

		[JsonConstructor]
		public NamedAuthorizationState(Guid guid, string name, AuthorizationState authorizationState)
		{
			Guid = guid;
			Name = name;
			AuthorizationState = authorizationState;
		}

		public NamedAuthorizationState(string name, IAuthorizationState authorizationState)
		{
			Guid = Guid.NewGuid();
			Name = name;
			AuthorizationState = authorizationState;
		}
	}
}