using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using LIC.Malone.Core.Authentication.OAuth;
using Newtonsoft.Json;
using RestSharp;

namespace LIC.Malone.Core
{
	public class RequestCollection
	{
		public string Name { get; set; }
		public List<Request> Requests { get; set; }

		[JsonConstructor]
		public RequestCollection() : this(string.Empty, null)
		{
		}

		public RequestCollection(string name, List<Request> requests)
		{
			Name = name;
			Requests = requests;
		}
	}
}