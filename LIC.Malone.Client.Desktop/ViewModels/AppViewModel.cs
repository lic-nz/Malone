using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using Caliburn.Micro;
using LIC.Malone.Client.Desktop.Messages;
using LIC.Malone.Core.Authentication;
using LIC.Malone.Core.Authentication.OAuth;
using Newtonsoft.Json;
using Path = System.IO.Path;

namespace LIC.Malone.Client.Desktop.ViewModels
{
	public class AppViewModel : Conductor<object>, IHandle<ShowMainScreen>, IHandle<ShowTokensScreen>
	{
		private EventAggregator _bus;

		public MainViewModel MainViewModel { get; set; }
		public TokensViewModel TokensViewModel { get; set; }

		public AppViewModel()
		{
			_bus = IoC.Get<EventAggregator>();
			_bus.Subscribe(this);

			MainViewModel = new MainViewModel(_bus);
			TokensViewModel = new TokensViewModel(_bus);

			LoadConfig(_bus);
			
			Handle(new ShowTokensScreen());
		}

		private void LoadConfig(EventAggregator bus)
		{
			var configLocation = ConfigurationManager.AppSettings["ConfigLocation"];

			// TODO: Handle the case of no config.
			if (string.IsNullOrWhiteSpace(configLocation))
				return;

			var applications = new List<OAuthApplication>();
			var authenticationUrls = new List<Uri>();
			UserCredentials userCredentials = null;

			var path = Path.Combine(configLocation, "oauth-applications.json");

			if (File.Exists(path))
				applications = JsonConvert.DeserializeObject<List<OAuthApplication>>(File.ReadAllText(path));

			path = Path.Combine(configLocation, "oauth-authentication-urls.json");

			if (File.Exists(path))
				authenticationUrls = JsonConvert
					.DeserializeObject<List<string>>(File.ReadAllText(path))
					.Select(url => new Uri(url))
					.ToList();

			path = Path.Combine(configLocation, "oauth-user-credentials.json");

			if (File.Exists(path))
				userCredentials = JsonConvert.DeserializeObject<UserCredentials>(File.ReadAllText(path));

			bus.PublishOnUIThread(new ConfigurationLoaded(applications, authenticationUrls, userCredentials));
		}

		public void Handle(ShowMainScreen message)
		{
			ActivateItem(MainViewModel);
		}

		public void Handle(ShowTokensScreen message)
		{
			ActivateItem(TokensViewModel);
		}
	}
}