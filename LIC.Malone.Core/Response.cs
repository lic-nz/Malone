using System;
using System.Net;

namespace LIC.Malone.Core
{
	public class Response
	{
		public Guid Guid { get; set; }
		public DateTimeOffset At { get; set; }
		public HttpStatusCode HttpStatusCode { get; set; }

		// TODO: Don't pollute core with Caliburn shiz.
		#region Caliburn workarounds

		public string AtString
		{
			get { return At.ToLocalTime().ToString("dddd dd MMMM yyyy hh:mm:ss.FFF"); }
		}

		#endregion
	}
}