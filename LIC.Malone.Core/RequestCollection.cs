using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using LIC.Malone.Core.Authentication.OAuth;
using Newtonsoft.Json;
using RestSharp;
using Caliburn.Micro;
using System.Collections.ObjectModel;

namespace LIC.Malone.Core
{
	public class RequestCollection
	{
		public string Name { get; set; }
		public List<Request> Requests { get; set; }
		public IObservableCollection<Request> _Requests
		{
			get
			{
				if (Requests != null)
				{
					foreach (var r in Requests)
					{
						r.Collection = this;
					}
				}
				return new BindableCollection<Request>(Requests);
			}
		}

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