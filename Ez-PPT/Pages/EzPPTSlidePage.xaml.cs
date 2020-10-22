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

using Ez_PPT.Classes;
using Ez_PPT.Windows;
using Google.Apis.Services;

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
			// Set up the custom search service then execute the query
			string apiKey = "AIzaSyC9XZlQmVl7c5N-lGnm9YlTlFZkxTjKiRI";
			string context = "47c96d1ee9214d539";
			var customSearchService = new CustomsearchService(new BaseClientService.Initializer { ApiKey = apiKey });
			string query = this.title.Text + " " + this.text.Text;
			var listRequest = customSearchService.Cse.List();
			listRequest.Cx = context;
			listRequest.Q = query;
			listRequest.Num = 10;
			listRequest.SearchType = CseResource.ListRequest.SearchTypeEnum.Image;
			listRequest.Safe = CseResource.ListRequest.SafeEnum.Active;
			var result = listRequest.Execute().Items?.ToList();

			//Add the image URLs to a list
			foreach(var item in result)
			{
				this.urls.Add(item.Link.ToString());
			}

			// Fire off a search results window. Made this choice to make searching more deliberate, the Google CSE API is pretty restrictive, about 100 requests/day
			// Making a box that would autoupdate as the user typed would use that rate up pretty quickly.
			SearchResultsWindow searchResultsWindow = new SearchResultsWindow(this.urls, ref currentSlideInfo);
			searchResultsWindow.Show();
		}

		private void Next_Button_Click(object sender, RoutedEventArgs e)
		{
			currentSlideInfo.title = this.title.Text;
			currentSlideInfo.text = this.text.Text;
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
			ConfirmCancelWindow confirmCancelWindow = new ConfirmCancelWindow();
			confirmCancelWindow.Show();
		}

		private void Finish_Button_Click(object sender, RoutedEventArgs e)
		{
			// Offload the ppt generation to a new confirm window. 
			// This will also quit out of this application once the ppt gets generated and saved, so we should let the user know about that.
			ConfirmFinishedWindow confirmFinishedWindow = new ConfirmFinishedWindow(currentSlideInfo);
			confirmFinishedWindow.Show();
		}
	}
}
