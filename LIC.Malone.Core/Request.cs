﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using LIC.Malone.Core.Authentication.OAuth;
using Newtonsoft.Json;
using RestSharp;

namespace LIC.Malone.Core
{
	public class Request
	{
		public const string DateFormatString = "dddd dd MMMM yyyy HH:mm:ss.FFF";

		public Guid Guid { get; set; }
		
		public DateTimeOffset At { get; set; }
		public string Url { get; set; }
		public Method Method { get; set; }
		public string Body { get; set; }
		public List<Header> Headers { get; set; }
		public Response Response { get; set; }

		public const string HistoryGroupKey = "At";

		public NamedAuthorizationState NamedAuthorizationState { get; set; }

		// TODO: Don't pollute core with Caliburn shiz.
		#region Caliburn workarounds

		public string ResponseTime
		{
			get
			{
				if (Response == null)
					return string.Empty;

				var diff = Response.At - this.At;
				var ms = diff.TotalMilliseconds;
				return string.Format("{0:n0}ms", ms);
			}
		}

		#endregion

		public string BaseUrl
		{
			get { return GetBaseUrl(); }
		}

		public string ResourcePath
		{
			get { return GetResourcePath(); }
		}

		[JsonConstructor]
		public Request() : this(string.Empty)
		{
		}

		public Request(string url) : this(url, Method.GET)
		{
			
		}

		public Request(string url, Method method, List<Header> headers = null)
		{
			Guid = Guid.NewGuid();
			Url = url;
			Method = method;
			Body = string.Empty;
			Headers = headers ?? new List<Header>();
		}

		public MaloneRestRequest ToMaloneRestRequest()
		{
			var url = GetUri();
			var baseUrl = GetBaseUrl(url);
			var resourcePath = GetResourcePath(url);
			var request = new MaloneRestRequest(url, baseUrl, resourcePath, Method);
			
			foreach (var header in Headers.Where(h => h.Name != Header.ContentType))
				request.AddHeader(header.Name, header.Value);

			request.AddParameter(Headers.Single(h => h.Name == Header.ContentType).Value, Body, ParameterType.RequestBody);

			return request;
		}

		private Uri GetUri()
		{
			Uri url;
			if (!Uri.TryCreate(Url, UriKind.Absolute, out url))
				throw new Exception(string.Format("Failed to create a URI from the string '{0}'.", Url));

			return url;
		}

		private string GetBaseUrl()
		{
			return GetBaseUrl(GetUri());
		}

		private string GetBaseUrl(Uri url)
		{
			var builder = new UriBuilder(url.Scheme, url.Host);

			if (!url.IsDefaultPort)
				builder.Port = url.Port;

			var baseUrl = builder.ToString();

			// Remove trailing slash to keep RestSharp happy.
			return baseUrl.Substring(0, baseUrl.Length - 1);
		}

		private string GetResourcePath()
		{
			return GetResourcePath(GetUri());
		}

		private string GetResourcePath(Uri url)
		{
			var path = url.PathAndQuery;

			// Remove leading slash to keep RestSharp happy.
			if (path.StartsWith("/"))
				path = path.Substring(1);

			return path;
		}
	}
}