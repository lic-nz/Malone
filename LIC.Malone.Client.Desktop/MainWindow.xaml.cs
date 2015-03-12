using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using LIC.Malone.Core;
using LIC.Malone.Core.Authentication.OAuth;
using Newtonsoft.Json;
using Path = System.IO.Path;

namespace LIC.Malone.Client.Desktop
{
	public partial class MainWindow : Window
	{
		private List<Request> _history;
		private List<OAuthApplication> _applications;
		private List<Uri> _authenticationUrls;

		public MainWindow()
		{
			InitializeComponent();

			LoadConfig();

			_history = new List<Request>
			{
				new Request { Method = "GET", Url = "http://localhost:1444/services/onfarmautomation/v2/shed/1" }
			};

			History.ItemsSource = _history;

			ManageTokens_Click(null, null);
		}

		private void LoadConfig()
		{
			_applications = new List<OAuthApplication>();

			var configLocation = ConfigurationManager.AppSettings["ConfigLocation"];

			if (string.IsNullOrWhiteSpace(configLocation))
				return;

			var applicationsJsonPath = Path.Combine(configLocation, "oauth-applications.json");

			if (File.Exists(applicationsJsonPath))
				_applications = JsonConvert.DeserializeObject<List<OAuthApplication>>(File.ReadAllText(applicationsJsonPath));

			var authenticationUrlsJsonPath = Path.Combine(configLocation, "oauth-authentication-urls.json");

			if (File.Exists(authenticationUrlsJsonPath))
				_authenticationUrls = JsonConvert
					.DeserializeObject<List<string>>(File.ReadAllText(authenticationUrlsJsonPath))
					.Select(url => new Uri(url))
					.ToList();
		}

		private void History_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var request = ((ListBox)sender).SelectedItem as Request;

			if (request == null)
				return;

			Url.Text = request.Url;
			Method.SelectedValue = request.Method;

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

			_history.Insert(0, request);

			History.ItemsSource = null;
			History.ItemsSource = _history;
		}

		private void Send_Click(object sender, RoutedEventArgs e)
		{
			if (string.IsNullOrWhiteSpace(Url.Text))
				return;

			var request = new Request
			{
				Url = Url.Text,
				Method = Method.SelectedValue.ToString(),
				Token = Token.Text
			};

			var client = new ApiClient();
			var response = client.Send(request);

			Response.Text = response;

			AddToHistory(request);
		}

		private void ManageTokens_Click(object sender, RoutedEventArgs e)
		{
			Hide();
			new ManageTokensWindow(_authenticationUrls, _applications).Show();
		}
	}
}