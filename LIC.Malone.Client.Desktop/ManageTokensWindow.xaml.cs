using System;
using System.Collections.Generic;
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
using LIC.Malone.Core.Authentication.OAuth;
using Newtonsoft.Json;

namespace LIC.Malone.Client.Desktop
{
	public partial class ManageTokensWindow : Window
	{
		private readonly MainWindow _mainWindow;
		private IAuthorizationState _state;

		public ManageTokensWindow(MainWindow mainWindow, List<Uri> authenticationUrls, List<OAuthApplication> applications)
		{
			_mainWindow = mainWindow;
			InitializeComponent();

			AuthenticationUrls.ItemsSource = authenticationUrls;
			AuthenticationUrls.SelectedIndex = 0;
			
			Applications.ItemsSource = applications;
			Applications.SelectedIndex = 0;
		}

		private void Authenticate_Click(object sender, RoutedEventArgs e)
		{
			var app = (OAuthApplication) Applications.SelectedItem;
			var url = (Uri) AuthenticationUrls.SelectedItem;

			var result = app.Authorize(url, Username.Text, Password.Text);

			AuthResponse.Text = result.HasError
				? result.Error
				: JsonConvert.SerializeObject(result.AuthorizationState, Formatting.Indented);

			_state = result.AuthorizationState;
		}

		private void SaveToken_Click(object sender, RoutedEventArgs e)
		{
			_mainWindow.Tokens.Add(_state);
			_mainWindow.TokensListBox.ItemsSource = new List<IAuthorizationState> { _state };
			Close();
			_mainWindow.Show();
		}
	}
}
