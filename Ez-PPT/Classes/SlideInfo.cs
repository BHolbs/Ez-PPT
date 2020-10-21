using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ez_PPT.Classes
{
	public class SlideInfo
	{
		public String title { get; set; }
		public String text  { get; set; }
		public List<String> imageURLs { get; set; }

		public SlideInfo()
		{
			this.imageURLs = new List<String>();
		}

		public SlideInfo(String title, String text, List<String> imageURLs)
		{
			this.title = title;
			this.text = text;
			this.imageURLs = imageURLs;
		}

		public void AddToImageURLs(String url)
		{
			this.imageURLs.Add(url);
		}
	}
}
