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
using LIC.Malone.Core.Authentication.OAuth;
using Newtonsoft.Json;

namespace LIC.Malone.Client.Desktop
{
	public partial class ManageTokensWindow : Window
	{
		public ManageTokensWindow(List<Uri> authenticationUrls, List<OAuthApplication> applications)
		{
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
		}
	}
}
