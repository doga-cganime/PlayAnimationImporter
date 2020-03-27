using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TotekanReaderLib
{
	[SerializableAttribute]
	public class TransformElement
	{
		public TransformElement(string typeTransform, float[] values)
		{
			this.typeTransform = typeTransform;
			this.values = values;
		}

		public TransformElement(string typeTransform, Vec3 vec)
		{
			this.typeTransform = typeTransform;
			this.values =  new float[] { vec.x, vec.y, vec.z };
		}

		public string typeTransform { get; set; }
		public float[] values { get; set; }
	}
}
