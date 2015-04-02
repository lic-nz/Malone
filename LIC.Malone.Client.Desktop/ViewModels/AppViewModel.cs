using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows;
using System.Xml.Linq;
using Caliburn.Micro;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
using LIC.Malone.Client.Desktop.Controls;
using LIC.Malone.Client.Desktop.Messages;
using LIC.Malone.Core;
using LIC.Malone.Core.Authentication;
using LIC.Malone.Core.Authentication.OAuth;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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

		private static IEnumerable<string> _accepts = new List<string>
		{
			"text/xml",
			"application/json"
		};
		
		public IEnumerable<string> Accepts
		{
			get { return _accepts; }
			set
			{
				_accepts = value;
				NotifyOfPropertyChange(() => Accepts);
			}
		}

		private string _selectedAccept;
		public string SelectedAccept
		{
			get { return _selectedAccept; }
			set
			{
				_selectedAccept = value;

				// Must be a better way to do this.
				var headers = new BindableCollection<Header>(Headers);
				var header = headers.Single(h => h.Name == "Accept");
				header.Value = _selectedAccept;
				Headers = headers;

				NotifyOfPropertyChange(() => SelectedAccept);
			}
		}

		private IObservableCollection<Header> _headers = new BindableCollection<Header>(new List<Header> { new Header("Accept", _accepts.First()) });
		public IObservableCollection<Header> Headers
		{
			get { return _headers; }
			set
			{
				_headers = value;
				NotifyOfPropertyChange(() => Headers);
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
				NotifyOfPropertyChange(() => SelectedTokenJson);
			}
		}

		private TextDocument _selectedTokenJson = new TextDocument();
		public TextDocument SelectedTokenJson
		{
			get
			{
				if (SelectedToken == null)
					_selectedTokenJson.Text = string.Empty;
				else if (SelectedToken.AuthorizationState == null)
					_selectedTokenJson.Text = string.Empty;
				else
					_selectedTokenJson.Text = JsonConvert.SerializeObject(SelectedToken.AuthorizationState, Formatting.Indented); ;

				return _selectedTokenJson;
			}
		}

		private HttpStatusCode? _httpStatusCode;
		public HttpStatusCode? HttpStatusCode
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
			get
			{
				_responseBody.Text = Prettify(_responseBody.Text, ResponseContentType);
				return _responseBody;
			}
			set
			{
				_responseBody.Text = Prettify(_responseBody.Text, ResponseContentType);
				_responseBody = value;
				NotifyOfPropertyChange(() => ResponseBody);
			}
		}

		private string _responseContentType;
		public string ResponseContentType
		{
			get { return _responseContentType; }
			set
			{
				_responseContentType = value;
				NotifyOfPropertyChange(() => ResponseContentType);
				NotifyOfPropertyChange(() => ResponseBodyHighlighting);
			}
		}

		public IHighlightingDefinition ResponseBodyHighlighting
		{
			get
			{
				return GetHighlightingForContentType(ResponseContentType);
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

			_addTokenViewModel = new AddTokenViewModel(_bus, _windowManager);

			SelectedAccept = Accepts.First();
			Tokens = new BindableCollection<NamedAuthorizationState>(new List<NamedAuthorizationState> { new NamedAuthorizationState("<Anonymous>", null)});
			SelectedToken = Tokens.First();
			HttpStatusCode = null;

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
				Body = RequestBody.Text,
				Headers = Headers.ToList()
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
				Content = response.Content,
				ContentType = response.ContentType
			};

			HttpStatusCode = response.StatusCode;
			ResponseContentType = response.ContentType;

			// Possibly detect response and format, e.g.:
			// XDocument.Parse(response.Content).ToString();
			// JsonConvert.SerializeObject(response, Formatting.Indented);
			ResponseBody = new TextDocument(response.Content);
			
			AddToHistory(request);
		}

		private enum ContentType
		{
			Unknown,
			Xml,
			Json
		}

		private ContentType GetContentType(string contentType)
		{
			if (contentType == null)
				return ContentType.Unknown;

			var parts = contentType
				   .Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries)
				   .Select(s => s.Trim())
				   .ToList();

			if (parts.Contains("application/xml") || parts.Contains("text/xml"))
				return ContentType.Xml;

			if (parts.Contains("application/json"))
				return ContentType.Json;

			return ContentType.Unknown;
		}

		private IHighlightingDefinition GetHighlightingForContentType(string contentType)
		{
			return GetHighlightingForContentType(GetContentType(contentType));
		}

		private IHighlightingDefinition GetHighlightingForContentType(ContentType contentType)
		{
			var manager = HighlightingManager.Instance;
			
			switch (contentType)
			{
				case ContentType.Xml:
					return manager.GetDefinition("XML");

				case ContentType.Json:
					return manager.GetDefinition("JavaScript");

				default:
					return null; 
			}
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
			RequestBody = new TextDocument(SelectedHistory.Body);

			HttpStatusCode = SelectedHistory.Response.HttpStatusCode;
			ResponseContentType = SelectedHistory.Response.ContentType;
			ResponseBody = new TextDocument(SelectedHistory.Response.Content);

			SelectedToken = Tokens.First();
			var historicalTokens = Tokens.Where(t => t.NamedAuthorizationStateOrigin == NamedAuthorizationStateOrigin.History).ToList();
			Tokens.RemoveRange(historicalTokens);

			var state = SelectedHistory.NamedAuthorizationState;

			if (state != null)
			{
				Tokens.Add(SelectedHistory.NamedAuthorizationState);
				SelectedToken = SelectedHistory.NamedAuthorizationState;
			}
		}

		public void RemoveFromHistory(object e)
		{
			var request = (Request) e;
			History.Remove(request);
			SaveHistory();
		}

		public void RemoveFromHeaders(object e)
		{
			var header = (Header) e;
			Headers.Remove(header);
		}

		public void AddHeader()
		{
			Headers.Add(new Header());
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
				{"WindowStartupLocation", WindowStartupLocation.CenterOwner}
			};

			_windowManager.ShowDialog(_addTokenViewModel, settings: settings);
			ActivateItem(_addTokenViewModel);
		}

		public void Reset()
		{
			SelectedHistory = null;

			// TODO: Bind all properties to a Request object.
			SelectedMethod = Methods.First();
			Url = null;
			SelectedAccept = Accepts.First();
			RequestBody.Text = string.Empty;
			ResponseBody.Text = string.Empty;
			ResponseContentType = null;
			HttpStatusCode = null;
		}

		public void Handle(TokenAdded message)
		{
			Tokens.Add(message.NamedAuthorizationState);
			SelectedToken = Tokens.Last();
		}

		private string Prettify(string content, string contentType)
		{
			content = content ?? string.Empty;

			if (string.IsNullOrWhiteSpace(content))
				return content;

			var type = GetContentType(contentType);

			if (type == ContentType.Xml)
			{
				try
				{
					return XDocument.Parse(content).ToString();
				}
				catch
				{
					return content;
				}
			}

			if (type == ContentType.Json)
			{
				try
				{
					var json = JObject.Parse(content);
					return json.ToString(Formatting.Indented);
				}
				catch
				{
					return content;
				}
			}

			// For ContentType.Unknown, the parameter "content" is returned as-is for the out parameter "indentedContent".
			return content;
		}

		public void WindowResized(ActionExecutionContext context)
		{
			// TODO: bug where you maximize, reduce history column to min-width, then un-maximize: the request column will be off the window.
			//var window = context.Source as Window;

			//if (window == null)
			//	return;

			//var grid = window.FindName("MainGrid") as Grid;
			//var historyColumn = window.FindName("HistoryColumn") as ColumnDefinition;
			////var requestColumn = window.FindName("HistoryColumn") as ColumnDefinition;

			//if (grid == null || historyColumn == null)
			//	return;

			//var ratio = grid.ActualWidth/historyColumn.ActualWidth;
			//var width = new GridLength(window.ActualWidth/3);

			//if (ratio > 3)
			//	historyColumn.Width = width;
		}
	}
}