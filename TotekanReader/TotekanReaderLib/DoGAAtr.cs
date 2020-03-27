using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace TotekanReaderLib
{
    [Serializable]
	public class DoGAAtr
	{
		public static string[] namesTypeColorValid = new string[] { "col", "amb", "dif", "tra", "ref", "rfr", "size", "spc", "spcsize" };
		public static string[] namesTypeMapValid = new string[] { "colormap", "bumpmap", "ambmap", "difmap", "tramap", "spcmap", "sizemap", "refmap", "rfrmap", "hmap", "glowpowermap", "glowradiusmap" };
		public static string[] namesTypeRangeMapValid = new string[] { "mapsize", "mapwind", "mapview" };

		public string name;
		public string path;
		public List<string> pathsTexture = new List<string>();
		Dictionary<string, Rgb> colors = new Dictionary<string, Rgb>();

		public float? spcH = null;

		public Dictionary<string, List<TextureInfo>> textures = new Dictionary<string, List<TextureInfo>>();
		public Dictionary<string, Range<Vec2>> mapRanges = new Dictionary<string, Range<Vec2>>();

		public DoGAAtrOption option = new DoGAAtrOption();

		public DoGAAtr(string name, string path)
		{
			this.name = StringUtil.stripQuote(name);
			this.path = StringUtil.stripQuote(path);
			this.pathsTexture.Add(this.path);
		}

		public void clearExceptColor()
		{
			this.clearTexture();
			this.clearMaterial();
		}

		public void clearExceptTexture()
		{
			this.clearColor();
			this.clearMaterial();
		}

		public void clearExceptMaterial()
		{
			this.clearColor();
			this.clearTexture();
		}

		public void clearColor()
		{
			if (this.colors.ContainsKey("col")) {
				this.colors.Remove("col");
			}
		}

		public void clearTexture()
		{
			this.textures.Clear();
			this.mapRanges.Clear();
		}

		public void clearMaterial()
		{
			foreach (var nameTypeColor in DoGAAtr.namesTypeColorValid) {
				if (nameTypeColor == "col") continue;
				if (this.colors.ContainsKey(nameTypeColor)) {
					this.colors.Remove(nameTypeColor);
				}
			}
			this.spcH = null;
			this.option.clear();
		}

		public void merge(DoGAAtr atr)
		{
			this.path = atr.path;
			this.pathsTexture.Add(atr.path);

			foreach (var typeColor in atr.colors.Keys) {
				if (this.colors.ContainsKey(typeColor)) {
					this.multiplyColor(typeColor, atr.colors[typeColor]);
				} else {
					this.colors.Add(typeColor, atr.colors[typeColor].CloneDeep());
				}
			}
			if (atr.spcH != null) this.spcH = atr.spcH;

			foreach (var typeMap in atr.textures.Keys) {
				if (this.textures.ContainsKey(typeMap)) {
					this.textures[typeMap] = atr.textures[typeMap].CloneDeep();
				} else {
					this.textures.Add(typeMap, atr.textures[typeMap].CloneDeep());
				}
			}
			this.option.merge(atr.option);
		}

		public void setColor(string typeColor, Rgb rgb)
		{
			Debug.Assert(DoGAAtr.getIsValidNameTypeColor(typeColor));
			if (this.colors.ContainsKey(typeColor)) {
				this.colors[typeColor] = rgb;
			} else {
				this.colors.Add(typeColor, rgb);
			}

		}

		public void multiplyColor(string typeColor, Rgb rgb)
		{
			Debug.Assert(DoGAAtr.getIsValidNameTypeColor(typeColor));
			if (this.colors.ContainsKey(typeColor)) {
				var rgbOrg = this.colors[typeColor];
				rgbOrg.r *= rgb.r;
				rgbOrg.g *= rgb.g;
				rgbOrg.b *= rgb.b;
			} else {
				this.colors.Add(typeColor, rgb);
			}
		}

		public bool getHasColor(string typeColor)
		{
			Debug.Assert(DoGAAtr.getIsValidNameTypeColor(typeColor));
			return this.colors.ContainsKey(typeColor) ? true : false;
		}

		public Rgb getColor(string typeColor)
		{
			Debug.Assert(DoGAAtr.getIsValidNameTypeColor(typeColor));
			if (!this.getHasColor(typeColor)) return null;
			return this.colors[typeColor];
		}

		public void addInfoTexture(string typeMap, TextureInfo infoTexture)
		{
			Debug.Assert(DoGAAtr.getIsValidNameTypeMap(typeMap));
			Debug.Assert(typeMap == infoTexture.typeMap);
			if (this.getHasInfoTexture(typeMap)) {
				this.textures[typeMap].Add(infoTexture);
			} else {
				var infosTexture = new List<TextureInfo>();
				infosTexture.Add(infoTexture);
				this.textures.Add(typeMap, infosTexture);
			}
		}

		public List<TextureInfo> getInfoTexture(string typeMap)
		{
			Debug.Assert(DoGAAtr.getIsValidNameTypeMap(typeMap));
			if (!this.getHasInfoTexture(typeMap)) return null;
			return this.textures[typeMap];
		}

		public bool getHasInfoTexture(string typeMap)
		{
			Debug.Assert(DoGAAtr.getIsValidNameTypeMap(typeMap));
			return this.textures.ContainsKey(typeMap) ? true : false;
		}

		public void setRangeMap(string typeRange, Range<Vec2> range)
		{
			Debug.Assert(DoGAAtr.getIsValidNameTypeRangeMap(typeRange));
			if (this.getHasRangeMap(typeRange)) {
				this.mapRanges[typeRange] = range;
			} else {
				this.mapRanges.Add(typeRange, range);
			}
		}

		public Range<Vec2> getRangeMap(string typeRange)
		{
			Debug.Assert(DoGAAtr.getIsValidNameTypeRangeMap(typeRange));
			if (!this.getHasRangeMap(typeRange)) return null;
			return this.mapRanges[typeRange];
		}

		public bool getHasRangeMap(string typeRange)
		{
			Debug.Assert(DoGAAtr.getIsValidNameTypeRangeMap(typeRange));
			return this.mapRanges.ContainsKey(typeRange) ? true : false;
		}

		public static bool getIsValidNameTypeColor(string typeColor)
		{
			return DoGAAtr.namesTypeColorValid.Contains<string>(typeColor) ? true : false;
		}

		public static bool getIsValidNameTypeMap(string typeMap)
		{
			return DoGAAtr.namesTypeMapValid.Contains<string>(typeMap) ? true : false;
		}

		public static bool getIsValidNameTypeRangeMap(string typeRange)
		{
			return DoGAAtr.namesTypeRangeMapValid.Contains<string>(typeRange) ? true : false;
		}
	}
}
