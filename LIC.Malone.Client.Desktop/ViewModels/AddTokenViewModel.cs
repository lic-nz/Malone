using System.Windows;
using Caliburn.Micro;
using LIC.Malone.Client.Desktop.Messages;
using LIC.Malone.Core.Authentication.OAuth;

namespace LIC.Malone.Client.Desktop.ViewModels
{
	public class AddTokenViewModel : Screen
	{
		private readonly IEventAggregator _bus;

		#region Databound properties

		private string _name;
		public string Name
		{
			get { return _name; }
			set
			{
				_name = value;
				NotifyOfPropertyChange(() => Name);
			}
		}

		#endregion

		public AddTokenViewModel(IEventAggregator bus)
		{
			_bus = bus;
			_bus.Subscribe(this);
		}

		public void AddToken()
		{
			var x = new NamedAuthorizationState("Hai!", null);
			var message = new TokenAdded
			{
				NamedAuthorizationState = x
			};

			_bus.PublishOnUIThread(message);
		}
	}
}
