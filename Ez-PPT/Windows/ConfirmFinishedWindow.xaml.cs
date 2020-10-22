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
using Ez_PPT.Classes;
using Microsoft.Office.Core;
using Microsoft.Office.Interop.PowerPoint;
using Application = Microsoft.Office.Interop.PowerPoint.Application;

namespace Ez_PPT.Windows
{
	/// <summary>
	/// Interaction logic for ConfirmFinishedWindow.xaml
	/// </summary>
	public partial class ConfirmFinishedWindow : Window
	{
		private SlideInfo slideWhenPressed;
		public ConfirmFinishedWindow(SlideInfo slideWhenPressed)
		{
			InitializeComponent();
			this.slideWhenPressed = slideWhenPressed;
		}

		private void Cancel_Button_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}

		private void Confirm_Finish_Button_Click(object sender, RoutedEventArgs e)
		{
			//Add the page we were on to the list.
			SlideInfoCollection.GetInstance().AddToCollection(this.slideWhenPressed);
			//Fetch our list of slides
			List<SlideInfo> slideInfo = SlideInfoCollection.GetInstance().GetList();
			//Spin up PPT.
			Application application = new Application();

			//These represent the array of slides, the individual slide we are editing, and the area of text we're dealing with.
			Slides slides;
			_Slide slide;
			Microsoft.Office.Interop.PowerPoint.TextRange objText;

			//Open a new presentation, fetch the title page. We'll stitch together the first one by hand before a loop.
			Presentation pptPresentation = application.Presentations.Add(MsoTriState.msoTrue);
			CustomLayout customLayout = pptPresentation.SlideMaster.CustomLayouts[PpSlideLayout.ppLayoutTitle];

			//Assign the slides variable the value of the slides inside of ppt, then create our first title slide.
			slides = pptPresentation.Slides;
			slide = slides.AddSlide(1, customLayout);

			slide.Shapes.Title.TextFrame.TextRange.Text = slideInfo[0].title;
			slide.Shapes[2].TextFrame.TextRange.Text = slideInfo[0].text;
			

			//Onto the regular slides.
			int i = 2;
			bool firstPass = true;
			foreach(SlideInfo info in slideInfo)
			{
				if (firstPass)
				{
					firstPass = false;
					continue;
				}

				int numOfPictures = info.imageURLs.Count;

				//Configure the layout of the slide
				switch (numOfPictures)
				{
					case 0:
						customLayout = pptPresentation.SlideMaster.CustomLayouts[2];
						break;
					case 1:
						customLayout = pptPresentation.SlideMaster.CustomLayouts[9];
						break;
					case 2:
						customLayout = pptPresentation.SlideMaster.CustomLayouts[5];
						break;
				}

				//Add the title of the slide
				slide = slides.AddSlide(i, customLayout);
				slide.Shapes[1].TextFrame.TextRange.Text = info.title;

				//Add info specific to the slide layout
				switch (numOfPictures)
				{
					case 0:
						slide.Shapes[2].TextFrame.TextRange.Text = info.text;
						break;
					case 1:
						objText = slide.Shapes[3].TextFrame.TextRange;
						objText.Text = info.text;

						var shape = slide.Shapes[2];
						slide.Shapes.AddPicture(info.imageURLs[0], MsoTriState.msoFalse, MsoTriState.msoTrue, shape.Left, shape.Top, shape.Width, shape.Height);

						slide.NotesPage.Shapes[2].TextFrame.TextRange.Text = "You may need to manually resize your images!";
						break;
					case 2:
						//split input on double newline
						string[] separators = { "\r\n\r\n" };
						string[] textArr = info.text.Split(separators, StringSplitOptions.RemoveEmptyEntries);
						slide.Shapes[2].TextFrame.TextRange.Text = textArr[0];
						if (textArr.Length <= 1)
						{
							slide.Shapes[4].TextFrame.TextRange.Text = " ";
						}
						else
						{
							slide.Shapes[4].TextFrame.TextRange.Text = textArr[1];
						}

						var shapeAddingTo = slide.Shapes[3];
						slide.Shapes.AddPicture(info.imageURLs[0], MsoTriState.msoFalse, MsoTriState.msoTrue, shapeAddingTo.Left, shapeAddingTo.Top, shapeAddingTo.Width, shapeAddingTo.Height);

						shapeAddingTo = slide.Shapes[5];
						slide.Shapes.AddPicture(info.imageURLs[1], MsoTriState.msoFalse, MsoTriState.msoTrue, shapeAddingTo.Left, shapeAddingTo.Top, shapeAddingTo.Width, shapeAddingTo.Height);

						slide.NotesPage.Shapes[2].TextFrame.TextRange.Text = "You may need to manually resize your images!";
						break;
				}
				i++;
			}

			pptPresentation.SaveAs(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/" + slideInfo[0].title +  @".pptx", Microsoft.Office.Interop.PowerPoint.PpSaveAsFileType.ppSaveAsDefault, Microsoft.Office.Core.MsoTriState.msoTrue);

			System.Windows.Application.Current.Shutdown();
		}
	}
}
