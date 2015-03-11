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
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public List<Request> History { get; set; }

		public MainWindow()
		{
			InitializeComponent();

			History = new List<Request>
			{
				new Request { Method = "GET", Url = "http://google.co.nz" },
				new Request { Method = "POST", Url = "http://lic.co.nz" },
			};

			HistoryListBox.ItemsSource = History;
		}

		private void History_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var request = ((ListBox)sender).SelectedItem as Request;

			if (request == null)
				return;

			Url.Text = request.Url;
		}
	}
}