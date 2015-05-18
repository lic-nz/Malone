using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace LIC.Malone.Client.Desktop.Converters
{
	public class DateTimeFormatConverter : IValueConverter
	{
		public const string DefaultDateTimeFormatString = "dddd dd MMMM yyyy HH:mm:ss.FFF";
		public bool InUtc { get; set; }
		public string Format { get; set; }

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is DateTime)
				return (InUtc ? ((DateTime)value).ToUniversalTime() : ((DateTime)value).ToLocalTime()).ToString(Format ?? DefaultDateTimeFormatString);
			else if (value is DateTimeOffset)
				return (InUtc ? ((DateTimeOffset)value).UtcDateTime : ((DateTimeOffset)value).LocalDateTime).ToString(Format ?? DefaultDateTimeFormatString);

			return DependencyProperty.UnsetValue;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}