using System.Windows;
using Caliburn.Micro;
using LIC.Malone.Client.Desktop.ViewModels;

namespace LIC.Malone.Client.Desktop
{
	public class AppBootstrapper : BootstrapperBase
	{
		private readonly SimpleContainer _container = new SimpleContainer();

		public AppBootstrapper()
		{
			Initialize();
		}

		protected override void Configure()
		{
			base.Configure();

			_container.Singleton<IEventAggregator, EventAggregator>();
		}

		protected override void OnStartup(object sender, StartupEventArgs e)
		{
			DisplayRootViewFor<AppViewModel>();
		}
	}
}