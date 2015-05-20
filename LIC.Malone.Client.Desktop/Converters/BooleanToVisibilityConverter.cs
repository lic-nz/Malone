using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace LIC.Malone.Client.Desktop.Converters
{
	public class BooleanToVisibilityConverter : IValueConverter
	{
		public bool IsReversed { get; set; }
		public bool UseHidden { get; set; }

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var val = (bool) value;
			
			if (IsReversed)
				val = !val;

			if (val)
				return Visibility.Visible;

			return UseHidden ? Visibility.Hidden : Visibility.Collapsed;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}