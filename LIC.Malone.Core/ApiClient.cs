using System;
using RestSharp;

namespace LIC.Malone.Core
{
	public class ApiClient
	{
		public SendResult Send(Request request)
		{
			return Send(request.ToMaloneRestRequest());
		}

		private SendResult Send(MaloneRestRequest request)
		{
			var result = new SendResult();

			var client = new RestClient(request.BaseUrl);

			result.SentAt = DateTimeOffset.UtcNow;
			var response = client.Execute(request);
			result.ReceivedAt = DateTimeOffset.UtcNow;
			result.Response = response;

			return result;
		}
	}
}