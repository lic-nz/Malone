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
		private readonly string _configFolder;

		public Config()
		{
			_configFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Malone", "config");

			if (!Directory.Exists(_configFolder))
			{
				try
				{
					Directory.CreateDirectory(_configFolder);
				}
				catch (Exception e)
				{
					throw new Exception("Can not create directory: " + _configFolder, e);
				}
			}
		}

		public List<Request> GetHistory()
		{
			var history = new List<Request>();
			var path = Path.Combine(_configFolder, "history.json");

			if (!File.Exists(path))
				return history;

			var json = File.ReadAllText(path);

			if (!json.Any())
				return history;

			try
			{
				history = JsonConvert.DeserializeObject<List<Request>>(json);
			}
			catch (Exception e)
			{
				throw new Exception("Could not deserialize history: " + path, e);
			}

			return history;
		}

		public List<OAuthApplication> GetOAuthApplications()
		{
			var applications = new List<OAuthApplication>();
			var path = Path.Combine(_configFolder, "oauth-applications.json");

			if (!File.Exists(path))
				return applications;

			var json = File.ReadAllText(path);

			if (!json.Any())
				return applications;

			try
			{
				applications = JsonConvert.DeserializeObject<List<OAuthApplication>>(json);
			}
			catch (Exception e)
			{
				throw new Exception("Could not deserialize OAuth applications: " + path, e);
			}

			return applications;
		}

		public List<Uri> GetOAuthAuthenticationUrls()
		{
			var authenticationUrls = new List<Uri>();
			var path = Path.Combine(_configFolder, "oauth-authentication-urls.json");

			if (!File.Exists(path))
				return authenticationUrls;

			var json = File.ReadAllText(path);

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
				throw new Exception("Could not deserialize OAuth URLs: " + path, e);
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

			var path = Path.Combine(_configFolder, "oauth-user-credentials.json");

			if (!File.Exists(path))
				return userCredentials;

			var json = File.ReadAllText(path);

			if (!json.Any())
				return userCredentials;

			try
			{
				userCredentials = JsonConvert.DeserializeObject<UserCredentials>(json);
			}
			catch (Exception e)
			{
				throw new Exception("Could not deserialize OAuth creds: " + path, e);
			}

			return userCredentials;
		}
	}
}