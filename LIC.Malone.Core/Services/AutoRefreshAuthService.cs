using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Caliburn.Micro;
using LIC.Malone.Core.Authentication.OAuth;
using LIC.Malone.Core.Scheduling;

namespace LIC.Malone.Core.Services
{
	/// <summary>
	/// A long-living backend service which is trying to refresh expired/soon-to-expire tokens.
	/// </summary>
	public class AutoRefreshAuthService : IAutoRefreshAuthService
	{
		private readonly ITaskSchedulerService _taskSchedulerService;
		private readonly List<OAuthApplication> _authApplications;
		private readonly ConcurrentDictionary<Guid, NamedAuthorizationState> _tokenDictionary;
		private RepeatingTimerTask _task;

		/// <summary>
		/// How often does this service refresh all tokens.
		/// </summary>
		public double RepeatingIntervalMinutes = 1;

		/// <summary>
		/// Refresh the tokens which is about to expire within this time frame.
		/// </summary>
		public TimeSpan RefreshTimeFrame = TimeSpan.FromMinutes(3);

		public event AuthRefreshEventHandler TokenChanged;

		public AutoRefreshAuthService(ITaskSchedulerService taskSchedulerService, List<OAuthApplication> authApplications)
		{
			_taskSchedulerService = taskSchedulerService;
			_authApplications = authApplications;
			_tokenDictionary = new ConcurrentDictionary<Guid, NamedAuthorizationState>();
		}

		public void UpdateTokens(params NamedAuthorizationState[] tokens)
		{
			foreach (var t in tokens)
			{
				_tokenDictionary.AddOrUpdate(t.Guid, t, (guid, state) => t);
			}
		}

		public void Start()
		{
			_task = _taskSchedulerService.AddRepeatingTask(InternalCheckAuthExpiration);
		}

		public void Stop()
		{
			if (_task != null)
				_taskSchedulerService.Remove(_task);

			if (_authApplications != null)
				_authApplications.Clear();

			if (_tokenDictionary != null)
				_tokenDictionary.Clear();
		}

		private DateTime InternalCheckAuthExpiration()
		{
			foreach (var kvp in _tokenDictionary)
			{
				var token = kvp.Value;
				if (!token.ShouldRefresh) continue;

				var expirationUtc = token.AuthorizationState.AccessTokenExpirationUtc;

				var expirationOffset = expirationUtc - DateTime.UtcNow;

				if (!(expirationOffset < TimeSpan.Zero) && !(expirationOffset < RefreshTimeFrame))
					continue;

				var app = _authApplications.FirstOrDefault(a => a.ClientIdentifier == token.AuthorizationState.GetClientIdentifier());

				if (app == null)
					continue;

				var result = app.Refresh(token.Url, token.AuthorizationState);

				if (result != null && !result.HasError)
					NotifyTokenChange(new AuthRefreshEventArgs(token));
			}

			return DateTime.UtcNow.AddMinutes(RepeatingIntervalMinutes);
		}

		private void NotifyTokenChange(AuthRefreshEventArgs args)
		{
			if (TokenChanged == null)
				return;

			Execute.OnUIThread(() =>
			{
				TokenChanged(this, args);
			});
		}
	}
}