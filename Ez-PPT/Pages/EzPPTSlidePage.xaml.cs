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
using System.Windows.Navigation;
using System.Windows.Shapes;

// FOR PERFORMING GOOGLE IMAGE SEARCHES
using Google.Apis.Customsearch.v1;

// FOR BUILDING THE .PPT
using Powerpoint = Microsoft.Office.Interop.PowerPoint;

using Ez_PPT.Classes;
using Ez_PPT.Windows;

namespace Ez_PPT.Pages
{
	/// <summary>
	/// Interaction logic for EzPPTSlidePage.xaml
	/// </summary>
	public partial class EzPPTSlidePage : Page
	{
		private int index;
		private List<String> urls;
		private SlideInfo currentSlideInfo;
		public EzPPTSlidePage(int prevIndex)
		{
			InitializeComponent();
			this.index = prevIndex + 1;
			this.urls = new List<String>();
			this.currentSlideInfo = new SlideInfo();
		}

		private void Search_Button_Click(object sender, RoutedEventArgs e)
		{
			// Eventually this will actually perform a google image search, for now, let's hold onto some canned URLS (5 will probably fit the use case for now)

			// Opens a new window containing pictures found, probably ListBox or ListView, user can select up to 2 images per slide (more than 2 would be pretty difficult to prevent from getting clumpy
			// When user confirms images, save those images to some data structure containing the current ppt info by slide for use by the Finish handler
			// Populate a label with preview images or some count of how many objects have been selected for that page
			this.urls = new List<String>
			{
				"https://encrypted-tbn0.gstatic.com/images?q=tbn%3AANd9GcRRAD9dz6lOsZgew3MS2IgfqykAhOR5Ds9oxw&usqp=CAU",
				"https://www.gematsu.com/wp-content/uploads/2020/07/Sekiro-Update_07-29-20_001.jpg",
				"https://d3tidaycr45ky4.cloudfront.net/media/catalog/product/cache/1/image/9df78eab33525d08d6e5fb8d27136e95/e/l/elite-dangerous.jpg"
			};
			SearchResultsWindow searchResultsWindow = new SearchResultsWindow(this.urls, ref currentSlideInfo);
			searchResultsWindow.Show();
		}

		private void Next_Button_Click(object sender, RoutedEventArgs e)
		{
			currentSlideInfo.title = this.title.Text;
			currentSlideInfo.title = this.text.Text;
			if (SlideInfoCollection.GetInstance().NumberOfSlides() <= index)
			{
				SlideInfoCollection.GetInstance().AddToCollection(currentSlideInfo);
			}
			else
			{
				SlideInfoCollection.GetInstance().EditSlideInCollection(index, currentSlideInfo);
			}
			EzPPTSlidePage ezPPTSlidePage = new EzPPTSlidePage(index);
			this.NavigationService.Navigate(ezPPTSlidePage);
		}

		private void Cancel_Button_Click(object sender, RoutedEventArgs e)
		{
			// Load a confirm page, if they confirm then give option to restart or exit.
			// This should be pretty easy, save it for last.
		}

		private void Finish_Button_Click(object sender, RoutedEventArgs e)
		{
			// Actually create the ppt here.
			// I have a feeling this will be the hardest part.
		}
	}
}
