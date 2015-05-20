using System;
using System.Windows;
using System.Windows.Data;

namespace LIC.Malone.Client.Desktop.Converters
{
	public class NullToVisibilityConverter : IValueConverter
	{
		public bool IsReversed { get; set; }
		public bool UseHidden { get; set; }

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			var notNull = (value != null);

			if (IsReversed)
				notNull = !notNull;

			if (notNull)
				return Visibility.Visible;
			
			return UseHidden ? Visibility.Hidden : Visibility.Collapsed;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}