using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace LIC.Malone.Client.Desktop.Converters
{
	public class ResponseBodyTabIndexConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			// TODO: Remove this once views are broken apart better and you can ActivateItem().
			// This is just a hack to ensure that the right tabs are selected when sending a request.
			return 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}