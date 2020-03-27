using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TotekanReaderLib
{
    [Serializable]
	public class Range<T>
	{
		public T lower { get; set; }
		public T upper { get; set; }

		public Range(T lower, T upper)
		{
			this.lower = lower;
			this.upper = upper;
		}
	}
}
