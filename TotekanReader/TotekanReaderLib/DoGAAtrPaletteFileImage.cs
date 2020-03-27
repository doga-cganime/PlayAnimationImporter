using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TotekanReaderLib
{
	public class DoGAAtrPaletteFileImage : DoGAAtrPaletteBase
	{
		public string nameAtrInSuf;
		public string nameFileImage;
		public string nameAtrInAtr;

		public DoGAAtrPaletteFileImage(string nameAtrInSuf, string nameFileImage)
		{
			this.nameAtrInSuf = StringUtil.stripQuote(nameAtrInSuf);
			this.nameFileImage = StringUtil.stripQuote(nameFileImage);
		}
	}
}
