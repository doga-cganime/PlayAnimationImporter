using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TotekanReaderLib
{
	public class PrimPoly : PrimBase
	{
		public void addVertex(Vec3 position)
		{
			this.positions.Add(position);
		}
	}
}
