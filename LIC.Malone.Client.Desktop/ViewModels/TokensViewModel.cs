using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;

namespace LIC.Malone.Client.Desktop.ViewModels
{
	public class TokensViewModel : Screen
	{
		private IEventAggregator _bus;

		public TokensViewModel(IEventAggregator bus)
		{
			_bus = bus;
		}
	}
}
