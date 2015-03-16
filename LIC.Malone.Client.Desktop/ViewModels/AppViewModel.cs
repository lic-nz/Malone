using Caliburn.Micro;

namespace LIC.Malone.Client.Desktop.ViewModels
{
	public class AppViewModel : Conductor<object>
	{
		public MainViewModel MainViewModel { get; set; }
		public TokensViewModel TokensViewModel { get; set; }

		public AppViewModel()
		{
			var bus = IoC.Get<EventAggregator>();

			MainViewModel = new MainViewModel(bus);
			TokensViewModel = new TokensViewModel(bus);

			ShowMainScreen();
		}

		public void ShowMainScreen()
		{
			ActivateItem(MainViewModel);
		}
	}
}