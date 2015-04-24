using System;
using System.Globalization;
using System.Windows.Data;

namespace LIC.Malone.Client.Desktop.Converters
{
	public class HistoryWidthConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			const double buttonWidth = 40;
			const double padding = 14;
			const double borderPadding = 1;

			var historyWidth = (double)values[0];
			var horizontalOffset = (double)values[1];
			var historyVerticalScrollBarOffset = (double)values[2];
			var source = (string)values[3];
			var isBorder = source == "Border";

			var width = historyWidth + horizontalOffset + historyVerticalScrollBarOffset - buttonWidth - padding;

			if (isBorder)
				width -= borderPadding;

			return width;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}
}