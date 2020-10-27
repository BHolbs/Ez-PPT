using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Security.Policy;
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
using Ez_PPT.Classes;

namespace Ez_PPT.Windows
{
	/// <summary>
	/// Interaction logic for SearchResultsWindow.xaml
	/// </summary>
	public partial class SearchResultsWindow : Window
	{
		private SlideInfo si;
		public SearchResultsWindow(List<String> urls, ref SlideInfo si)
		{
			InitializeComponent();
			this.si = si;
			foreach(String url in urls)
			{
				BitmapImage img = new BitmapImage();
				img.BeginInit();
				img.UriSource = new Uri(url, UriKind.RelativeOrAbsolute);
				img.CacheOption = BitmapCacheOption.OnLoad;
				img.EndInit();
				this.ImageList.Items.Add(new Image { Source = img, MaxHeight=500, MaxWidth=500, Margin=new Thickness(2) });
			}

			if (this.ImageList.Items.IsEmpty)
			{
				TextBlock empty = new TextBlock { Text = "Sorry, I couldn't find anything. Are you sure you entered something into either field?" };
				this.ImageList.Items.Add(empty);
			}
		}

		private void Cancel_Button_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}

		private void Confirm_Button_Click(object sender, RoutedEventArgs e)
		{
			int count = 0;
			si.ClearImageUrls();
			foreach (Image item in this.ImageList.SelectedItems)
			{
				si.AddToImageURLs(item.Source.ToString());
				count++;
			}

			if (count > 2)
			{
				this.Error.Text = "Please narrow your selection down to 2 items at most.";
			}
			else
			{
				this.Close();
			}
		}
	}
}
