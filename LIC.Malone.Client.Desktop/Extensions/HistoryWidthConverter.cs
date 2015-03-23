using System;
using System.Globalization;
using System.Net;
using System.Windows.Data;

namespace LIC.Malone.Client.Desktop.Extensions
{
	public class HistoryWidthConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return -((double) value / 2);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}
}