using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using Antlr4.Runtime.Tree;

namespace TotekanReaderLib
{
	public class DoGAAtrInfo
	{
		public const string prefixPaletteColor = "Color";
		public const string prefixPaletteTexture = "Texture";
		public const string prefixPaletteMaterial = "Material";
		public const string prefixPaletteAttribute = "Attribute";
		public const string prefixPaletteDepthColor = "DepthColor";
		public const string prefixPaletteLightColor = "LightColor";

		public string[] tablePrifixes = {
			DoGAAtrInfo.prefixPaletteColor,  DoGAAtrInfo.prefixPaletteTexture, 
			DoGAAtrInfo.prefixPaletteMaterial, DoGAAtrInfo.prefixPaletteAttribute, 
			DoGAAtrInfo.prefixPaletteDepthColor, DoGAAtrInfo.prefixPaletteLightColor
		};

		public string pathExe { get; set; }
		public string nameFileGenie;
		public string pathAtr;

		public List<string> namesPrefixAtrToUsePalette = new List<string>();

		Dictionary<string, DoGAAtr> atrsGenie =	new Dictionary<string, DoGAAtr>();
		Dictionary<string, Dictionary<string, DoGAAtrPaletteBase>> palettes = new Dictionary<string, Dictionary<string, DoGAAtrPaletteBase>>();

		public DoGAAtrInfo(string nameProduct, string pathExe, Dictionary<string, string> profiles)
		{
			this.pathExe = pathExe;
			this.nameFileGenie = this.pathExe + profiles["AtrFile"];
			this.pathAtr = Path.GetDirectoryName(this.nameFileGenie);

			this.loadAtrs(profiles);
			/*
			ColorChangeAtr=BodyM*,BodyD*,GrayG*,Brown*,Main*
			//ColorChangeColorSetting=..\atr\color.ini
			//ColorChangeMaterialSetting=..\atr\material.ini
			AtrFile=..\atr\genie.atr
			AtrFile と同じディレクトリにある atr.ini を読み込み
			TOTEKAN2.ini か atr.ini から Color*, Texture*, Material* を読み込む(color.atr, texture.atr, material.atr)
			 * 
			 */

		}

		public DoGAAtr getAtrFromPalette(string nameAtrInSuf, string nameAtrColor, string nameAtrTexture, string nameAtrMaterial,
							Dictionary<string, DoGAAtr> atrsInPartsAssembler = null)
		{
			DoGAAtr atr = null;
			if (this.atrsGenie.ContainsKey(nameAtrInSuf.ToLower())) {
				atr = this.atrsGenie[nameAtrInSuf.ToLower()].CloneDeep();
			} else {
				atr = new DoGAAtr(nameAtrInSuf, this.pathAtr);
			}

			if (!this.doesUsePalette(nameAtrInSuf)) return atr;

			// If atr found in L3 attribute, use it and ignore palette.
			if (atrsInPartsAssembler != null && nameAtrColor != null && atrsInPartsAssembler.ContainsKey(nameAtrColor)) {
				atr.clearTexture();
				atr.merge(atrsInPartsAssembler[nameAtrColor]);
				return atr;
			}
	
			var atrRet = this.mergeAtrFromPalette(atr, nameAtrColor, nameAtrTexture, nameAtrMaterial);
			
			return atrRet;
		}

		public DoGAAtr getAtrFromPalette(DoGAAtr atr, string nameAtrColor, string nameAtrTexture, string nameAtrMaterial,
							 Dictionary<string, DoGAAtr> atrsInPartsAssembler = null)
		{
			if (!this.doesUsePalette(atr.name)) return atr;

			// If atr found in L3 attribute, use it and ignore palette.
			if (atrsInPartsAssembler != null && nameAtrColor != null && atrsInPartsAssembler.ContainsKey(nameAtrColor)) {
				atr.clearTexture();
				atr.merge(atrsInPartsAssembler[nameAtrColor]);
				return atr;
			}

			var atrRet = this.mergeAtrFromPalette(atr, nameAtrColor, nameAtrTexture, nameAtrMaterial);
			return atrRet;
		}

		public DoGAAtr mergeAtrFromPalette(DoGAAtr atr, string nameAtrColor, string nameAtrTexture, string nameAtrMaterial)
		{
			var atrsAttribute = this.palettes[DoGAAtrInfo.prefixPaletteAttribute];

			var atrsColor = this.palettes[DoGAAtrInfo.prefixPaletteColor];
			DoGAAtr atrColor = null;
			if (nameAtrColor != null) {
				if (atrsColor.ContainsKey(nameAtrColor)) {
					atrColor = atrsColor[nameAtrColor].atr.CloneDeep();
				} else if (atrsAttribute.ContainsKey(nameAtrColor)) {
					atrColor = atrsAttribute[nameAtrColor].atr.CloneDeep();
				}
			}
			if (atrColor != null) {
				atrColor.clearExceptColor();
				atr.merge(atrColor);
			}

			var atrsTexture = this.palettes[DoGAAtrInfo.prefixPaletteTexture];
			DoGAAtr atrTexture = null;
			if (nameAtrTexture != null) {
				if (atrsTexture.ContainsKey(nameAtrTexture)) {
					atrTexture = atrsTexture[nameAtrTexture].atr.CloneDeep();
				} else if (atrsAttribute.ContainsKey(nameAtrTexture)) {
					atrTexture = atrsAttribute[nameAtrTexture].atr.CloneDeep();
				}
			}
			if (atrTexture != null) {
				atrTexture.clearExceptTexture();
				atr.clearTexture();
				atr.merge(atrTexture);
			}

			var atrsMaterial = this.palettes[DoGAAtrInfo.prefixPaletteMaterial];
			DoGAAtr atrMaterial = null;
			if (nameAtrMaterial != null) {
				if (atrsMaterial.ContainsKey(nameAtrMaterial)) {
					atrMaterial = atrsMaterial[nameAtrMaterial].atr.CloneDeep();
				} else if (atrsAttribute.ContainsKey(nameAtrMaterial)) {
					atrMaterial = atrsAttribute[nameAtrMaterial].atr.CloneDeep();
				}
			}
			if (atrMaterial != null) {
				atrMaterial.clearExceptMaterial();
				atr.clearMaterial();
				atr.merge(atrMaterial);
			}

			return atr;
		}

		private bool doesUsePalette(string nameAtrInSuf)
		{
			foreach (var prefix in this.namesPrefixAtrToUsePalette) {
				if (nameAtrInSuf.StartsWith(prefix)) {
					return true;
				}
			}
			return false;
		}

		private bool loadAtrs(Dictionary<string, string> profiles)
		{

			//ColorChangeAtr=BodyM*,BodyD*,GrayG*,Brown*,Main*
			var namesPrefixAtrChange = profiles["ColorChangeAtr"].Split(',');
			foreach (var namePrefix in namesPrefixAtrChange) {
				var namePrefixLowered = namePrefix.ToLower();
				var index = namePrefixLowered.LastIndexOf('*');
				string prefix = namePrefixLowered;
				if (index != -1) {
					prefix = namePrefixLowered.Substring(0, index);
				}
				this.namesPrefixAtrToUsePalette.Add(prefix);
			}

			// load genie.atr
			var context = TotekanParse.parseAtr(this.nameFileGenie);
			var visitor = new DoGAAtrVisitor(this.pathAtr);
			visitor.Visit(context as IParseTree);
			this.atrsGenie = getMapAtrs(visitor.atrs);

			// load atr.ini
			var setNamesFileAtr = new HashSet<string>();			
			var namesFileIni = Directory.GetFiles(this.pathAtr, "*.ini", SearchOption.TopDirectoryOnly);
			foreach (var nameFileIni in namesFileIni) {
				var result = this.loadIniPalette(nameFileIni, ref setNamesFileAtr);
				if (!result) return false;
			}
			// load atrs

			var mapNameFileToAtr = loadFilesAtr(this.pathAtr, setNamesFileAtr);

			// associate palettes and atr
			this.associatePaletteFileAtrToAtr(mapNameFileToAtr);
			this.associatePaletteFileImageToAtr();
			this.associatePaletteMaterialToAtr();
			this.associatePaletteRefToAtr();

			return true;
		}

		private bool loadIniPalette(string nameFileIniAtr, ref HashSet<string> setNamesFileAtr)
		{
			var profilesAtr = new IniProfile(nameFileIniAtr);
			var profilesPalette = profilesAtr.profiles["Attribute"];

			foreach (var prefix in this.tablePrifixes) {
				Dictionary<string, DoGAAtrPaletteBase> mapAtr;
				if (!this.palettes.ContainsKey(prefix)) {
					mapAtr = new Dictionary<string, DoGAAtrPaletteBase>();
					this.palettes.Add(prefix, mapAtr);
				} else {
					mapAtr = this.palettes[prefix];
				}
				this.extractMapAtr(profilesPalette, prefix, ref mapAtr, ref setNamesFileAtr);
			}
			return true;
		}

		public static Dictionary<string, Dictionary<string, DoGAAtr>> loadFilesAtr(string pathAtr, HashSet<string> setNamesFileAtr)
		{
			var mapNamesFileAtrFullPath = new Dictionary<string, string>();
			foreach (var nameFile in setNamesFileAtr) {
				mapNamesFileAtrFullPath.Add(nameFile, pathAtr + @"\" + nameFile);
			}

			return loadFilesAtr(mapNamesFileAtrFullPath);
		}

		public static Dictionary<string, Dictionary<string, DoGAAtr>> loadFilesAtr(Dictionary<string, string> mapNamesFileAtrFullPath)
		{
			var mapNameFileToAtr = new Dictionary<string, Dictionary<string, DoGAAtr>>();
			// load atr files described in atr.ini
			foreach (var pairNameFileFullPath in mapNamesFileAtrFullPath) {
				var nameFile = pairNameFileFullPath.Key;
				var nameFileFullPath = pairNameFileFullPath.Value;
				if (mapNameFileToAtr.ContainsKey(nameFileFullPath)) continue;
				if (!File.Exists(nameFileFullPath)) continue;
				var contextAtr = TotekanParse.parseAtr(nameFileFullPath);
				var visitorAtr = new DoGAAtrVisitor(Path.GetDirectoryName(nameFileFullPath));
				visitorAtr.Visit(contextAtr as IParseTree);
				mapNameFileToAtr.Add(nameFile, getMapAtrs(visitorAtr.atrs));
			}
			return mapNameFileToAtr;
		}

		private bool associatePaletteFileAtrToAtr(Dictionary<string, Dictionary<string, DoGAAtr>> mapNameFileToAtr)
		{
			foreach (var palette in this.palettes) {
				var nameCategory = palette.Key;
				var mapAtrs = palette.Value;
				foreach (var mapAtr in mapAtrs) {
					if (!(mapAtr.Value is DoGAAtrPaletteFileAtr)) continue;
					var paletteFile = mapAtr.Value as DoGAAtrPaletteFileAtr;
					var nameAtrInSuf = mapAtr.Key;
					var nameFileAtr = paletteFile.nameFileAtr;
					if (nameFileAtr == null) continue;
					var nameAtrInAtr = paletteFile.nameAtrInAtr;

					// atr.ini のアトリビュート名の不具合の対応
					if (nameAtrInAtr == "emission1") nameAtrInAtr = "emittion1";

					var atrsFile = mapNameFileToAtr[nameFileAtr];
					var atr = atrsFile[nameAtrInAtr];
					mapAtr.Value.atr = atr;
				}
			}

			return true;
		}

		private bool associatePaletteFileImageToAtr()
		{
			foreach (var palette in this.palettes) {
				var nameCategory = palette.Key;
				var mapAtrs = palette.Value;
				foreach (var mapAtr in mapAtrs) {
					if (!(mapAtr.Value is DoGAAtrPaletteFileImage)) continue;
					var paletteFileImage = mapAtr.Value as DoGAAtrPaletteFileImage;
					var nameAtrInSuf = mapAtr.Key;
					var nameFileImage = paletteFileImage.nameFileImage;
					if (nameFileImage == null) continue;

					var atr = new DoGAAtr(nameAtrInSuf, this.pathAtr);
					atr.addInfoTexture("colormap", new TextureInfo("colormap", nameFileImage, 0.0f, 1.0f));

					mapAtr.Value.atr = atr;
				}
			}

			return true;
		}

		private bool associatePaletteMaterialToAtr()
		{
			foreach (var palette in this.palettes) {
				var nameCategory = palette.Key;
				var mapAtrs = palette.Value;
				foreach (var mapAtr in mapAtrs) {
					if (!(mapAtr.Value is DoGAAtrPaletteMaterial)) continue;
					var paletteMaterial = mapAtr.Value as DoGAAtrPaletteMaterial;
					var nameAtrInSuf = mapAtr.Key;

					var atr = new DoGAAtr(nameAtrInSuf, this.pathAtr);
					atr.setColor("amb", new Rgb(paletteMaterial.valAmb, paletteMaterial.valAmb, paletteMaterial.valAmb));
					atr.setColor("dif", new Rgb(paletteMaterial.valDif, paletteMaterial.valDif, paletteMaterial.valDif));
					atr.setColor("tra", new Rgb(paletteMaterial.valTra, paletteMaterial.valTra, paletteMaterial.valTra));
					atr.setColor("ref", new Rgb(paletteMaterial.valRef, paletteMaterial.valRef, paletteMaterial.valRef));
					atr.setColor("spc", new Rgb(paletteMaterial.valSp1, paletteMaterial.valSp1, paletteMaterial.valSp1));
					atr.setColor("spcsize", new Rgb(paletteMaterial.valSp2, paletteMaterial.valSp2, paletteMaterial.valSp2));
					atr.spcH = paletteMaterial.valSp3;

					mapAtr.Value.atr = atr;
				}
			}

			return true;
		}

		private bool associatePaletteRefToAtr()
		{
			foreach (var palette in this.palettes) {
				var nameCategory = palette.Key;
				var mapAtrs = palette.Value;
				foreach (var mapAtr in mapAtrs) {
					if (!(mapAtr.Value is DoGAAtrPaletteRef)) continue;
					var paletteRef = mapAtr.Value as DoGAAtrPaletteRef;
					var nameAtrInSuf = mapAtr.Key;

					var nameAtrColor = paletteRef.nameAtrColor;
					var nameAtrTexture = paletteRef.nameAtrTexture;
					var nameAtrMaterial = paletteRef.nameAtrMaterial;

					DoGAAtr atr;
					if (this.atrsGenie.ContainsKey(nameAtrInSuf.ToLower())) {
						atr = this.atrsGenie[nameAtrInSuf.ToLower()].CloneDeep();
					} else {
						atr = new DoGAAtr(nameAtrInSuf, this.pathAtr);
					}

					var atrRet = this.mergeAtrFromPalette(atr, nameAtrColor, nameAtrTexture, nameAtrMaterial);
					mapAtr.Value.atr = atrRet;
				}
			}

			return true;
		}

		private void extractMapAtr(Dictionary<string, string> profiles, string prefix, ref Dictionary<string, DoGAAtrPaletteBase> mapPalette, ref HashSet<string> setNamesFileAtr)
		{
			// Color1=C1,White,  color.atr,C1
			// Color14=C4,Light Purple, 0.95,0.64,0.86
			// Texture1=NO,None
			// T10,Camouflage 1,  meisai1.pic, texture.bmp,320,0,351,31

			var tablePostfix = new string[] { "s", "Max" };
			int numElems = 0;
			foreach (var postfix in tablePostfix) {
				var labelNum = prefix + postfix;
				if (profiles.ContainsKey(labelNum)) {
					numElems = int.Parse(profiles[labelNum]);
				}
			}
			if (numElems == 0) return;
			
			for (int index = 1; index <= numElems; index++) {
				var key = prefix + index.ToString();
				var value = profiles[key];
				var items = value.Split(',');
				if (items.Length < 4) { // Texture1=NO,None
					continue;
				}
				var nameAtrKey = items[0].Trim().ToLower();
				float valR = 0.0f, valG = 0.0f, valB = 0.0f;
				if (items.Length == 5 && float.TryParse(items[2], out valR) && float.TryParse(items[3], out valG) && float.TryParse(items[4], out valB)) { // Color14=C4,Light Purple, 0.95,0.64,0.86
					var atr = new DoGAAtr(nameAtrKey, this.pathAtr);
					atr.setColor("col", new Rgb(valR, valG, valB));
					var palette = new DoGAAtrPaletteFileAtr(nameAtrKey, atr);
					mapPalette.Add(nameAtrKey, palette);
				} else {

					var regexpAtr = new Regex(@"\.atr$");
					var regexpImage = new Regex(@"\.(pic|png|bmp|jpg)$");
					var item2 = items[2].Trim().ToLower();
					float valTra = 0.0f, valAmb = 0.0f, valDif = 0.0f, valSp1 = 0.0f, valSp2 = 0.0f, valSp3 = 0.0f, valRef = 0.0f;
					if (regexpAtr.IsMatch(item2)) {
						// Color1=C1,White,  color.atr,C1, ...
						var nameFileAtr = item2;
						var nameAtrInFileAtr = items[3].Trim().ToLower();
						if (!setNamesFileAtr.Contains(nameFileAtr)) {
							setNamesFileAtr.Add(nameFileAtr);
						}
						var palette = new DoGAAtrPaletteFileAtr(nameAtrKey.ToLower(), nameFileAtr, nameAtrInFileAtr);
						mapPalette.Add(nameAtrKey, palette);
					} else if (regexpImage.IsMatch(item2)) {
						var nameFileImage = item2;
						var palette = new DoGAAtrPaletteFileImage(nameAtrKey.ToLower(), nameFileImage);
						mapPalette.Add(nameAtrKey, palette);
					} else if (items.Length > 9 &&
								float.TryParse(item2, out valTra) && float.TryParse(items[3], out valAmb) &&
								float.TryParse(items[4], out valDif) &&
								float.TryParse(items[5], out valSp1) && float.TryParse(items[6], out valSp2) &&
								float.TryParse(items[7], out valSp3) &&
								float.TryParse(items[8], out valRef)) {
						var palette = new DoGAAtrPaletteMaterial(nameAtrKey.ToLower(), valTra, valAmb, valDif, valSp1, valSp2, valSp3, valRef);
						mapPalette.Add(nameAtrKey, palette);
					} else {
						// Attribute1=Rough_C1, "White", C1, Rough, M4, ...
						var nameAtrColorRef = items[2].Trim().ToLower();
						var nameAtrTextureRef = items[3].Trim().ToLower();
						var nameAtrMaterialRef = items[4].Trim().ToLower();
						var palette = new DoGAAtrPaletteRef(nameAtrColorRef, nameAtrTextureRef, nameAtrMaterialRef);
						mapPalette.Add(nameAtrKey, palette);
					}
				}
			}
            if (prefix == "Texture") {
				var nameAtrNo = "no";
				mapPalette.Add(nameAtrNo, new DoGAAtrPaletteFileAtr(nameAtrNo, new DoGAAtr(nameAtrNo, null)));
            }

//			return mapPalette;
		}

		static Dictionary<string, DoGAAtr> getMapAtrs(List<DoGAAtr> atrs)
		{
			var mapAtrs = new Dictionary<string, DoGAAtr>();
			foreach (var atr in atrs) {
				mapAtrs.Add(atr.name, atr);
			}
			return mapAtrs;
		}
	}
}
