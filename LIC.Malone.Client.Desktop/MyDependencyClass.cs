using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace LIC.Malone.Client.Desktop
{
	public class MyDependencyClass : DependencyObject
	{
		#region IsCheckedOnDataProperty

		public static readonly DependencyProperty DataForPathProperty;

		public static void SetDataForPath(DependencyObject DepObject, Canvas value)
		{
			DepObject.SetValue(DataForPathProperty, value);
		}

		public static Canvas GetDataForPath(DependencyObject DepObject)
		{
			return (Canvas)DepObject.GetValue(DataForPathProperty);
		}

		#endregion

		static MyDependencyClass()
		{
			DataForPathProperty = DependencyProperty.RegisterAttached("DataForPath",
				typeof(Canvas),
				typeof(MyDependencyClass),
				new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));
		}
	}
}