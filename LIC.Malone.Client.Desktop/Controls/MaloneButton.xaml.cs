using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace LIC.Malone.Client.Desktop.Controls
{
	public partial class MaloneButton : Button
	{
		public static readonly DependencyProperty IconDataProperty = DependencyProperty.Register("IconData", typeof(Geometry), typeof(MaloneButton), new PropertyMetadata());
		public Geometry IconData
		{
			get { return (Geometry)GetValue(IconDataProperty); }
			set { SetValue(IconDataProperty, value); }
		}

		public static readonly DependencyProperty IconTopProperty = DependencyProperty.Register("IconTop", typeof(double), typeof(MaloneButton), new PropertyMetadata((double)-1));
		public double IconTop
		{
			get { return (double)GetValue(IconTopProperty); }
			set { SetValue(IconTopProperty, value); }
		}

		public static readonly DependencyProperty TextMarginProperty = DependencyProperty.Register("TextMargin", typeof(Thickness), typeof(MaloneButton), new PropertyMetadata(new Thickness(20, 0, 0, 0)));
		public Thickness TextMargin
		{
			get { return (Thickness)GetValue(TextMarginProperty); }
			set { SetValue(TextMarginProperty, value); }
		}

		public MaloneButton()
		{
			InitializeComponent();
		}
	}
}