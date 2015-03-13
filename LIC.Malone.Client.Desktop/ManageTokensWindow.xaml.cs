using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using DotNetOpenAuth.OAuth2;
using LIC.Malone.Core.Authentication;
using LIC.Malone.Core.Authentication.OAuth;
using Newtonsoft.Json;

namespace LIC.Malone.Client.Desktop
{
	public partial class ManageTokensWindow : Window
	{
		private readonly MainWindow _mainWindow;
		private IAuthorizationState _state;

		public ManageTokensWindow(MainWindow mainWindow, List<Uri> authenticationUrls, List<OAuthApplication> applications, UserCredentials userCredentials)
		{
			_mainWindow = mainWindow;
			InitializeComponent();

			Closing += OnClosing;

			AuthenticationUrls.ItemsSource = authenticationUrls;
			AuthenticationUrls.SelectedIndex = 0;
			
			Applications.ItemsSource = applications;
			Applications.SelectedIndex = 0;

			Username.Text = userCredentials.Username;
			Password.Text = userCredentials.Password;
		}

		private void OnClosing(object sender, CancelEventArgs cancelEventArgs)
		{
			_mainWindow.Show();
		}

		private void Authenticate_Click(object sender, RoutedEventArgs e)
		{
			var app = (OAuthApplication) Applications.SelectedItem;
			var url = (Uri) AuthenticationUrls.SelectedItem;

			var result = app.Authorize(url, Username.Text, Password.Text);

			if (result.HasError)
			{
				AuthResponse.Text = result.Error;
				_state = null;
				return;
			}

			AuthResponse.Text = JsonConvert.SerializeObject(result.AuthorizationState, Formatting.Indented);
			_state = result.AuthorizationState;
		}

		private void SaveToken_Click(object sender, RoutedEventArgs e)
		{
			if (string.IsNullOrWhiteSpace(TokenName.Text))
				return;

			var token = new NamedAuthorizationState(TokenName.Text, _state);
			
			_mainWindow.Tokens.Add(token);

			_mainWindow.TokensListBox.ItemsSource = null;
			_mainWindow.TokensListBox.ItemsSource = _mainWindow.Tokens;

			_mainWindow.TokenComboBox.ItemsSource = null;
			_mainWindow.TokenComboBox.ItemsSource = _mainWindow.Tokens;

			Tokens.ItemsSource = null;
			Tokens.ItemsSource = _mainWindow.Tokens;
		}
	}
}
