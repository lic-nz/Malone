using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNetOpenAuth.OAuth2;

namespace LIC.Malone.Core.Authentication.OAuth
{
	public class NamedAuthorizationState
	{
		public string Name { get; private set; }
		public IAuthorizationState AuthorizationState { get; private set; }

		public NamedAuthorizationState(string name, IAuthorizationState authorizationState)
		{
			Name = name;
			AuthorizationState = authorizationState;
		}
	}
}