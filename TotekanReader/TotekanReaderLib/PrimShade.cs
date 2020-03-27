using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TotekanReaderLib
{
	public class PrimShade : PrimBase, PrimNormalIF
	{
		public List<Vec3> normals = new List<Vec3>();

		public List<Vec3> getNormals()
		{
			return this.normals;
		}

		public PrimShade()
		{
		}

		public void addVertex(Vec3 position, Vec3 normal)
		{
			this.positions.Add(position);
			this.normals.Add(normal);
		}
	}
}
