using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace LIC.Malone.Client.Desktop.Controls
{
	public partial class IconLink : Button
	{
		public static readonly DependencyProperty IconDataProperty = DependencyProperty.Register("IconData", typeof(Geometry), typeof(IconLink), new PropertyMetadata());
		public Geometry IconData
		{
			get { return (Geometry)GetValue(IconDataProperty); }
			set { SetValue(IconDataProperty, value); }
		}

		public static readonly DependencyProperty IconTopProperty = DependencyProperty.Register("IconTop", typeof(double), typeof(IconLink), new PropertyMetadata((double)-1));
		public double IconTop
		{
			get { return (double)GetValue(IconTopProperty); }
			set { SetValue(IconTopProperty, value); }
		}

		public static readonly DependencyProperty TextMarginProperty = DependencyProperty.Register("TextMargin", typeof(Thickness), typeof(IconLink), new PropertyMetadata(new Thickness(-4, -4, 0, 0)));
		public Thickness TextMargin
		{
			get { return (Thickness)GetValue(TextMarginProperty); }
			set { SetValue(TextMarginProperty, value); }
		}

		public IconLink()
		{
			InitializeComponent();
		}
	}
}