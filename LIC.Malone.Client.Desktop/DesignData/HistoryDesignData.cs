using System.Collections.Generic;
using System.Net;
using LIC.Malone.Core;
using RestSharp;

namespace LIC.Malone.Client.Desktop.DesignData
{
	public class HistoryDesignData : List<Request>
	{
		private static readonly IEnumerable<Request> FakeHistory = new List<Request>
		{
			new Request("http://api.openstreetmap.org/api/capabilities", Method.GET)
			{
				Response = new Response
				{
					HttpStatusCode = HttpStatusCode.OK
				}
			},
			new Request("http://api.openstreetmap.org/api/0.6/changeset/create", Method.PUT)
			{
				Response = new Response
				{
					HttpStatusCode = HttpStatusCode.PaymentRequired
				}
			},
			new Request("http://httpbin.org/get", Method.GET)
			{
				Response = new Response
				{
					HttpStatusCode = HttpStatusCode.OK
				}
			},
			new Request("http://httpbin.org/get", Method.POST)
			{
				Response = new Response
				{
					HttpStatusCode = HttpStatusCode.MethodNotAllowed
				}
			}
		};

		public HistoryDesignData() : base(FakeHistory)
		{
		}
	}
}