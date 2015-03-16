using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using DotNetOpenAuth.OAuth2;
using LIC.Malone.Client.Desktop.Messages;
using LIC.Malone.Core.Authentication.OAuth;
using Newtonsoft.Json;

namespace LIC.Malone.Client.Desktop.ViewModels
{
	public class TokensViewModel : Screen, IHandle<ConfigurationLoaded>
	{
		private IEventAggregator _bus;
		private IAuthorizationState _authorizationState;

		#region Databound properties

		private List<Uri> _authenticationUrls;
		public List<Uri> AuthenticationUrls
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

		private List<OAuthApplication> _applications;
		public List<OAuthApplication> Applications
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

		#endregion

		public TokensViewModel(IEventAggregator bus)
		{
			_bus = bus;
			_bus.Subscribe(this);
		}

		public void Handle(ConfigurationLoaded message)
		{
			AuthenticationUrls = message.AuthenticationUrls;
			SelectedAuthenticationUrl = AuthenticationUrls.First();

			Applications = message.Applications;
			SelectedApplication = Applications.First();

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
				Response = result.Error;
				_authorizationState = null;
				return;
			}

			Response = JsonConvert.SerializeObject(result.AuthorizationState, Formatting.Indented);
			_authorizationState = result.AuthorizationState;
		}

		public void SaveToken()
		{
			if (string.IsNullOrWhiteSpace(TokenName))
				return;

			var token = new NamedAuthorizationState(TokenName, _authorizationState);

			Tokens.Add(token);
		}

		public void Back()
		{
			_bus.BeginPublishOnUIThread(new ShowMainScreen());
		}
	}
}
