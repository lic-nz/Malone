using System.Collections.Generic;
using System.Linq;
using LIC.Malone.Core;

namespace LIC.Malone.Client.Desktop.Extensions
{
	public static class ListExtensions
	{
		public static string GetValue(this List<Header> headers, string name)
		{
			if (headers == null)
				return null;

			var header = headers.FirstOrDefault(h => h.Name == name);

			if (header == null)
				return null;

			return header.Value;
		}
	}
}