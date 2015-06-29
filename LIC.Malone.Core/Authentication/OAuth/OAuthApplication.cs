using System;
using DotNetOpenAuth.OAuth2;

namespace LIC.Malone.Core.Authentication.OAuth
{
	public class OAuthApplication
	{
		public string Name { get; private set; }
		public string ClientIdentifier { get; private set; }
		public string ClientSecret { get; private set; }

		public OAuthApplication(string name, string clientIdentifier, string clientSecret)
		{
			Name = name;
			ClientIdentifier = clientIdentifier;
			ClientSecret = clientSecret;
		}

		private UserAgentClient GetClient(Uri url)
		{
			var authServer = new AuthorizationServerDescription
			{
				AuthorizationEndpoint = null, // We are not actually using authorization, just authentication.
				TokenEndpoint = url
			};

			return new UserAgentClient(authServer, ClientIdentifier, ClientCredentialApplicator.PostParameter(ClientSecret));
		}

		public AuthorizeResult Authorize(Uri url, string username, string password)
		{
			var client = GetClient(url);

			IAuthorizationState state;

			try
			{
				state = client.ExchangeUserCredentialForToken(username, password);
			}
			catch (Exception e)
			{
				var error = e.Message;

				if (e.InnerException != null)
					error = string.Format("{0}\nInner exception: {1}", error, e.InnerException.Message);

				return new AuthorizeResult
				{
					Error = error
				};
			}

			return new AuthorizeResult
			{
				AuthorizationState = state
			};
		}

		public RefreshResult Refresh(Uri url, IAuthorizationState authorizationState)
		{
			var client = GetClient(url);

			try
			{
				var refreshed = client.RefreshAuthorization(authorizationState);

				if (!refreshed)
					return new RefreshResult
					{
						Error = "Uhm, not entirely sure what happened."
					};
			}
			catch (Exception e)
			{
				var error = e.Message;

				if (e.InnerException != null)
					error = string.Format("{0}\nInner exception: {1}", error, e.InnerException.Message);

				return new RefreshResult
				{
					Error = error
				};
			}

			return new RefreshResult
			{
				AuthorizationState = authorizationState
			};
		}
	}
}
