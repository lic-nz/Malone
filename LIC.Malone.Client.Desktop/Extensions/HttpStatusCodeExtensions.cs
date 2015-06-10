using System.Net;
using System.Windows.Media;

namespace LIC.Malone.Client.Desktop.Extensions
{
	public static class HttpStatusCodeExtensions
	{
		public static Brush ToBrush(this HttpStatusCode code)
		{
			switch (code)
			{
				case HttpStatusCode.OK:
					return Colors.Ok;

				case HttpStatusCode.Accepted:
					return Colors.Ok;

				case HttpStatusCode.NoContent:
					return Colors.Ok;

				case HttpStatusCode.Created:
					return Colors.Maybe;

				default:
					return Colors.Nope;
			}
		}
	}
}