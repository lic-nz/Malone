using System;
using DotNetOpenAuth.OAuth2;
using Newtonsoft.Json;

namespace LIC.Malone.Core.Authentication.OAuth
{
	public enum NamedAuthorizationStateOrigin
	{
		History,
		Current
	}

	public class NamedAuthorizationState
	{
		public Guid Guid { get; set; }
		public NamedAuthorizationStateOrigin NamedAuthorizationStateOrigin { get; set; }
		public string Name { get; set; }
		public IAuthorizationState AuthorizationState { get; set; }

		[JsonConstructor]
		public NamedAuthorizationState(Guid guid, string name, AuthorizationState authorizationState)
		{
			Guid = guid;
			NamedAuthorizationStateOrigin = NamedAuthorizationStateOrigin.History;
			Name = name + " (from historical request)";
			AuthorizationState = authorizationState;
		}

		public NamedAuthorizationState(string name, IAuthorizationState authorizationState)
		{
			Guid = Guid.NewGuid();
			NamedAuthorizationStateOrigin = NamedAuthorizationStateOrigin.Current;
			Name = name;
			AuthorizationState = authorizationState;
		}
	}
}