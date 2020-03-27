using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YanagisawaPicLoaderLib
{
	public class YanagisawaPicLineSegment
	{
		public int x;
		public byte[] color;

		public YanagisawaPicLineSegment(int x, byte[] color)
		{
			this.x = x;
			this.color = color;
		}

		static public int compareLessThan(YanagisawaPicLineSegment seg0, YanagisawaPicLineSegment seg1)
		{
			return (seg0.x == seg1.x) ? 0 : ((seg0.x < seg1.x) ? -1 : 1);
		}
	}
}
