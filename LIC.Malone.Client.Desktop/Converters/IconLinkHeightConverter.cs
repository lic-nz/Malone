using System;
using System.Globalization;
using System.Net;
using System.Windows;
using System.Windows.Data;
using LIC.Malone.Client.Desktop.Extensions;

namespace LIC.Malone.Client.Desktop.Converters
{
	public class IconLinkHeightConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var height = (double) value;
			return double.IsNaN(height)
				? 30d
				: height;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}
}