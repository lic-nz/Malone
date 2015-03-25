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
			const double buttonWidth = 53;

			var historyWidth = (double)values[0];
			var horizontalOffset = (double)values[1];
			var historyVerticalScrollBarOffset = (double)values[2];

			return historyWidth + horizontalOffset + historyVerticalScrollBarOffset - buttonWidth;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}
}