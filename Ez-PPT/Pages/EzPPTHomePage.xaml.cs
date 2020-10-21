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
using Ez_PPT.Classes;

namespace Ez_PPT.Pages
{
	/// <summary>
	/// Interaction logic for EzPPTHomePage.xaml
	/// </summary>
	public partial class EzPPTHomePage : Page
	{
		private readonly int index;
		public EzPPTHomePage()
		{
			// the title page will always be the first page of a presentation
			this.index = 0;
			InitializeComponent();
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			EzPPTSlidePage ezPPTSlidePage = new EzPPTSlidePage(index);
			this.NavigationService.Navigate(ezPPTSlidePage);
			// no images on title page, just text
			SlideInfo slideInfo = new SlideInfo(this.title.Text, this.subtitle.Text, null);
			SlideInfoCollection.GetInstance().AddToCollection(slideInfo);
		}
	}
}
