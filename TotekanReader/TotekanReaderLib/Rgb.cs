using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TotekanReaderLib
{
    [Serializable]
	public class Rgb
	{
		public float r;
		public float g;
		public float b;

		public Rgb(float r = 0.0f, float g = 0.0f, float b = 0.0f)
		{
			this.r = r;
			this.g = g;
			this.b = b;
		}

		public Rgb(Vec3 vec)
		{
			this.r = vec.x;
			this.g = vec.y;
			this.b = vec.z;
		}

		public void set(float r, float g, float b)
		{
			this.r = r;
			this.g = g;
			this.b = b;
		}

	}
}
