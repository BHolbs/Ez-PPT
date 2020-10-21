using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 *  This is a small Singleton class to contain slide information as we go, rather than try to collect them all at the end.
 */
namespace Ez_PPT.Classes
{
	public sealed class SlideInfoCollection
	{
		private static SlideInfoCollection Instance = null;
		private List<SlideInfo> Slides = new List<SlideInfo>();
		private SlideInfoCollection()
		{

		}

		// I think there's a more C# way to do this, but this describes what I'm trying to do a little bit better.
		public static SlideInfoCollection GetInstance()
		{
			if (Instance == null)
				Instance = new SlideInfoCollection();

			return Instance;
		}

		public void AddToCollection(SlideInfo slideInfo)
		{
			Slides.Add(slideInfo);
		}

		public void EditSlideInCollection(int index, SlideInfo slideInfo)
		{
			Slides[index] = slideInfo;
		}

		public int NumberOfSlides()
		{
			return Slides.Count;
		}
	}
}
