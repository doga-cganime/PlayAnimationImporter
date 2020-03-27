using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TotekanReaderLib
{
	public class InstallInfo : ProductInfo
	{
		public string pathInstalled { get; set; }
		public ProductInfo infoProduct { get; set; }
		public Dictionary<string, string> profiles;

		public InstallInfo(InstallInfo infoInstall)
			: base(infoInstall.infoProduct)
		{
			this.pathInstalled = infoInstall.pathInstalled;
			this.loadProfileIni();
		}

		public InstallInfo(string pathInstalled, ProductInfo infoProduct)
			: base(infoProduct)
		{			
			this.pathInstalled = pathInstalled;
			this.loadProfileIni();
		}

		private void loadProfileIni()
		{
			var iniProduct = new IniProfile(this.pathInstalled + this.pathRelativeExe + this.nameFileIni);
			this.profiles = iniProduct.profiles[this.nameSection];
		}

	}
}
