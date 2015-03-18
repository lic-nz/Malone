using System.Net;
using System.Windows.Media;

namespace LIC.Malone.Client.Desktop.Extensions
{
	public static class HttpStatusCodeExtensions
	{
		public static Brush ToBrush(this HttpStatusCode code)
		{
			var ok = new SolidColorBrush(Color.FromRgb(4, 150, 4));
			var maybe = new SolidColorBrush(Color.FromRgb(245, 113, 25));
			var nope = new SolidColorBrush(Color.FromRgb(235, 40, 34));

			switch (code)
			{
				case HttpStatusCode.OK:
					return ok;

				case HttpStatusCode.Accepted:
					return ok;

				case HttpStatusCode.NoContent:
					return maybe;

				case HttpStatusCode.Created:
					return maybe;

				default:
					return nope;
			}
		}
	}
}