using System.Net;
using System.Web;
using System.Windows.Media;

namespace LIC.Malone.Client.Desktop.ViewModels
{
	public class HttpStatusCodeViewModel
	{
		public int Code { get; private set; }
		public string Description { get; private set; }
		public Brush CodeColor { get; private set; }

		public HttpStatusCodeViewModel(HttpStatusCode code)
		{
			Code = (int) code;
			Description = HttpWorkerRequest.GetStatusDescription(Code);
			CodeColor = GetBackgroundColor(code);

		}

		private SolidColorBrush GetBackgroundColor(HttpStatusCode code)
		{
			var ok = new SolidColorBrush(Color.FromRgb(4, 150, 4));
			var maybe = new SolidColorBrush(Color.FromRgb(245, 113, 25));
			var nope = new SolidColorBrush(Color.FromRgb(235, 40, 34));

			switch (code)
			{
				case HttpStatusCode.OK:
					return ok;

				case HttpStatusCode.NoContent:
					return maybe;

				default:
					return nope;
			}
		}
	}
}