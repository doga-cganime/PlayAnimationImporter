using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TotekanReaderLib
{
	public class PrimBase : PrimIF
	{
		public List<Vec3> positions = new List<Vec3>();

		public int getNumVertices()
		{
			return this.positions.Count;
		}

		public List<Vec3> getPositions()
		{
			return this.positions;
		}

		public List<int[]> getListIndicesTriangle()
		{
			var listIndices = new List<int[]>();
			int numVertices = this.getNumVertices();
			for (int index = 0; index < numVertices - 2; index++) {
				listIndices.Add(new int[] { 0, index + 1, index + 2 });
			}
			return listIndices;
		}
	}
}
