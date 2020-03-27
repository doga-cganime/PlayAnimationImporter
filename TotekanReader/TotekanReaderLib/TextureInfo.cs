using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TotekanReaderLib
{
    [Serializable]
	public class TextureInfo
	{
		public string typeMap;
		public string pathTexture;
		public string PathTexture
		{
			get { return this.pathTexture; }
			set { this.pathTexture = StringUtil.stripQuote(value); }
		}
		public float valueMin;
		public float valueMax;

		public TextureInfo(string typeMap, string pathTexture, float valueMin, float valueMax)
		{
			this.typeMap = typeMap;
			this.pathTexture = StringUtil.stripQuote(pathTexture);
			this.valueMin = valueMin;
			this.valueMax = valueMax;
		}
	}
}
