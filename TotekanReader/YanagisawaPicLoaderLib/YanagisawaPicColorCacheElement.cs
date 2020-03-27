using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YanagisawaPicLoaderLib
{
	[Serializable]
	public class YanagisawaPicColorCacheElement
	{
		public byte[] color = new byte[] { 0xff, 0x00, 0x00, 0x00 };
		public int indexPrev = 0;
		public int indexNext = 0;

		public YanagisawaPicColorCacheElement(byte[] color, int indexPrev, int indexNext)
		{
			this.color = color;
			this.indexPrev = indexPrev;
			this.indexNext = indexNext;
		}

		public void setColor(byte[] color)
		{
			this.color = color;
		}

		public void setIndexPrev(int index)
		{
			this.indexPrev = index;
		}

		public void setIndexNext(int index)
		{
			this.indexNext = index;
		}
	}
}
