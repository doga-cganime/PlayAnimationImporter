using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TotekanReaderLib
{
	public class DoGASufInfo
	{
		public DoGASufInfo(List<TransformElement> listTransforms, string nameObj, string nameFileSuf,
			string nameAtrColor, string nameTexture, string nameMaterial)
		{
			this.listTransforms = listTransforms;
			this.nameObj = nameObj;
			this.nameFileSuf = StringUtil.stripQuote(nameFileSuf);
			this.nameAtrColor = StringUtil.stripQuote(nameAtrColor);
			this.nameTexture = StringUtil.stripQuote(nameTexture);
			this.nameMaterial = StringUtil.stripQuote(nameMaterial);
		}

		public List<TransformElement> listTransforms { get; set; }
		public string nameObj { get; set; }
		public string nameFileSuf;
		public string NameFileSuf {
			get
			{
				return this.nameFileSuf;
			}
			set
			{
				this.nameFileSuf = StringUtil.stripQuote(value);
			}
		}
		public string nameAtrColor { get; set; }
		public string nameTexture { get; set; }
		public string nameMaterial { get; set; }
	}
}
