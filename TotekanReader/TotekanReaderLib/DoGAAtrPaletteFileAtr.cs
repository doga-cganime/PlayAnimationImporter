using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TotekanReaderLib
{
	public class DoGAAtrPaletteFileAtr : DoGAAtrPaletteBase
	{
		public string nameAtrInSuf;
		public string nameFileAtr;
		public string nameAtrInAtr;

		public DoGAAtrPaletteFileAtr(string nameAtrInSuf, string nameFileAtr, string nameAtrInAtr)
		{
			this.nameAtrInSuf = StringUtil.stripQuote(nameAtrInSuf);
			this.nameFileAtr = StringUtil.stripQuote(nameFileAtr);
			this.nameAtrInAtr = StringUtil.stripQuote(nameAtrInAtr);
		}

		public DoGAAtrPaletteFileAtr(string nameAtrInSuf, DoGAAtr atr)
		{
			this.nameAtrInSuf = StringUtil.stripQuote(nameAtrInSuf);
			this.atr = atr;
		}
	}
}
