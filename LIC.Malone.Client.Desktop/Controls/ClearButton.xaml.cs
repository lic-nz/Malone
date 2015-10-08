using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace LIC.Malone.Client.Desktop.Controls
{
	public partial class ClearButton : Button
	{
		public static readonly DependencyProperty IconDataProperty = DependencyProperty.Register("IconData", typeof(Geometry), typeof(ClearButton), new PropertyMetadata());
		public Geometry IconData
		{
			get { return (Geometry)GetValue(IconDataProperty); }
			set { SetValue(IconDataProperty, value); }
		}

		public static readonly DependencyProperty IconMarginProperty = DependencyProperty.Register("IconMargin", typeof(Thickness), typeof(ClearButton), new PropertyMetadata());
		public Thickness IconMargin
		{
			get { return (Thickness)GetValue(IconMarginProperty); }
			set { SetValue(IconMarginProperty, value); }
		}

		public static readonly DependencyProperty TopProperty = DependencyProperty.Register("Top", typeof(double), typeof(ClearButton), new PropertyMetadata());
		public double Top
		{
			get { return (double)GetValue(TopProperty); }
			set { SetValue(TopProperty, value); }
		}

		public static readonly DependencyProperty LeftProperty = DependencyProperty.Register("Left", typeof(double), typeof(ClearButton), new PropertyMetadata());
		public double Left
		{
			get { return (double)GetValue(LeftProperty); }
			set { SetValue(LeftProperty, value); }
		}

		public ClearButton()
		{
			InitializeComponent();
		}
	}
}