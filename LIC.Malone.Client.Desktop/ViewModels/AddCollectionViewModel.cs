using System.Windows;
using Caliburn.Micro;

namespace LIC.Malone.Client.Desktop.ViewModels
{
	public class AddCollectionViewModel : Screen
	{
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

		protected override void OnActivate()
		{
			this.DisplayName = "hwoeir";
			base.OnActivate();
		}
	}
}
