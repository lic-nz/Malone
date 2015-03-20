using System;
using RestSharp;

namespace LIC.Malone.Core
{
	public class MaloneRestResponse : RestResponseBase, IRestResponse
	{
		public DateTimeOffset ReceivedAt { get; set; }
	}
}