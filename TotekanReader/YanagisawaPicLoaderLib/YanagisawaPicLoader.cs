using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace YanagisawaPicLoaderLib
{

	public class YanagisawaPicLoader
	{
		public enum PixelOrder
		{
			ARGB,	// for Unity Texture2D.LoadRawTextureData()
			BGRA	// for Marshal.Copy() to Bitmap
		}
		public enum DirectionY
		{
			BottomToTop,	// for Unity Texture2D.LoadRawTextureData()
			TopToBottom		// for Marshal.Copy() to Bitmap
		}

		private const int numBitsByte = sizeof(byte) * 8;

		private List<YanagisawaPicLightning> lightnings = new List<YanagisawaPicLightning>();
		private byte[] bytesImageCompressed;
		public byte[] bytesImageExtracted;
		public PixelOrder orderPixel;
		public DirectionY directionY;

		public int width = 0;
		public int height = 0;
		private int numBitsColor = 15;

		private int offsetBytesCompressed;
		private int offsetBitsCompressed;

		private YanagisawaPicColorCache cacheColor = new YanagisawaPicColorCache();

		private Pos2 posCurrent = new Pos2(-1, 0);
		private byte[] colorCurrent = new byte[] { 0x00, 0x00, 0x00, 0xff }; // bgra
		private List<YanagisawaPicLineSegment> segments = new List<YanagisawaPicLineSegment>();
		private int length = 0;

		public YanagisawaPicLoader(byte[] bytesImageCompressed, PixelOrder orderPixelExtracted = PixelOrder.ARGB, DirectionY directionY = DirectionY.BottomToTop)
		{
			this.bytesImageCompressed = bytesImageCompressed;
			bool result = this.parseHeader();
			this.bytesImageExtracted = new byte[this.width * this.height * 4];
//			this.bytesImageExtracted = new byte[this.width * 4];

			this.orderPixel = orderPixelExtracted;
			this.directionY = directionY;
		}

		public bool extractOneLine()
		{
			while (this.posCurrent.x < this.width) {
				if (this.length == 0) {
					if (!this.readLength(out this.length)) {
						// Debug.WriteLine("Failed in reading length: offsetByte = " + this.offsetBytesCompressed + ", offsetBits = " + this.offsetBitsCompressed);
						return false;
					}
				}

				if (this.width <= this.posCurrent.x + this.length) {
					this.setLineSegment(this.posCurrent, this.width - this.posCurrent.x, this.segments);
					this.length -= this.width - this.posCurrent.x;
					this.posCurrent.x = this.width;
				} else {
					if (0 <= this.posCurrent.x) {
						this.setLineSegment(this.posCurrent, this.length, this.segments);
					}
					this.posCurrent.x += this.length;
					this.length = 0;

					if (!this.readColor(out this.colorCurrent)) {
						Debug.WriteLine("Failed in reading color: offsetByte = " + this.offsetBytesCompressed + ", offsetBits = " + this.offsetBitsCompressed);
						return false;
					}
					// Debug.WriteLine("color=" + this.getStrColor(this.colorCurrent));

					YanagisawaPicLightning lightning = null;
					if (!this.readLightning(this.posCurrent, out lightning)) {
						Debug.WriteLine("Failed in reading lightning: offsetByte = " + this.offsetBytesCompressed + ", offsetBits = " + this.offsetBitsCompressed);
						return false;
					}
					if (lightning != null) {
						lightning.color = this.colorCurrent.CloneDeep();
						// this.drawLightning(lightning); // debug
						this.lightnings.Add(lightning);
						// Debug.WriteLine("lightning size=" + lightning.numOffsets.ToString());
					}

				}
			}

			this.posCurrent.x = 0;
			this.posCurrent.y++;
			if (this.height <= this.posCurrent.y) return true;
			this.segments = this.scanSegmentsActiveNextLine();
	
			return true;
		}

		public bool extract()
		{
			while (true) {
				if (!this.readLength(out this.length)) {
//					Debug.WriteLine("Failed in reading length: offsetByte = " + this.offsetBytesCompressed + ", offsetBits = " + this.offsetBitsCompressed);
					return false;
				}
//				Debug.WriteLine("length=" + this.length.ToString());
				while (this.width <= this.posCurrent.x + this.length) {
					this.setLineSegment(this.posCurrent, this.width - this.posCurrent.x, this.segments);
					this.length -= this.width - this.posCurrent.x;
					this.posCurrent.x = 0;
					this.posCurrent.y++;
					if (this.height <= this.posCurrent.y) return true;
					this.segments = this.scanSegmentsActiveNextLine();
				}
				if (0 < this.length) {
					if (0 <= this.posCurrent.x) {
						this.setLineSegment(this.posCurrent, this.length, this.segments);
					}
					this.posCurrent.x += this.length;
				}

				if (!this.readColor(out this.colorCurrent)) {
					Debug.WriteLine("Failed in reading color: offsetByte = " + this.offsetBytesCompressed + ", offsetBits = " + this.offsetBitsCompressed);
					return false;
				}
//				Debug.WriteLine("color=" + this.getStrColor(this.colorCurrent));
				
				YanagisawaPicLightning lightning = null;
				if (!this.readLightning(this.posCurrent, out lightning)) {
					Debug.WriteLine("Failed in reading lightning: offsetByte = " + this.offsetBytesCompressed + ", offsetBits = " + this.offsetBitsCompressed);
					return false;
				}
				if (lightning != null) {
					lightning.color = this.colorCurrent.CloneDeep();
//					this.drawLightning(lightning); // debug
					this.lightnings.Add(lightning);
//					Debug.WriteLine("lightning size=" + lightning.numOffsets.ToString());
				} else {
//					Debug.WriteLine("lightning none");
				}
			}
		}

		private bool parseHeader()
		{
			// File identifier
			long value = 0;
			if (!readBitsCompressed(8, out value) || value != 'P') return false;
			if (!readBitsCompressed(8, out value) || value != 'I') return false;
			if (!readBitsCompressed(8, out value) || value != 'C') return false;

			// Skip comments
			while (readBitsCompressed(8, out value) && value != 0x1a) ;
			while (readBitsCompressed(8, out value) && value != 0) ;

			// Null terminator
			if (!readBitsCompressed(8, out value) || value != 0) return false;

			// Type / Mode
			if (!readBitsCompressed(8, out value) || value != 0) return false;

			// 15bit/16it color only
			long numBitsColor = 0;
			if (!readBitsCompressed(16, out numBitsColor)) return false;
			if (!(numBitsColor == 15 || numBitsColor == 16)) return false;
			this.numBitsColor = (int)numBitsColor;

			long w = 0, h = 0;
			if (!readBitsCompressed(16, out w)) return false;
			if (!readBitsCompressed(16, out h)) return false;
			this.width = (int)w;
			this.height = (int)h;

			return true;
		}
		
		private void drawLightning(YanagisawaPicLightning lightning)
		{
			var pos = lightning.posStart.CloneDeep();
			this.setPixel(pos, lightning.color);
			var numOffsets = lightning.numOffsets;
			for (var index = 0; index < numOffsets; index++) {
				pos.x += lightning.offsetsX[index];
				pos.y ++;
				this.setPixel(pos, lightning.color);
			}
		}

		private string getStrColor(byte[] color)
		{
			var builder = new StringBuilder();
			builder.Append("[ ");
			foreach (var elem in color) {
				builder.Append(elem.ToString());
				builder.Append(" ");
			}
			builder.Append("] ");

			return builder.ToString();
		}

		private List<YanagisawaPicLineSegment> scanSegmentsActiveNextLine()
		{
			var segments = new List<YanagisawaPicLineSegment>();
			for (int index = 0, indexNext = 0; indexNext < this.lightnings.Count; index = indexNext) {
				indexNext = index + 1;
				var lightning = this.lightnings[index];
				segments.Add(new YanagisawaPicLineSegment(lightning.posCurrent.x, lightning.color));
				if (!lightning.moveNext()) {
					this.lightnings.Remove(lightning);
					indexNext = index;
				}
			}

			segments.Sort(YanagisawaPicLineSegment.compareLessThan);
			return segments;
		}

		private void setLineSegment(Pos2 pos, int length, List<YanagisawaPicLineSegment> segments)
		{
			System.Diagnostics.Debug.Assert(this.colorCurrent.Length == 4);
			int index = (this.width * this.getYExtracted(pos.y) + pos.x) * 4;
			// int index = pos.x * 4;
			var enumerator = segments.GetEnumerator();
			YanagisawaPicLineSegment segmentCurrent = enumerator.MoveNext() ? enumerator.Current : null;
			while (segmentCurrent != null && segmentCurrent.x < pos.x) {
				this.colorCurrent = segmentCurrent.color;
				segmentCurrent = enumerator.MoveNext() ? enumerator.Current : null;
			}
			for (var indexPixel = 0; indexPixel < length; indexPixel++) {
				if (segmentCurrent != null && segmentCurrent.x <= pos.x + indexPixel) {
					this.colorCurrent = segmentCurrent.color;
					segmentCurrent = enumerator.MoveNext() ? enumerator.Current : null;
				}
				for (var indexByte = 0; indexByte < this.colorCurrent.Length; indexByte++) {
					this.bytesImageExtracted[index + indexByte] = this.colorCurrent[indexByte];
				}
				index += 4;
			}			
		}

		private void setPixel(Pos2 pos, byte[] color)
		{
			int index = (this.width * this.getYExtracted(pos.y) + pos.x) * 4;
			for (var indexByte = 0; indexByte < color.Length; indexByte++) {
				this.bytesImageExtracted[index + indexByte] = color[indexByte];
			}						
		}

		private int getYExtracted(int y)
		{
			return (this.directionY == DirectionY.BottomToTop) ? (this.height - y - 1) : y;
		}

		private bool readLength(out int length)
		{
			length = 0;

			int numBitsHead = 1;
			long value = 0;
			while (this.readBitsCompressed(1, out value) && value != 0) {
				numBitsHead++;
			}

			if (!this.readBitsCompressed(numBitsHead, out value)) return false;
			length = (int)value + (1 << numBitsHead) - 1;
			return true;
		}

		private bool readColor(out byte[] color)
		{
			color = new byte[] { 0xff, 0x00, 0x00, 0x00 };
			long value = 0;
			if (!this.readBitsCompressed(1, out value)) return false;
			if (value == 0) {
				// not cached
				if (!this.readBitsCompressed(this.numBitsColor, out value)) return false;
				color = this.convertColor(value);
				this.cacheColor.addColor(color);
			} else {
				if (!this.readBitsCompressed(7, out value)) return false;
				int index = (int)value;
				color = this.cacheColor.getColor(index);
			}
			return true;
		}

		private byte[] convertColor(long colorPic)
		{
			int bitTransparent = 0;
			if (this.numBitsColor == 16) {
				bitTransparent = (int)(colorPic & (long)0x1);
				colorPic = colorPic >> 1;
			}
			// 0ggg ggrr rrrb bbbb
			var g = (colorPic & (long)0x7c00) >> 10;
			g = (g << 3) | (g >> 2);
			var r = (colorPic & (long)0x03e0) >> 5;
			r = (r << 3) | (r >> 2);
			var b = (colorPic & (long)0x001f);
			b = (b << 3) | (b >> 2);

			var a = (bitTransparent == 0) ? 0xff : 0x00;

			if (this.orderPixel == PixelOrder.BGRA) {
				return new byte[] { (byte)b, (byte)g, (byte)r, (byte)a };
			} else {
				return new byte[] { (byte)a, (byte)r, (byte)g, (byte)b };
			}
		}

		private bool readLightning(Pos2 pos, out YanagisawaPicLightning lightning)
		{
			lightning = new YanagisawaPicLightning(pos);

			long value = 0;
			if (!this.readBitsCompressed(1, out value)) return false;
			if (value == 0) return true;

			bool needToExit = false;
			while (!needToExit) {
				if (!this.readBitsCompressed(2, out value)) return false;
				var next = value;
				switch (next) {
					case 0x0:
						if (!this.readBitsCompressed(1, out value)) return false;
						if (value == 1) {
							if (!this.readBitsCompressed(1, out value)) return false;
							var next2 = value;
							lightning.addOffset(next2 == 0 ? -2 : 2);
						} else {
							needToExit = true;
						}
						break;
					case 0x1:
						lightning.addOffset(-1);
						break;
					case 0x2:
						lightning.addOffset(0);
						break;
					case 0x3:
						lightning.addOffset(1);
						break;
				}
			}
			return true;
		}

		private bool readBitsCompressed(int numBits, out long result)
		{
			result = 0;
			int numBitsOrg = numBits;
//			Debug.WriteLine("reading " + numBitsOrg + " ...");

            if (0 < this.offsetBitsCompressed) {
	            int numBitsMask = (numBits < numBitsByte - this.offsetBitsCompressed) ? numBits : (numBitsByte - this.offsetBitsCompressed);
			    byte maskHead = this.getMask(this.offsetBitsCompressed, numBitsMask);
                result = (this.bytesImageCompressed[this.offsetBytesCompressed] & maskHead) >> (numBitsByte - this.offsetBitsCompressed - numBitsMask);
	            if (!((numBits < numBitsByte - this.offsetBitsCompressed))) {
				    this.offsetBytesCompressed++;
					if (this.bytesImageCompressed.Length <= this.offsetBytesCompressed) return false;
			    }		
			    numBits -= numBitsMask;
			    
                this.offsetBitsCompressed = (this.offsetBitsCompressed + numBitsMask) % numBitsByte;
	        }

			while (numBitsByte < numBits) {
				long value = this.bytesImageCompressed[this.offsetBytesCompressed];
				result = (result << numBitsByte) | value;
				this.offsetBytesCompressed ++;
				if (this.bytesImageCompressed.Length <= this.offsetBytesCompressed) return false;
				numBits -= numBitsByte;
			}
			if (0 < numBits) {
				byte maskTail = this.getMask(0, numBits);
				long value = (this.bytesImageCompressed[this.offsetBytesCompressed] & maskTail) >> (numBitsByte - numBits);
                result = (result << numBits) | value;
				this.offsetBitsCompressed = numBits;
			}

//			Debug.WriteLine("read " + numBitsOrg + "bits: value = " + result);

			return true;
		}

		private byte getMask(int numBitsFromMSB, int numBitsOne = 0)
		{
			const int numBitsByte = sizeof(byte) * 8;
			if (numBitsOne == 0) {
				numBitsOne = numBitsByte - numBitsFromMSB;
			}
			if (numBitsByte < numBitsFromMSB + numBitsOne) return (byte)0;

			int maskUpper = 0xff >> numBitsFromMSB;
			int maskLower = (byte)(0xff << (numBitsByte - numBitsFromMSB - numBitsOne));

			return (byte)(maskUpper & maskLower);
		}
	}
}