using System;

namespace LIC.Malone.Core.Authentication.OAuth
{
	public class AuthRefreshEventArgs : EventArgs
	{
		private readonly NamedAuthorizationState _token;

		public NamedAuthorizationState Token
		{
			get { return _token; }
		}

		public AuthRefreshEventArgs(NamedAuthorizationState token)
		{
			_token = token;
		}
	}

	public delegate void AuthRefreshEventHandler(object sender, AuthRefreshEventArgs e);
}
