using System;
using System.Globalization;
using System.Net;
using System.Windows;
using System.Windows.Data;
using LIC.Malone.Client.Desktop.Extensions;

namespace LIC.Malone.Client.Desktop.Converters
{
	public class HttpStatusCodeToBrushConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var code = (HttpStatusCode) value;
			return code.ToBrush();
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}

	public class HttpStatusCodeToIntConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return (int) value;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}
}