using System;

namespace YanagisawaPicLoaderLib
{
	[Serializable]
	public struct Pos2
	{
		public int x;
		public int y;
	
		public Pos2(int x, int y)
		{
			this.x = x;
			this.y = y;
		}
	
		public Pos2(Pos2 pos)
		{
			this.x = pos.x;
			this.y = pos.y;
		}
	
		public void moveX(int offset)
		{
			this.x += offset;
		}
	
		public void moveY(int offset)
		{
			this.y += offset;
		}
        }
}