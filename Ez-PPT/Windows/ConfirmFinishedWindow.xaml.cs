using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using TextRange = Microsoft.Office.Interop.PowerPoint.TextRange;

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
			//Add the page we were on to the list, unless it's totally empty
			if (this.slideWhenPressed.text != null || this.slideWhenPressed.title != null ||  this.slideWhenPressed.imageURLs.Count != 0)
			{
				SlideInfoCollection.GetInstance().AddToCollection(this.slideWhenPressed);
			}
			//Fetch our list of slides
			List<SlideInfo> slideInfo = SlideInfoCollection.GetInstance().GetList();
			//Spin up PPT.
			Application application = new Application();

			//These represent the array of slides, the individual slide we are editing, and the area of text we're dealing with.
			Slides slides;
			_Slide slide;

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

				/*
				 * There is a lot of duplicated code living here.
				 * The way that interop is set up, you can't pass these TextRanges as ref in functions,
				 * so I can't do this in a nice function call, I have to do it here.
				 */
				List<int> boldWordIndexes = new List<int>();
				switch (numOfPictures)
				{
					case 0:
						boldWordIndexes = GetIndicesOfBold(info.text);
						info.text = info.text.Replace("**", "");
						slide.Shapes[2].TextFrame.TextRange.Text = info.text;
						foreach(int boldIndex in boldWordIndexes)
						{
							slide.Shapes[2].TextFrame.TextRange.Words(boldIndex).Font.Bold = MsoTriState.msoTrue;
						}
						break;
					case 1:
						boldWordIndexes = GetIndicesOfBold(info.text);
						info.text = info.text.Replace("**", "");
						slide.Shapes[3].TextFrame.TextRange.Text = info.text;
						foreach (int boldIndex in boldWordIndexes)
						{
							slide.Shapes[3].TextFrame.TextRange.Words(boldIndex).Font.Bold = MsoTriState.msoTrue;
						}
						var shape = slide.Shapes[2];
						slide.Shapes.AddPicture(info.imageURLs[0], MsoTriState.msoFalse, MsoTriState.msoTrue, shape.Left, shape.Top, shape.Width, shape.Height);

						slide.NotesPage.Shapes[2].TextFrame.TextRange.Text = "You may need to manually resize your images!";
						break;
					case 2:
						//split input on double newline
						string[] separators = { "\r\n\r\n" };
						string[] textArr = info.text.Split(separators, StringSplitOptions.RemoveEmptyEntries);
						boldWordIndexes = GetIndicesOfBold(textArr[0]);
						slide.Shapes[2].TextFrame.TextRange.Text = textArr[0].Replace("**", "");
						foreach (int boldIndex in boldWordIndexes)
						{
							slide.Shapes[2].TextFrame.TextRange.Words(boldIndex).Font.Bold = MsoTriState.msoTrue;
						}
						if (textArr.Length <= 1)
						{
							slide.Shapes[4].TextFrame.TextRange.Text = " ";
						}
						else
						{
							boldWordIndexes = GetIndicesOfBold(textArr[1]);
							slide.Shapes[4].TextFrame.TextRange.Text = textArr[1].Replace("**", "");
							foreach (int boldIndex in boldWordIndexes)
							{
								slide.Shapes[4].TextFrame.TextRange.Words(boldIndex).Font.Bold = MsoTriState.msoTrue;
							}
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

		public List<int> GetIndicesOfBold(string text)
		{
			List<int> outList = new List<int>();
			string[] tokens = text.Split(' ');

			//since interop indexes their words with starting at 1 indexing, let's start at 1
			int i = 1;
			bool wordIsBolded = false;
			foreach(var token in tokens)
			{
				if (token.StartsWith("**"))
				{
					wordIsBolded = true;
				}

				if (wordIsBolded)
				{
					outList.Add(i);
				}

				if (token.EndsWith("**"))
				{
					wordIsBolded = false;
				}
				i++;
			}
			
			return outList;
		}
	}
}
