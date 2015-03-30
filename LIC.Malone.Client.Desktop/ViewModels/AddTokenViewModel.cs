using System;
using System.Linq;
using System.Windows;
using Caliburn.Micro;
using DotNetOpenAuth.OAuth2;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using LIC.Malone.Client.Desktop.Messages;
using LIC.Malone.Core.Authentication.OAuth;
using Newtonsoft.Json;

namespace LIC.Malone.Client.Desktop.ViewModels
{
	public class AddTokenViewModel : Screen, IHandle<ConfigurationLoaded>
	{
		private IAuthorizationState _authorizationState;

		private readonly IEventAggregator _bus;

		#region Databound properties

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
				NotifyOfPropertyChange(() => CanAuthenticate);
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
				NotifyOfPropertyChange(() => CanAuthenticate);
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
				NotifyOfPropertyChange(() => CanAuthenticate);
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
				NotifyOfPropertyChange(() => CanAuthenticate);
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
				NotifyOfPropertyChange(() => CanSaveToken);
			}
		}

		private TextDocument _response = new TextDocument();
		public TextDocument Response
		{
			get { return _response; }
			set
			{
				_response = value;
				NotifyOfPropertyChange(() => Response);
				NotifyOfPropertyChange(() => CanSaveToken);
			}
		}

		public void ResponseChanged(ActionExecutionContext context)
		{
			var source = (TextEditor) context.Source;
			Response = source.Document;
		}

		public bool CanSaveToken
		{
			get { return !string.IsNullOrWhiteSpace(Response.Text) && !string.IsNullOrWhiteSpace(TokenName); }
		}

		public bool CanAuthenticate
		{
			get { return SelectedApplication != null && SelectedAuthenticationUrl != null && !string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Password); }
		}

		#endregion

		public AddTokenViewModel(IEventAggregator bus)
		{
			_bus = bus;
			_bus.Subscribe(this);
		}

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
			Reset();

			var app = SelectedApplication;
			var url = SelectedAuthenticationUrl;

			var result = app.Authorize(url, Username, Password);

			if (result.HasError)
			{
				Response.Text = result.Error;
				return;
			}

			Response.Text = JsonConvert.SerializeObject(result.AuthorizationState, Formatting.Indented);
			_authorizationState = result.AuthorizationState;
		}

		public void SaveToken()
		{
			// TODO: verify if this is needed.
			Reset();

			var token = new NamedAuthorizationState(TokenName, _authorizationState);

			var message = new TokenAdded
			{
				NamedAuthorizationState = token
			};

			_bus.PublishOnUIThread(message);

		}

		private void Reset()
		{
			_authorizationState = null;
			Response.Text = string.Empty;
			TokenName = null;
		}
	}
}