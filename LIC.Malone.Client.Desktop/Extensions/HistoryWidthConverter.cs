using System;
using System.Globalization;
using System.Net;
using System.Windows.Data;

namespace LIC.Malone.Client.Desktop.Extensions
{
	public class HistoryWidthConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			const double buttonWidth = 51;

			var historyWidth = (double)values[0];
			var horizontalOffset = (double)values[1];

			return historyWidth + horizontalOffset - buttonWidth;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}
}