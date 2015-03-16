using System;
using System.Collections.Generic;
using LIC.Malone.Core.Authentication;
using LIC.Malone.Core.Authentication.OAuth;

namespace LIC.Malone.Client.Desktop.Messages
{
	public class ConfigurationLoaded
	{
		public List<OAuthApplication> Applications { get; private set; }
		public List<Uri> AuthenticationUrls { get; private set; }
		public UserCredentials UserCredentials { get; private set; }

		public ConfigurationLoaded(List<OAuthApplication> applications, List<Uri> authenticationUrls, UserCredentials userCredentials)
		{
			Applications = applications;
			AuthenticationUrls = authenticationUrls;
			UserCredentials = userCredentials;
		}
	}
}
