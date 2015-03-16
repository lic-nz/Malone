using System;
using System.Collections.Generic;
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

		public void Authenticate(object sender, RoutedEventArgs e)
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
	}
}
