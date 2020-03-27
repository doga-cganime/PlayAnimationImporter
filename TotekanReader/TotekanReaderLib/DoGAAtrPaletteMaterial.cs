using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TotekanReaderLib
{
	public class DoGAAtrPaletteMaterial : DoGAAtrPaletteBase
	{
		public string nameAtrInSuf;
		public float valTra = 0.0f;
		public float valAmb = 0.0f;
		public float valDif = 0.0f;
		public float valSp1 = 0.0f;
		public float valSp2 = 0.0f;
		public float valSp3 = 0.0f;
		public float valRef = 0.0f;

		public DoGAAtrPaletteMaterial(string nameAtrInSuf, float valTra, float valAmb, float valDif, float valSp1, float valSp2, float valSp3, float valRef)
		{
			this.nameAtrInSuf = StringUtil.stripQuote(nameAtrInSuf);
			this.valTra = valTra;
			this.valAmb = valAmb;
			this.valDif = valDif;
			this.valSp1 = valSp1;
			this.valSp2 = valSp2;
			this.valSp3 = valSp3;
			this.valRef = valRef;
		}
	}
}
