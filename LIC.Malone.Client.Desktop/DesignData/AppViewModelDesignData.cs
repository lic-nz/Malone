using System.Windows.Data;
using Caliburn.Micro;
using LIC.Malone.Client.Desktop.ViewModels;
using LIC.Malone.Core;

namespace LIC.Malone.Client.Desktop.DesignData
{
	public class AppViewModelDesignData : AppViewModel
	{
		public AppViewModelDesignData()
		{
			//HistoryView = new BindableCollection<Request>(new HistoryDesignData());
			//HistoryView = new BindingListCollectionView(new HistoryDesignData());
		}
	}
}