using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using LIC.Malone.Client.Desktop.Messages;

namespace LIC.Malone.Client.Desktop.ViewModels
{
	public class MainViewModel : Screen
	{
		private IEventAggregator _bus;

		public MainViewModel(IEventAggregator bus)
		{
			_bus = bus;
		}

		public void ManageTokens()
		{
			_bus.BeginPublishOnUIThread(new ShowTokensScreen());
		}
	}
}