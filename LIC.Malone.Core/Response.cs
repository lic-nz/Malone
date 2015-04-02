using System;
using System.Net;

namespace LIC.Malone.Core
{
	public class Response
	{
		public Guid Guid { get; set; }
		public DateTimeOffset At { get; set; }
		public HttpStatusCode HttpStatusCode { get; set; }
		public string ContentType { get; set; }

		public string Content { get; set; }

		// TODO: Don't pollute core with Caliburn shiz.
		#region Caliburn workarounds

		public string AtLocalString
		{
			get { return At.ToLocalTime().ToString(Request.DateFormatString); }
		}

		public string AtUtcString
		{
			get { return At.ToString(Request.DateFormatString); }
		}

		#endregion
	}
}