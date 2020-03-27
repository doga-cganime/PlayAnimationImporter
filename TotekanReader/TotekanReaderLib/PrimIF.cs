using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TotekanReaderLib
{
	public interface PrimIF
	{
		int getNumVertices();
		List<int[]> getListIndicesTriangle();

		List<Vec3> getPositions();
	}
}
