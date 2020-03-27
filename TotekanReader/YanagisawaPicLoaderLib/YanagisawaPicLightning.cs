using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace YanagisawaPicLoaderLib
{
	public class YanagisawaPicLightning
	{
		public byte[] color;
		public Pos2 posStart;
		public List<int> offsetsX = new List<int>();
		private int indexOffset = 0;
		public Pos2 posCurrent;

		public int numOffsets { get { return this.offsetsX.Count; } }

		public YanagisawaPicLightning(Pos2 posStart)
		{
			this.posStart = posStart;
			this.posCurrent = posStart.CloneDeep();
		}

		public void addOffset(int offset)
		{
			this.offsetsX.Add(offset);
		}


		public bool doesIntsersect(int y)
		{
			return (this.posStart.y <= y && y <= this.posStart.y + offsetsX.Count) ? true : false;
		}

		public void reset()
		{
			this.indexOffset = 0;
			this.posCurrent = this.posStart.CloneDeep();
		}
		
		public bool moveNext()
		{
			if (this.offsetsX.Count <= this.indexOffset) return false;
			this.posCurrent.x += this.offsetsX[this.indexOffset];
			this.posCurrent.y ++;
			this.indexOffset ++;

			return true;
		}
	}
}
