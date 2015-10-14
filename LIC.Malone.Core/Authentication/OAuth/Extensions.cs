using System;
using System.Linq;
using DotNetOpenAuth.OAuth2;

namespace LIC.Malone.Core.Authentication.OAuth
{
	public static class Extensions
	{
		public static string GetClientIdentifier(this IAuthorizationState authorizationState)
		{
			var source = authorizationState.Scope.FirstOrDefault(s => s.StartsWith("Source:"));

			if (source == null || !source.Any())
				return null;

			var parts = source.Split(new[] { ":" }, StringSplitOptions.RemoveEmptyEntries);

			if (parts.Length != 2)
				return null;

			return parts.Last();
		}
	}
}
