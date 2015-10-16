using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LIC.Malone.Core;
using LIC.Malone.Core.Authentication;
using LIC.Malone.Core.Authentication.OAuth;
using Newtonsoft.Json;

namespace LIC.Malone.Client.Desktop
{
	public class Config
	{
		private class InternalConfig
		{
			public string GitHubPersonalAccessToken { get; set; }
		}

		private readonly string _historyFile;
		private readonly string _collectionsFile;
		private readonly string _oAuthApplicationsFile;
		private readonly string _oAuthAuthenticationUrlsFile;
		private readonly string _oAuthUserCredentialsFile;
		private readonly string _configFile;

		public string GitHubPersonalAccessToken { get; private set; }

		public Config()
		{
			var configFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Malone", "config");

			if (!Directory.Exists(configFolder))
			{
				try
				{
					Directory.CreateDirectory(configFolder);
				}
				catch (Exception e)
				{
					throw new Exception("Can not create directory: " + configFolder, e);
				}
			}

			_historyFile = Path.Combine(configFolder, "history.json");
			_collectionsFile = Path.Combine(configFolder, "collections.json");
			_oAuthApplicationsFile = Path.Combine(configFolder, "oauth-applications.json");
			_oAuthAuthenticationUrlsFile = Path.Combine(configFolder, "oauth-authentication-urls.json");
			_oAuthUserCredentialsFile = Path.Combine(configFolder, "oauth-user-credentials.json");
			_configFile = Path.Combine(configFolder, "config.json");

			Load();
		}

		private void Load()
		{
			if (!File.Exists(_configFile))
				return;

			var json = File.ReadAllText(_configFile);

			if (!json.Any())
				return;

			InternalConfig config;

			try
			{
				config = JsonConvert.DeserializeObject<InternalConfig>(json);
			}
			catch (Exception e)
			{
				throw new Exception("Could not deserialize config: " + _configFile, e);
			}

			GitHubPersonalAccessToken = config.GitHubPersonalAccessToken;
		}

		public List<Request> GetHistory()
		{
			var history = new List<Request>();

			if (!File.Exists(_historyFile))
				return history;

			var json = File.ReadAllText(_historyFile);

			if (!json.Any())
				return history;

			try
			{
				history = JsonConvert.DeserializeObject<List<Request>>(json);
			}
			catch (Exception e)
			{
				throw new Exception("Could not deserialize history: " + _historyFile, e);
			}

			return history;
		}

		public void SaveHistory(IEnumerable<Request> history)
		{
			var json = JsonConvert.SerializeObject(history, Formatting.Indented);

			try
			{
				File.WriteAllText(_historyFile, json);
			}
			catch (Exception e)
			{
				throw new Exception("Could not write history: " + _historyFile, e);
			}
		}

		public List<RequestCollection> GetCollections()
		{
			var collections = new List<RequestCollection>();

			if (!File.Exists(_collectionsFile))
				return collections;

			var json = File.ReadAllText(_collectionsFile);

			if (!json.Any())
				return collections;

			try
			{
				collections = JsonConvert.DeserializeObject<List<RequestCollection>>(json);
			}
			catch (Exception e)
			{
				throw new Exception("Could not deserialize collections: " + _collectionsFile, e);
			}

			return collections;
		}

		public void SaveCollections(IEnumerable<RequestCollection> collections)
		{
			var json = JsonConvert.SerializeObject(collections, Formatting.Indented);

			try
			{
				File.WriteAllText(_collectionsFile, json);
			}
			catch (Exception e)
			{
				throw new Exception("Could not write collections: " + _collectionsFile, e);
			}
		}

		public List<OAuthApplication> GetOAuthApplications()
		{
			var applications = new List<OAuthApplication>();

			if (!File.Exists(_oAuthApplicationsFile))
				return applications;

			var json = File.ReadAllText(_oAuthApplicationsFile);

			if (!json.Any())
				return applications;

			try
			{
				applications = JsonConvert.DeserializeObject<List<OAuthApplication>>(json);
			}
			catch (Exception e)
			{
				throw new Exception("Could not deserialize OAuth applications: " + _oAuthApplicationsFile, e);
			}

			return applications;
		}

		public List<Uri> GetOAuthAuthenticationUrls()
		{
			var authenticationUrls = new List<Uri>();

			if (!File.Exists(_oAuthAuthenticationUrlsFile))
				return authenticationUrls;

			var json = File.ReadAllText(_oAuthAuthenticationUrlsFile);

			if (!json.Any())
				return authenticationUrls;

			try
			{
				authenticationUrls = JsonConvert
					.DeserializeObject<List<string>>(json)
					.Select(url => new Uri(url))
					.ToList();
			}
			catch (Exception e)
			{
				throw new Exception("Could not deserialize OAuth URLs: " + _oAuthAuthenticationUrlsFile, e);
			}

			return authenticationUrls;
		}

		public UserCredentials GetUserCredentials()
		{
			var userCredentials = new UserCredentials
			{
				Username = string.Empty,
				Password = string.Empty
			};

			if (!File.Exists(_oAuthUserCredentialsFile))
				return userCredentials;

			var json = File.ReadAllText(_oAuthUserCredentialsFile);

			if (!json.Any())
				return userCredentials;

			try
			{
				userCredentials = JsonConvert.DeserializeObject<UserCredentials>(json);
			}
			catch (Exception e)
			{
				throw new Exception("Could not deserialize OAuth creds: " + _oAuthUserCredentialsFile, e);
			}

			return userCredentials;
		}
	}
}