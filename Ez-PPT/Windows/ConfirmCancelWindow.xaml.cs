using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Ez_PPT.Windows
{
	/// <summary>
	/// Interaction logic for ConfirmCancelWindow.xaml
	/// </summary>
	public partial class ConfirmCancelWindow : Window
	{
		public ConfirmCancelWindow()
		{
			InitializeComponent();
		}

		private void Confirm_Cancel_Button_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
			// since we warned the user this will kill any progress, just go ahead and kill the process.
			Application.Current.Shutdown();
		}

		private void Cancel_Button_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}
	}
}
