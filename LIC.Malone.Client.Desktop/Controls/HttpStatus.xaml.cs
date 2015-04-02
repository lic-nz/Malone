using System.Net;
using System.Web;
using System.Windows;
using System.Windows.Media;
using LIC.Malone.Client.Desktop.Extensions;

namespace LIC.Malone.Client.Desktop.Controls
{
	// http://stackoverflow.com/questions/1364484/dependency-property-dependent-on-another

	public partial class HttpStatus
	{
		public static readonly DependencyProperty HttpStatusCodeProperty = DependencyProperty.Register("HttpStatusCode", typeof(HttpStatusCode?), typeof(HttpStatus), new PropertyMetadata(null, OnHttpStatusCodePropertyChanged));
		public HttpStatusCode? HttpStatusCode
		{
			get { return (HttpStatusCode?)GetValue(HttpStatusCodeProperty); }
			set { SetValue(HttpStatusCodeProperty, value); }
		}

		private static void OnHttpStatusCodePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var vm = (HttpStatus)d;
			vm.CoerceValue(ControlVisibilityProperty);
			vm.CoerceValue(CodeProperty);
			vm.CoerceValue(CodeColorProperty);
			vm.CoerceValue(DescriptionProperty);
		}

		private const Visibility DefaultVisibility = Visibility.Hidden;

		public static readonly DependencyProperty ControlVisibilityProperty = DependencyProperty.Register("ControlVisibility", typeof(Visibility), typeof(HttpStatus), new PropertyMetadata(DefaultVisibility, null, OnCoerceControlVisibilityProperty));
		public Visibility ControlVisibility
		{
			get { return (Visibility)GetValue(ControlVisibilityProperty); }
			set { SetValue(ControlVisibilityProperty, value); }
		}

		private static object OnCoerceControlVisibilityProperty(DependencyObject d, object baseValue)
		{
			var vm = (HttpStatus)d;
			return vm.HttpStatusCode == null
				? Visibility.Hidden
				: Visibility.Visible;
		}

		public static readonly DependencyProperty CodeProperty = DependencyProperty.Register("Code", typeof(string), typeof(HttpStatus), new PropertyMetadata("-", null, OnCoerceCodeProperty));
		public string Code
		{
			get { return (string)GetValue(CodeProperty); }
			set { SetValue(CodeProperty, value); }
		}

		private static object OnCoerceCodeProperty(DependencyObject d, object baseValue)
		{
			var vm = (HttpStatus)d;
			return vm.HttpStatusCode.HasValue
				? ((int)vm.HttpStatusCode).ToString()
				: string.Empty;
		}

		public static readonly DependencyProperty CodeColorProperty = DependencyProperty.Register("CodeColor", typeof(Brush), typeof(HttpStatus), new PropertyMetadata(Colors.Maybe, null, OnCoerceCodeColorProperty));
		public Brush CodeColor
		{
			get { return (Brush)GetValue(CodeColorProperty); }
			set { SetValue(CodeColorProperty, value); }
		}

		private static object OnCoerceCodeColorProperty(DependencyObject d, object baseValue)
		{
			var vm = (HttpStatus)d;
			return vm.HttpStatusCode.HasValue
				? vm.HttpStatusCode.Value.ToBrush()
				: Colors.Maybe;
		}

		public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register("Description", typeof(string), typeof(HttpStatus), new PropertyMetadata(string.Empty, null, OnCoerceDescriptionProperty));
		public string Description
		{
			get { return (string)GetValue(DescriptionProperty); }
			set { SetValue(DescriptionProperty, value); }
		}

		private static object OnCoerceDescriptionProperty(DependencyObject d, object baseValue)
		{
			var vm = (HttpStatus)d;
			return vm.HttpStatusCode.HasValue
				? HttpWorkerRequest.GetStatusDescription((int)vm.HttpStatusCode)
				: string.Empty;
		}

		public HttpStatus()
		{
			InitializeComponent();
		}
	}
}