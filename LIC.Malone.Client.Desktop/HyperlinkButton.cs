using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace LIC.Malone.Client.Desktop
{
	public class HyperlinkButton : DependencyObject
	{
		#region IsCheckedOnDataProperty

		public static readonly DependencyProperty IconProperty;

		public static void SetIcon(DependencyObject obj, PathGeometry value)
		{
			obj.SetValue(IconProperty, value);
		}

		public static PathGeometry GetIcon(DependencyObject obj)
		{
			return (PathGeometry)obj.GetValue(IconProperty);
		}

		#endregion

		static HyperlinkButton()
		{
			IconProperty = DependencyProperty.RegisterAttached("Icon",
				typeof(PathGeometry),
				typeof(HyperlinkButton),
				new FrameworkPropertyMetadata(null));
		}
	}
}