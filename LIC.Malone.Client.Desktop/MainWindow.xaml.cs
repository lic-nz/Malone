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
using System.Windows.Navigation;
using System.Windows.Shapes;
using LIC.Malone.Core;

namespace LIC.Malone.Client.Desktop
{
	public partial class MainWindow : Window
	{
		private List<Request> _history { get; set; }

		public MainWindow()
		{
			InitializeComponent();

			_history = new List<Request>
			{
				new Request { Method = "GET", Url = "http://localhost:1444/services/onfarmautomation/v2/shed/1" }
			};

			History.ItemsSource = _history;
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
	}
}