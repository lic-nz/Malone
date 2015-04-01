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

		public static readonly DependencyProperty IconMarginProperty = DependencyProperty.Register("IconMargin", typeof(Thickness), typeof(IconLink), new PropertyMetadata());
		public Thickness IconMargin
		{
			get { return (Thickness)GetValue(IconMarginProperty); }
			set { SetValue(IconMarginProperty, value); }
		}

		public IconLink()
		{
			InitializeComponent();
		}
	}
}