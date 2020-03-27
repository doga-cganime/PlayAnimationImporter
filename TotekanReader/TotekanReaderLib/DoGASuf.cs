using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TotekanReaderLib
{
	public class DoGASuf
	{
		public string name;
		public Dictionary<string, Dictionary<bool, List<PrimIF>>> mapAtrToPrims = new Dictionary<string, Dictionary<bool, List<PrimIF>>>();

		public DoGASuf(string name)
		{
			
			this.name = name;
		}

		public void addAtr(string nameAtr)
		{
			if (this.mapAtrToPrims.ContainsKey(nameAtr)) return; // Do nothing
			var tablePrims = new Dictionary<bool, List<PrimIF>>();
			tablePrims[true] = new List<PrimIF>();
			tablePrims[false] = new List<PrimIF>();
			this.mapAtrToPrims.Add(nameAtr, tablePrims);
		}

		public void addPrim(string nameAtr, bool hasUV, PrimIF prim)
		{
			if (!this.mapAtrToPrims.ContainsKey(nameAtr)) {
				this.addAtr(nameAtr);
			}
			this.mapAtrToPrims[nameAtr][hasUV].Add(prim);
		}
	}
}
