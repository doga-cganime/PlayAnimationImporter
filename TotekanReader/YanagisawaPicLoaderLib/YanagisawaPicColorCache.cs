using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YanagisawaPicLoaderLib
{
	public class YanagisawaPicColorCache
	{
		public int sizeCache = 128;
		private List<YanagisawaPicColorCacheElement> elements = new List<YanagisawaPicColorCacheElement>();
		private int indexColorP = 0;

		public YanagisawaPicColorCache()
		{
			var colorInit = new byte[] { 0xff, 0x00, 0x00, 0x00 };
			for (var index = 0; index < sizeCache; index++) {
				int indexNext = (index == 0) ? sizeCache - 1 : index - 1;
				int indexPrev = (index == sizeCache - 1) ? 0 : index + 1;
				this.elements.Add(new YanagisawaPicColorCacheElement(colorInit.CloneDeep(), indexPrev, indexNext));
			}
			
		}

		public byte[] getColor(int index)
		{
			if (index != this.indexColorP) {
				// Detatch index from elements list.
				this.elements[this.elements[index].indexPrev].setIndexNext(this.elements[index].indexNext);
				this.elements[this.elements[index].indexNext].setIndexPrev(this.elements[index].indexPrev);
				// Set index at the next of latest element.
				this.elements[this.elements[this.indexColorP].indexPrev].setIndexNext(index);
				this.elements[index].setIndexPrev(this.elements[this.indexColorP].indexPrev);
				this.elements[this.indexColorP].setIndexPrev(index);
				this.elements[index].setIndexNext(this.indexColorP);
				// Update the latest location.
				this.indexColorP = index;
			}
			return this.elements[index].color;
		}

		public void addColor(byte[] color)
		{
			this.indexColorP = this.elements[this.indexColorP].indexPrev;
			this.elements[this.indexColorP].setColor(color);
		}
	}
}
