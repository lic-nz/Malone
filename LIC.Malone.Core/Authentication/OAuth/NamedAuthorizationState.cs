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
		public bool IsHistorical { get; private set; }
		public bool ShouldRefresh { get; set; }

		[JsonConstructor]
		public NamedAuthorizationState(Guid guid, string name, AuthorizationState authorizationState, bool shouldRefresh)
		{
			Guid = guid;
			Name = name;
			AuthorizationState = authorizationState;
			ShouldRefresh = shouldRefresh;
			IsHistorical = true;
		}

		public NamedAuthorizationState(string name, IAuthorizationState authorizationState, bool shouldRefresh)
		{
			Guid = Guid.NewGuid();
			Name = name;
			AuthorizationState = authorizationState;
			ShouldRefresh = shouldRefresh;
			IsHistorical = false;
		}
	}
}