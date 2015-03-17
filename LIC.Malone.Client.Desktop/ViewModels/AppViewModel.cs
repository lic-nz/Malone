using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using Caliburn.Micro;
using LIC.Malone.Client.Desktop.Messages;
using LIC.Malone.Core;
using LIC.Malone.Core.Authentication;
using LIC.Malone.Core.Authentication.OAuth;
using Newtonsoft.Json;
using Path = System.IO.Path;

namespace LIC.Malone.Client.Desktop.ViewModels
{
	public class AppViewModel : Conductor<object>, IHandle<ShowTokensScreen>
	{
		private EventAggregator _bus;
		private bool _aceControlIsOpen;

		public MainViewModel MainViewModel { get; set; }
		public TokensViewModel TokensViewModel { get; set; }

		#region Databound properties

		private IObservableCollection<Request> _history = new BindableCollection<Request>();
		public IObservableCollection<Request> History
		{
			get { return _history; }
			set
			{
				_history = value;
				NotifyOfPropertyChange(() => History);
			}
		}

		private Request _selectedHistory;
		public Request SelectedHistory
		{
			get { return _selectedHistory; }
			set
			{
				_selectedHistory = value;
				NotifyOfPropertyChange(() => SelectedHistory);
			}
		}

		private string _url;
		public string Url
		{
			get { return _url; }
			set
			{
				_url = value;
				NotifyOfPropertyChange(() => Url);
			}
		}

		private string _method;
		public string Method
		{
			get { return _method; }
			set
			{
				_method = value;
				NotifyOfPropertyChange(() => Method);
			}
		}

		private IObservableCollection<NamedAuthorizationState> _tokens = new BindableCollection<NamedAuthorizationState>();
		public IObservableCollection<NamedAuthorizationState> Tokens
		{
			get { return _tokens; }
			set
			{
				_tokens = value;
				NotifyOfPropertyChange(() => Tokens);
			}
		}

		private NamedAuthorizationState _selectedToken;
		public NamedAuthorizationState SelectedToken
		{
			get { return _selectedToken; }
			set
			{
				_selectedToken = value;
				NotifyOfPropertyChange(() => SelectedToken);
			}
		}

		private string _response;
		public string Response
		{
			get { return _response; }
			set
			{
				_response = value;
				NotifyOfPropertyChange(() => Response);
			}
		}

		#endregion

		public AppViewModel()
		{
			_bus = IoC.Get<EventAggregator>();
			_bus.Subscribe(this);

			MainViewModel = new MainViewModel(_bus);
			TokensViewModel = new TokensViewModel(_bus);

			LoadConfig(_bus);

			History.Add(new Request { Method = "GET", Url = "http://localhost:1444/services/onfarmautomation/v2/shed/1" });
			History.Add(new Request { Method = "POST", Url = "http://wah" });

			ActivateItem(MainViewModel);
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

		public void Handle(ShowTokensScreen message)
		{
			ActivateItem(TokensViewModel);
		}

		public void ManageTokens()
		{
			//AceControlIsOpen = true;
			//ActivateItem(TokensViewModel);
		}

		private bool ShouldSkipHistory(Request request)
		{
			if (!_history.Any())
				return false;

			var latestRequest = _history[0];

			return
				request.Url == latestRequest.Url
				&& request.Method == latestRequest.Method;
		}

		private void AddToHistory(Request request)
		{
			if (ShouldSkipHistory(request))
				return;

			History.Insert(0, request);
		}

		public void Send()
		{
			if (string.IsNullOrWhiteSpace(Url))
				return;

			var request = new Request
			{
				Url = Url,
				Method = Method,
				Token = SelectedToken.AuthorizationState.AccessToken
			};

			var client = new ApiClient();
			var response = client.Send(request);

			Response = System.Xml.Linq.XDocument.Parse(response.Content).ToString(); //JsonConvert.SerializeObject(response, Formatting.Indented);

			AddToHistory(request);
		}

		public void HistoryClicked(object e)
		{
			// Rebind.
			if (SelectedHistory != null)
				Url = SelectedHistory.Url;
		}
	}
}