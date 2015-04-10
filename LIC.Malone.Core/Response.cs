using System;
using System.Collections.Generic;
using System.Net;

namespace LIC.Malone.Core
{
	public class Response
	{
		public Guid Guid { get; set; }
		public DateTimeOffset At { get; set; }
		public HttpStatusCode HttpStatusCode { get; set; }
		public string ContentType { get; set; }
		public string Body { get; set; }
		public List<Header> Headers { get; set; }

		public Response()
		{
			Body = string.Empty;
		}

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