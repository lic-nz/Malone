using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;
using Caliburn.Micro;
using DotNetOpenAuth.OAuth2;
using LIC.Malone.Client.Desktop.Messages;
using LIC.Malone.Core;
using LIC.Malone.Core.Authentication;
using LIC.Malone.Core.Authentication.OAuth;
using Newtonsoft.Json;
using RestSharp;

namespace LIC.Malone.Client.Desktop.ViewModels
{
	// TODO: Actually conduct the flyout.
	public class AppViewModel : Conductor<object>, IHandle<ConfigurationLoaded>
	{
		private readonly IWindowManager _windowManager;
		private readonly EventAggregator _bus;
		private readonly DialogManager _dialogManager = new DialogManager();
		private readonly List<string> _allowedSchemes = new List<string> { Uri.UriSchemeHttp, Uri.UriSchemeHttps };

		private AddCollectionViewModel _addCollectionViewModel = new AddCollectionViewModel();

		private string _historyJsonPath;

		private IAuthorizationState _authorizationState;

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

		private double _historyHorizontalOffset;
		public double HistoryHorizontalOffset
		{
			get { return _historyHorizontalOffset; }
			set
			{
				_historyHorizontalOffset = value;
				NotifyOfPropertyChange(() => HistoryHorizontalOffset);
			}
		}

		private double _historyVerticalScrollBarOffset;
		public double HistoryVerticalScrollBarOffset
		{
			get { return _historyVerticalScrollBarOffset; }
			set
			{
				_historyVerticalScrollBarOffset = value;
				NotifyOfPropertyChange(() => HistoryVerticalScrollBarOffset);
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
				NotifyOfPropertyChange(() => CanSend);
			}
		}

		private IEnumerable<Method> _methods = new List<Method>
		{
			Method.GET,
			Method.POST,
			Method.PUT
		};
		public IEnumerable<Method> Methods
		{
			get { return _methods; }
			set
			{
				_methods = value;
				NotifyOfPropertyChange(() => Methods);
			}
		}

		private Method _selectedMethod;
		public Method SelectedMethod
		{
			get { return _selectedMethod; }
			set
			{
				_selectedMethod = value;
				NotifyOfPropertyChange(() => SelectedMethod);
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

		private HttpStatusCodeViewModel _httpStatusCode;
		public HttpStatusCodeViewModel HttpStatusCode
		{
			get { return _httpStatusCode; }
			set
			{
				_httpStatusCode = value;
				NotifyOfPropertyChange(() => HttpStatusCode);
			}
		}

		private string _responseContent;
		public string ResponseContent
		{
			get { return _responseContent; }
			set
			{
				_responseContent = value;
				NotifyOfPropertyChange(() => ResponseContent);
			}
		}

		private bool _showAddTokenFlyout;
		public bool ShowAddTokenFlyout
		{
			get { return _showAddTokenFlyout; }
			set
			{
				_showAddTokenFlyout = value;
				NotifyOfPropertyChange(() => ShowAddTokenFlyout);
			}
		}

		public bool CanSend
		{
			get
			{
				Uri url;
				return Uri.TryCreate(Url, UriKind.Absolute, out url) && _allowedSchemes.Contains(url.Scheme);
			}
		}

		#endregion
		
		// TODO: Move these to view model for flyout.
		#region Databound properties for flyout

		private IObservableCollection<Uri> _authenticationUrls = new BindableCollection<Uri>();
		public IObservableCollection<Uri> AuthenticationUrls
		{
			get { return _authenticationUrls; }
			set
			{
				_authenticationUrls = value;
				NotifyOfPropertyChange(() => AuthenticationUrls);
			}
		}

		private Uri _selectedAuthenticationUrl;
		public Uri SelectedAuthenticationUrl
		{
			get { return _selectedAuthenticationUrl; }
			set
			{
				_selectedAuthenticationUrl = value;
				NotifyOfPropertyChange(() => SelectedAuthenticationUrl);
			}

		}

		private IObservableCollection<OAuthApplication> _applications = new BindableCollection<OAuthApplication>();
		public IObservableCollection<OAuthApplication> Applications
		{
			get { return _applications; }
			set
			{
				_applications = value;
				NotifyOfPropertyChange(() => Applications);
			}
		}

		private OAuthApplication _selectedApplication;
		public OAuthApplication SelectedApplication
		{
			get { return _selectedApplication; }
			set
			{
				_selectedApplication = value;
				NotifyOfPropertyChange(() => SelectedApplication);
			}
		}

		private string _username;
		public string Username
		{
			get { return _username; }
			set
			{
				_username = value;
				NotifyOfPropertyChange(() => Username);
			}
		}

		private string _password;
		public string Password
		{
			get { return _password; }
			set
			{
				_password = value;
				NotifyOfPropertyChange(() => Password);
			}
		}

		private string _authResponse;
		public string AuthResponse
		{
			get { return _authResponse; }
			set
			{
				_authResponse = value;
				NotifyOfPropertyChange(() => AuthResponse);
			}
		}

		private string _tokenName;
		public string TokenName
		{
			get { return _tokenName; }
			set
			{
				_tokenName = value;
				NotifyOfPropertyChange(() => TokenName);
			}
		}

		//private IObservableCollection<NamedAuthorizationState> _tokens = new BindableCollection<NamedAuthorizationState>();
		//public IObservableCollection<NamedAuthorizationState> Tokens
		//{
		//	get { return _tokens; }
		//	set
		//	{
		//		_tokens = value;
		//		NotifyOfPropertyChange(() => Tokens);
		//	}
		//}

		#endregion

		public AppViewModel()
		{
			_windowManager = IoC.Get<WindowManager>();
			_bus = IoC.Get<EventAggregator>();
			_bus.Subscribe(this);

			LoadConfig(_bus);
		}

		private void LoadConfig(EventAggregator bus)
		{
			var configLocation = ConfigurationManager.AppSettings["ConfigLocation"];

			// TODO: Handle the case of no config.
			if (string.IsNullOrWhiteSpace(configLocation))
				return;

			var applications = new List<OAuthApplication>();
			var authenticationUrls = new List<Uri>();
			var userCredentials = new UserCredentials
			{
				Username = "Username",
				Password = "Password"
			};

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

			_historyJsonPath = Path.Combine(configLocation, "history.json");

			if (File.Exists(_historyJsonPath))
			{
				var json = File.ReadAllText(_historyJsonPath);

				if (json.Any())
				{
					var history = JsonConvert.DeserializeObject<List<Request>>(json);
					History.AddRange(history);
				}
			}

			bus.PublishOnUIThread(new ConfigurationLoaded(applications, authenticationUrls, userCredentials));
		}

		public void ManageTokens()
		{
			ShowAddTokenFlyout = true;
		}

		public async void Send()
		{
			// Reset.
			ResponseContent = null;
			HttpStatusCode = null;
			SelectedHistory = null;

			if (string.IsNullOrWhiteSpace(Url))
				return;

			var request = new Request(Url, SelectedMethod);

			if (SelectedToken != null)
				request.NamedAuthorizationState = SelectedToken;

			var client = new ApiClient();
			var result = client.Send(request);
			var response = result.Response;

			var responseError = GetResponseError(response);

			if (responseError != null)
			{
				var dialogResult = await _dialogManager.Show("Oh dear", "I'll be honest with you: we've hit a snag. Not sure exactly what the problem is but I suggest you've got the URL wrong or forgotten to plug in your Internet. Double check those things and we'll have another go.\n\nBTW, the low level reponse was: " + responseError);
				return;
			}

			request.At = result.SentAt;
			request.Response = new Response
			{
				Guid = Guid.NewGuid(),
				At = result.ReceivedAt,
				HttpStatusCode = response.StatusCode,
				Content = response.Content
			};

			HttpStatusCode = new HttpStatusCodeViewModel(response.StatusCode);
			ResponseContent = response.Content;
			//ResponseContent = XDocument.Parse(response.Content).ToString(); //JsonConvert.SerializeObject(response, Formatting.Indented);
			
			AddToHistory(request);
		}

		private void AddToHistory(Request request)
		{
			History.Insert(0, request);
			NotifyOfPropertyChange(() => History);
			SelectedHistory = History.First();
			SaveHistory();
		}

		private string GetResponseError(IRestResponse response)
		{
			if (response == null)
				return "Uhm, response was null?";
			
			switch (response.ResponseStatus)
			{
				case ResponseStatus.None:
					return "Uh, not sure what happened. Didn't get a response?";
				case ResponseStatus.Completed:
					return null;
				case ResponseStatus.Error:
					return "Error. Network might be down, DNS failed, or sunspots messed up the signal.";
				case ResponseStatus.TimedOut:
					return "Timed out.";
				case ResponseStatus.Aborted:
					return "Aborted.";
				default:
					return null;
			}
		}

		public void HistoryLayoutUpdated(object e)
		{
			var listBox = (MaloneListBox) ((ActionExecutionContext) e).Source;

			HistoryHorizontalOffset = listBox.ScrollViewerHorizontalOffset;
			HistoryVerticalScrollBarOffset = listBox.VerticalScrollBarVisibility == Visibility.Visible
				? -14
				: 0;
		}

		public void HistoryClicked(object e)
		{
			if (SelectedHistory == null)
				return;

			// Rebind.
			Url = SelectedHistory.Url;
			SelectedMethod = SelectedHistory.Method;
			HttpStatusCode = new HttpStatusCodeViewModel(SelectedHistory.Response.HttpStatusCode);
			ResponseContent = SelectedHistory.Response.Content;
		}

		public void RemoveFromHistory(object e)
		{
			var request = (Request) e;
			History.Remove(request);
			SaveHistory();
		}

		public void ClearHistory(object e)
		{
			History = new BindableCollection<Request>();
			SaveHistory();
		}

		private void SaveHistory()
		{
			var json = JsonConvert.SerializeObject(History, Formatting.Indented);
			File.WriteAllText(_historyJsonPath, json);
		}

		public void AddCollection()
		{
			var settings = new Dictionary<string, object>
			{
				{"Title", "Add collection"},
				{"WindowStartupLocation", WindowStartupLocation.CenterOwner}
			};

			_windowManager.ShowDialog(_addCollectionViewModel, settings: settings);
			ActivateItem(_addCollectionViewModel);
		}

		// TODO: Move to own view model.
		#region Add token flyout methods
		public void Handle(ConfigurationLoaded message)
		{
			AuthenticationUrls = new BindableCollection<Uri>(message.AuthenticationUrls);
			SelectedAuthenticationUrl = AuthenticationUrls.FirstOrDefault();

			Applications = new BindableCollection<OAuthApplication>(message.Applications);
			SelectedApplication = Applications.FirstOrDefault();

			Username = message.UserCredentials.Username;
			Password = message.UserCredentials.Password;
		}

		public void Authenticate()
		{
			var app = SelectedApplication;
			var url = SelectedAuthenticationUrl;

			var result = app.Authorize(url, Username, Password);

			if (result.HasError)
			{
				ResponseContent = result.Error;
				_authorizationState = null;
				return;
			}

			AuthResponse = JsonConvert.SerializeObject(result.AuthorizationState, Formatting.Indented);
			_authorizationState = result.AuthorizationState;
		}

		public void SaveToken()
		{
			if (string.IsNullOrWhiteSpace(TokenName))
				return;

			var token = new NamedAuthorizationState(TokenName, _authorizationState);

			Tokens.Add(token);
			SelectedToken = SelectedToken ?? Tokens.First();

			ShowAddTokenFlyout = false;

			// Reset.
			_authorizationState = null;
			AuthResponse = null;
			TokenName = null;
		}
		#endregion
	}
}