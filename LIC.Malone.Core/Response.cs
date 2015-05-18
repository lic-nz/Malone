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
	}
}