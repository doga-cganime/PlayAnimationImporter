using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TotekanReaderLib
{
	public class PrimUVShade : PrimBase, PrimNormalIF, PrimUVIF
	{
		public List<Vec3> normals = new List<Vec3>();
		public List<Vec2> uvs = new List<Vec2>();

		public List<Vec3> getNormals()
		{
			return this.normals;
		}

		public List<Vec2> getUVs()
		{
			return this.uvs;
		}

		public PrimUVShade()
		{
		}

		public void addVertex(Vec3 position, Vec3 normal, Vec2 uv)
		{
			this.positions.Add(position);
			this.normals.Add(normal);
			this.uvs.Add(uv);
		}

	}
}
