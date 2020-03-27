using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TotekanReaderLib
{
    [Serializable]
	public struct Vec2
	{
		public float x;
		public float y;

		public Vec2(float x = 0.0f, float y = 0.0f)
		{
			this.x = x;
			this.y = y;
		}

		public void set(float x, float y)
		{
			this.x = x;
			this.y = y;
		}

		public float u {
			get { return this.x; }
			set { this.x = value; } 
		}
		public float v {
			get { return this.y; }
			set { this.y = value; }
		}
	}
}
