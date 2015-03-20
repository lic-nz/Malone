using System;
using RestSharp;

namespace LIC.Malone.Core
{
	public class SendResult
	{
		public IRestResponse Response { get; set; }
		public DateTimeOffset SentAt { get; set; }
		public DateTimeOffset ReceivedAt { get; set; }
	}
}
