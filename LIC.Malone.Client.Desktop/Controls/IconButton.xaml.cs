using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace LIC.Malone.Client.Desktop.Controls
{
	public partial class IconButton : Button
	{
		public static readonly DependencyProperty IconDataProperty = DependencyProperty.Register("IconData", typeof(Geometry), typeof(IconButton), new PropertyMetadata());
		public Geometry IconData
		{
			get { return (Geometry)GetValue(IconDataProperty); }
			set { SetValue(IconDataProperty, value); }
		}

		public static readonly DependencyProperty IconMarginProperty = DependencyProperty.Register("IconMargin", typeof(Thickness), typeof(IconButton), new PropertyMetadata());
		public Thickness IconMargin
		{
			get { return (Thickness)GetValue(IconMarginProperty); }
			set { SetValue(IconMarginProperty, value); }
		}

		public IconButton()
		{
			InitializeComponent();
		}
	}
}