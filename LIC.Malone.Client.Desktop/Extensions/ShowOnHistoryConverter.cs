using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Data;
using LIC.Malone.Core;

namespace LIC.Malone.Client.Desktop.Extensions
{
	public class ShowOnHistoryConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var list = (IEnumerable<Request>) value;
			return list.Any() ? Visibility.Visible : Visibility.Collapsed;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}
}