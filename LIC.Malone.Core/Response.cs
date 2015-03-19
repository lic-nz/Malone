using System;
using System.Net;

namespace LIC.Malone.Core
{
	public class Response
	{
		public Guid Guid { get; set; }
		public DateTimeOffset At { get; set; }
		public HttpStatusCode HttpStatusCode { get; set; }
	}
}