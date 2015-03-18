using System.Net;
using System.Web;
using System.Windows.Media;
using LIC.Malone.Client.Desktop.Extensions;

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
			CodeColor = code.ToBrush();
		}
	}
}