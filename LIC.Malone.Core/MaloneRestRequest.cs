using System;
using RestSharp;

namespace LIC.Malone.Core
{
	public class MaloneRestRequest : RestRequest
	{
		public Uri AbsoluteUrl { get; private set; }
		public string BaseUrl { get; private set; }
		public string ResourcePath { get; private set; }

		public MaloneRestRequest(Uri absoluteUrl, string baseUrl, string resourcePath, Method method) : base(resourcePath, method)
		{
			AbsoluteUrl = absoluteUrl;
			BaseUrl = baseUrl;
			ResourcePath = resourcePath;
		}
	}
}