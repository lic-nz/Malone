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

		public static readonly DependencyProperty HoverColorProperty = DependencyProperty.Register("HoverColor", typeof(SolidColorBrush), typeof(IconLink), new PropertyMetadata());
		public SolidColorBrush HoverColor
		{
			get { return (SolidColorBrush)GetValue(HoverColorProperty); }
			set { SetValue(HoverColorProperty, value); }
		}

		public static readonly DependencyProperty BackgroundColorProperty = DependencyProperty.Register("BackgroundColor", typeof(Brush), typeof(IconLink), new PropertyMetadata());
		public Brush BackgroundColor
		{
			get { return (Brush)GetValue(BackgroundColorProperty); }
			set { SetValue(BackgroundColorProperty, value); }
		}

		public IconLink()
		{
			InitializeComponent();
		}
	}
}