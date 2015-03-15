using RestSharp;

namespace LIC.Malone.Core
{
	public class ApiClient
	{
		public IRestResponse Send(Request request)
		{
			return Send(request.ToMaloneRestRequest());
		}

		private IRestResponse Send(MaloneRestRequest request)
		{
			var client = new RestClient(request.BaseUrl);
			var response = client.Execute(request);

			return response;
		}
	}
}
