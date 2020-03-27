using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TotekanReaderLib
{
	public class PrimUVPoly : PrimBase, PrimUVIF
	{
		public List<Vec2> uvs = new List<Vec2>();

		public List<Vec2> getUVs()
		{
			return this.uvs;
		}

		public PrimUVPoly()
		{
		}

		public void addVertex(Vec3 position, Vec2 uv)
		{
			this.positions.Add(position);
			this.uvs.Add(uv);
		}

	}
}
