using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace ArrayImageProcessingLib
{
    public class ArrayImageProcessing
    {
		/// <summary>
		/// widthDst, heightDst の画像サイズにリサイズしつつ、bytesSrc の画像を bytesDst の画像に乗算ブレンドをする。
		/// </summary>
		/// <param name="doesBlend">ブレンドするかどうか。最初の画像は false にしないと真っ黒になっていまう。</param>
		/// <param name="numElems">1ピクセルの要素数</param>
		/// <param name="widthSrc">ソース画像の横幅</param>
		/// <param name="heightSrc">ソース画像の縦幅</param>
		/// <param name="bytesSrc">ソース画像のバイト列</param>
		/// <param name="widthDst">合成先画像の横幅</param>
		/// <param name="heightDst">合成先画像の縦幅</param>
		/// <param name="bytesDst">合成先画像のバイト列幅</param>
		public static unsafe void blendImageResized(bool doesBlend, int numElems, int widthSrc, int heightSrc, byte[] bytesSrc,
									int widthDst, int heightDst, byte[] bytesDst)
		{
			byte* pPixel = stackalloc byte[numElems];
			fixed (byte* pBytesDst = bytesDst) {
				fixed (byte* pBytesSrc = bytesSrc) {
					byte* pDst = pBytesDst;
					for (int yDst = 0; yDst < heightDst; yDst ++) {
						for (int xDst = 0; xDst < widthDst; xDst ++) {
							getColor32Resized(numElems, widthSrc, heightSrc, pBytesSrc, xDst, yDst, widthDst, heightDst, pPixel);
							int indexPixelDst = yDst * widthDst + xDst;
							int indexElemDst = indexPixelDst * numElems;
							byte* pPtrPixel = pPixel;
							for (int indexElem = 0; indexElem < numElems; indexElem ++) {
								byte dst = *pDst;
								if (doesBlend) {
									*pDst = (byte)(((((int)*pPtrPixel + 1) * ((int)dst + 1)) >> 8));
								} else {
									*pDst = *pPtrPixel;
								}
								pPtrPixel ++;
								pDst ++;
							}
						}
					}
				}
			}
		}
		
		private static unsafe void getColor32Resized(int numElems, int widthSrc, int heightSrc, byte* pSrc,
									int xDst, int yDst, int widthDst, int heightDst, byte* pPixel)
		{
			if (widthSrc == widthDst && heightSrc == heightDst) {
				pSrc += (yDst * widthSrc + xDst) * numElems; 
				for (int indexElem = 0; indexElem < numElems; indexElem++) {
					*pPixel ++ = *pSrc ++;					
				}
				return;
			}

			float xSrc = (float)xDst * (float)(widthSrc - 1) / (float)(widthDst - 1);
			float ySrc = (float)yDst * (float)(heightSrc - 1) / (float)(heightDst - 1);
			int xSrcInt = (int)Math.Floor(xSrc);
			float xDecimal = xSrc - (float)xSrcInt;
			int ySrcInt = (int)Math.Floor(ySrc);
			float yDecimal = ySrc - (float)ySrcInt;

			var xSrcIntNext = (xSrcInt + 1 < widthSrc) ? (xSrcInt + 1) : xSrcInt;
			var ySrcIntNext = (ySrcInt + 1 < heightSrc) ? (ySrcInt + 1) : ySrcInt;

			byte* pPixel00 = stackalloc byte[numElems];
			byte* pPixel10 = stackalloc byte[numElems];
			byte* pPixel01 = stackalloc byte[numElems];
			byte* pPixel11 = stackalloc byte[numElems];
			copyPixel(pSrc, ySrcInt * widthSrc + xSrcInt, numElems, pPixel00);
			copyPixel(pSrc, ySrcInt * widthSrc + xSrcIntNext, numElems, pPixel10);
			copyPixel(pSrc, ySrcIntNext * widthSrc + xSrcInt, numElems, pPixel01);
			copyPixel(pSrc, ySrcIntNext * widthSrc + xSrcIntNext, numElems, pPixel11);

			byte* pPixel0 = stackalloc byte[numElems];
			byte* pPixel1 = stackalloc byte[numElems];
			filterBilinear(numElems, pPixel00, pPixel10, xDecimal, pPixel0);
			filterBilinear(numElems, pPixel01, pPixel11, xDecimal, pPixel1);

			filterBilinear(numElems, pPixel0, pPixel1, yDecimal, pPixel);
		}

		private static unsafe void filterBilinear(int numElems, byte* pPixel0, byte* pPixel1, float d, byte* pPixel)
		{
			for (int indexElem = 0; indexElem < numElems; indexElem++) {
				*pPixel ++ = (byte)((float)(*pPixel0++) * (1.0f - d) + (float)(*pPixel1++) * d);
			}
		}

		private static unsafe void copyPixel(byte* pSrc, int indexPixelStartSrc, int numElems, byte* pDst)
		{
			byte* pSrcStart = pSrc + indexPixelStartSrc * numElems;
			for (int indexElem = 0; indexElem < numElems; indexElem++) {
				*pDst++  = * pSrcStart++;
			}
		}
	}
}
