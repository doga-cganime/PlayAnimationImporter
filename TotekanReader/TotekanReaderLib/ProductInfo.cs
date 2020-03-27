using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TotekanReaderLib
{
	public class ProductInfo
	{
		public string idProductUnInstall;
		public string idProductAuth;
		public string nameProduct;
		public string pathRelativeExe { get; set; }
		public string nameFileIni { get; set; }
		public string nameSection { get; set; }
		public string nameExtPartsAsm { get; set; }

		public ProductInfo(string idProductUnInstall, string idProductAuth, string nameProduct, string pathRelativeExe, string nameFileIni, string nameSection, string nameExtPartsAsm)
		{
			this.idProductUnInstall = idProductUnInstall;
			this.idProductAuth = idProductAuth;
			this.nameProduct = nameProduct;
			this.pathRelativeExe = pathRelativeExe;
			this.nameFileIni = nameFileIni;
			this.nameSection = nameSection;
			this.nameExtPartsAsm = nameExtPartsAsm;
		}

		public ProductInfo(ProductInfo infoProduct)
		{
			this.idProductUnInstall = infoProduct.idProductUnInstall;
			this.idProductAuth = infoProduct.idProductAuth;
			this.nameProduct = infoProduct.nameProduct;
			this.pathRelativeExe = infoProduct.pathRelativeExe;
			this.nameFileIni = infoProduct.nameFileIni;
			this.nameSection = infoProduct.nameSection;
			this.nameExtPartsAsm = infoProduct.nameExtPartsAsm;
		}
	}
}
