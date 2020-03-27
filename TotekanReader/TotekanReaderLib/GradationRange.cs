using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TotekanReaderLib
{
    [Serializable]
	public class GradationRange
	{
		public Rgb color;
		public float minRange = 0.0f;
		public float maxRange = 0.0f;
		public string pathTexture;

		public GradationRange(float minRange, float maxRange, Rgb color, string pathTexture)
		{
			this.minRange = minRange;
			this.maxRange = maxRange;
			this.color = color;
			this.pathTexture = pathTexture;
		}
	}
}
