using System;
using System.Windows;
using System.Windows.Data;

namespace LIC.Malone.Client.Desktop.Converters
{
	public class NullToVisibilityConverter : IValueConverter
	{
		public bool IsReversed { get; set; }
		public bool UseHidden { get; set; }

		#region IValueConverter Members

		public object Convert(object value, Type targetType, object parameter,
			System.Globalization.CultureInfo culture)
		{
			bool notNull = (value != null);
			if (IsReversed) notNull = !notNull;

			if (notNull)
				return Visibility.Visible;
			else
				return this.UseHidden ? Visibility.Hidden : Visibility.Collapsed;
		}

		public object ConvertBack(object value, Type targetType, object parameter,
			System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}