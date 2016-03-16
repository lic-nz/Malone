using System;
using System.Globalization;
using System.Windows.Data;

namespace LIC.Malone.Client.Desktop.Converters
{
	public class HistoryGroupConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var date = DateTime.Parse(value.ToString());

			var yesterday = DateTime.Today.AddDays(-1);
			var previousMonday = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek).AddDays(1);
			
			if (date.Date == DateTime.Today)
			{
				return "Today";
			}
			if (date.Date == yesterday)
			{
				return "Yesterday";
			}
			if (date.Date >= previousMonday && date.Date < yesterday)
			{
				return date.DayOfWeek.ToString();
			}

			return "Older";
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
