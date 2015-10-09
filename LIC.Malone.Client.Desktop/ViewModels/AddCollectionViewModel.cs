using System.Windows;
using Caliburn.Micro;
using System.Collections.Generic;
using LIC.Malone.Core;

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

		public void Create()
		{
			AppViewModel.Instance.Collections.Add(new Core.RequestCollection() { Name = this.CollectionName, Requests = new List<Request>() });
			AppViewModel.Instance.Collections = AppViewModel.Instance.Collections;

			this.CollectionName = string.Empty;

			this.TryClose();
		}

		public string CollectionName { get; set; }
	}
}
