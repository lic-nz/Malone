using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using LIC.Malone.Client.Desktop.Messages;
using LIC.Malone.Core;
using LIC.Malone.Core.Authentication.OAuth;

namespace LIC.Malone.Client.Desktop.ViewModels
{
	public class MainViewModel : Screen
	{
		private IEventAggregator _bus;

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

		public MainViewModel(IEventAggregator bus)
		{
			_bus = bus;
			_bus.Subscribe(this);

			History.Add(new Request { Method = "GET", Url = "http://localhost:1444/services/onfarmautomation/v2/shed/1" });
			History.Add(new Request { Method = "GET", Url = "http://wah" });
		}

		public void ManageTokens()
		{
			_bus.BeginPublishOnUIThread(new ShowTokensScreen());
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
			Url = SelectedHistory.Url;
		}
	}
}