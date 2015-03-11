using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DotNetOpenAuth.OAuth2;
using LIC.Malone.Core.Authentication.OAuth;

namespace LIC.Malone.Core
{
	public class ApiClient
	{
		public string Send(Request request)
		{
			return Send(request.ToHttpWebRequest());
		}

		public string Send(HttpWebRequest request)
		{
			var response = request.GetResponse() as HttpWebResponse;

			if (response == null)
				throw new Exception(string.Format("Failed to get response for URL '{0}'", request.RequestUri));

			var stream = response.GetResponseStream();

			if (stream == null)
				throw new Exception(string.Format("Failed to get stream for URL '{0}'", request.RequestUri));

			var s = new StreamReader(stream).ReadToEnd();

			return s;
		}

		private IAuthorizationState Auth()
		{
			var authenticationUrl = "url";
			var app = new OAuthApplication("xxxx", "yyyy", "zzzz");

			// Can be null?
			var authServer = new AuthorizationServerDescription
			{
				AuthorizationEndpoint = null,
				TokenEndpoint = new Uri(authenticationUrl)
			};

			var client = new UserAgentClient(authServer, app.ClientIdentifier, ClientCredentialApplicator.PostParameter(app.ClientSecret));

			var state = client.ExchangeUserCredentialForToken("1111", "222");

			return state;
		}
	}
}
