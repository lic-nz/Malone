using System.Collections.Generic;
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
			var settings = new Dictionary<string, object> {{"Title", "Malone"}};
			DisplayRootViewFor<AppViewModel>(settings);
		}
	}
}