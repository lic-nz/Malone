using System.Threading.Tasks;
using System.Windows;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace LIC.Malone.Client.Desktop
{
	public class DialogManager
	{
		public async Task<MessageDialogResult> Show(string title, string message)
		{
			return await Show(title, message, MessageDialogStyle.Affirmative);
		}

		public async Task<MessageDialogResult> Show(string title, string message, MessageDialogStyle dialogStyle)
		{
			return await Show(title, message, dialogStyle, new MetroDialogSettings());
		}

		public async Task<MessageDialogResult> Show(string title, string message, MessageDialogStyle dialogStyle, MetroDialogSettings settings)
		{
			var metroWindow = (MetroWindow) Application.Current.MainWindow;
			return await metroWindow.ShowMessageAsync(title, message, dialogStyle, settings);
		}
	}
}