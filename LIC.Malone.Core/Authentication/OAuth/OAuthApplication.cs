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
	}
}
