using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TotekanReaderLib
{
    [Serializable]
	public struct Vec3
	{
		public float x;
		public float y;
		public float z;

		public Vec3(float x = 0.0f, float y = 0.0f, float z = 0.0f)
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}

		public void set(float x, float y, float z)
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}

	}
}
