using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace LIC.Malone.Client.Desktop.Controls
{
	// http://stackoverflow.com/questions/10293236/accessing-the-scrollviewer-of-a-listbox-from-c-sharp

	[StyleTypedProperty(Property = "ItemContainerStyle", StyleTargetType = typeof(ListBoxItem))]
	[Localizability(LocalizationCategory.ListBox)]
	public class MaloneListBox : ListBox
	{
		public Visibility VerticalScrollBarVisibility
		{
			get
			{
				var scrollViewer = GetScrollViewer();
				return scrollViewer.ComputedVerticalScrollBarVisibility;
			}
		}

		public double ScrollViewerHorizontalOffset
		{
			get
			{
				var scrollViewer = GetScrollViewer();
				var offset = scrollViewer.ContentHorizontalOffset;
				return offset;
			}
		}

		private ScrollViewer GetScrollViewer()
		{
			return GetDescendantByType(this, typeof (ScrollViewer)) as ScrollViewer;
		}

		public static Visual GetDescendantByType(Visual element, Type type)
		{
			if (element == null)
				return null;
			if (element.GetType() == type)
				return element;
			
			Visual foundElement = null;
			
			if (element is FrameworkElement)
				(element as FrameworkElement).ApplyTemplate();

			for (var i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
			{
				var visual = VisualTreeHelper.GetChild(element, i) as Visual;
				foundElement = GetDescendantByType(visual, type);
				
				if (foundElement != null)
					break;
			}

			return foundElement;
		}
	}
}