using LIC.Malone.Core.Authentication.OAuth;

namespace LIC.Malone.Core.Services
{
	public interface IAutoRefreshAuthService
	{
		event AuthRefreshEventHandler TokenChanged;

      void UpdateTokens(params NamedAuthorizationState[] tokens);

		void Start();
		void Stop();
	}
}
