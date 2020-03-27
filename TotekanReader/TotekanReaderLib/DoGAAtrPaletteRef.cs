using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TotekanReaderLib
{
	public class DoGAAtrPaletteRef : DoGAAtrPaletteBase
	{
		public string nameAtrColor;
		public string nameAtrTexture;
		public string nameAtrMaterial;

		public DoGAAtrPaletteRef(string nameAtrColor, string nameAtrTexture, string nameAtrMaterial) {
			this.nameAtrColor = StringUtil.stripQuote(nameAtrColor);
			this.nameAtrTexture = StringUtil.stripQuote(nameAtrTexture);
			this.nameAtrMaterial = StringUtil.stripQuote(nameAtrMaterial);
		}
	}
}
