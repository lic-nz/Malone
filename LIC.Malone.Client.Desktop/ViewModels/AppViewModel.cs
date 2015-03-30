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
using ICSharpCode.AvalonEdit.Document;
using LIC.Malone.Client.Desktop.Messages;
using LIC.Malone.Core;
using LIC.Malone.Core.Authentication;
using LIC.Malone.Core.Authentication.OAuth;
using Newtonsoft.Json;
using RestSharp;

namespace LIC.Malone.Client.Desktop.ViewModels
{
	// TODO: Actually conduct the flyout.
	public class AppViewModel : Conductor<object>, IHandle<TokenAdded>
	{
		private readonly IWindowManager _windowManager;
		private readonly IEventAggregator _bus;
		private readonly DialogManager _dialogManager = new DialogManager();
		private readonly List<string> _allowedSchemes = new List<string> { Uri.UriSchemeHttp, Uri.UriSchemeHttps };

		private AddCollectionViewModel _addCollectionViewModel = new AddCollectionViewModel();
		private AddTokenViewModel _addTokenViewModel;

		private string _historyJsonPath;

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

		private IObservableCollection<NamedAuthorizationState> _tokens;
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

		private TextDocument _requestBody = new TextDocument();
		public TextDocument RequestBody
		{
			get { return _requestBody; }
			set
			{
				_requestBody = value;
				NotifyOfPropertyChange(() => RequestBody);
			}
		}

		private TextDocument _responseBody = new TextDocument();
		public TextDocument ResponseBody
		{
			get { return _responseBody; }
			set
			{
				_responseBody = value;
				NotifyOfPropertyChange(() => ResponseBody);
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
		
		public AppViewModel()
		{
			_windowManager = IoC.Get<WindowManager>();
			_bus = IoC.Get<EventAggregator>();
			_bus.Subscribe(this);

			_addTokenViewModel = new AddTokenViewModel(_bus);

			Tokens = new BindableCollection<NamedAuthorizationState>(new List<NamedAuthorizationState> { new NamedAuthorizationState("<Anonymous>", null)});
			SelectedToken = Tokens.First();

			LoadConfig();
		}

		private void LoadConfig()
		{
			var configLocation = ConfigurationManager.AppSettings["ConfigLocation"];

			// TODO: Handle the case of no config.
			if (string.IsNullOrWhiteSpace(configLocation))
				return;

			var applications = new List<OAuthApplication>();
			var authenticationUrls = new List<Uri>();
			var userCredentials = new UserCredentials
			{
				Username = string.Empty,
				Password = string.Empty
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

			_bus.PublishOnUIThread(new ConfigurationLoaded(applications, authenticationUrls, userCredentials));
		}

		public async void Send()
		{
			// Reset.
			ResponseBody = new TextDocument();
			HttpStatusCode = null;
			SelectedHistory = null;

			if (string.IsNullOrWhiteSpace(Url))
				return;

			var request = new Request(Url, SelectedMethod)
			{
				Body = RequestBody.Text
			};

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

			// Possibly detect response and format, e.g.:
			// XDocument.Parse(response.Content).ToString();
			// JsonConvert.SerializeObject(response, Formatting.Indented);
			ResponseBody = new TextDocument(response.Content);
			
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
			ResponseBody = new TextDocument(SelectedHistory.Response.Content);
			RequestBody = new TextDocument(SelectedHistory.Body);
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

		public void AddToken()
		{
			var settings = new Dictionary<string, object>
			{
				{"Title", "Add token"},
				{"WindowStartupLocation", WindowStartupLocation.CenterOwner}
			};

			_windowManager.ShowDialog(_addTokenViewModel, settings: settings);
			ActivateItem(_addTokenViewModel);
		}

		public void Handle(TokenAdded message)
		{
			Tokens.Add(message.NamedAuthorizationState);
			SelectedToken = Tokens.Last();
		}
	}
}