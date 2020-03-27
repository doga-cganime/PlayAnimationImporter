using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Reflection;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using TotekanReaderLib;
using YanagisawaPicLoaderLib;

namespace TotekanReader
{
	class Program
	{
		static void testLoadE1P()
		{
//			var nameFileE1P = @"C:\Program Files (x86)\DoGA\PLAYAnimation\data\sample\01vehicle\01aircraft\airship.E1P";
			var nameFileE1P = @"C:\home\tomotaco-doga\Unity\work\data\unity3d\testdir\playanimation-sample\01vehicle\01aircraft\airship.E1P";

//			var nameFileE1P = @"D:\Program Files (x86)\DOGACGA\PLAYAnimation\data\sample\02life\01animal\unicorn.E1P";
			var context = TotekanParse.parseE1P(nameFileE1P);
			DoGAE1PVisitor visitor = new DoGAE1PVisitor(Path.GetDirectoryName(nameFileE1P));

			visitor.Visit(context as IParseTree);
			var listSufObjs = visitor.listSufObjs;
		}

		static void testLoadPartsAssemblerSubdir(string pathDir)
		{
			var patternsExtension = new string[] { "*.e1p", "*.l3p", "*.l2p", "*.fsc" };
			foreach (var patternExtension in patternsExtension) {
				var namesFile = Directory.GetFiles(pathDir, patternExtension);
				foreach (var nameFile in namesFile) {
					Console.WriteLine(@"=== Parsing " + nameFile + @"===");
					try {
						var context = TotekanParse.parseE1P(nameFile);
						DoGAE1PVisitor visitor = new DoGAE1PVisitor(pathDir);
						visitor.Visit(context as IParseTree);
						var listSufObjs = visitor.listSufObjs;
					} catch (Exception ex) {
						Console.WriteLine(@"Caught Exception: " + ex.Message);
						Console.WriteLine(@"StackTrace: " + ex.StackTrace);
					}

				}
			}
			var namesDir = Directory.GetDirectories(pathDir);
			foreach (var nameDir in namesDir) {
				testLoadPartsAssemblerSubdir(nameDir);
			}
		}

		static void testLoadL2P(DetectInstall detectInstall)
		{
			var nameFile = @"c:\home\tomotaco-doga\Unity\work\data\unity3d\testdir\dogal2-sample\mecha\l2_samp\human\gal1\GAL_1HD.L2P";
			var context = TotekanParse.parseE1P(nameFile);
			DoGAE1PVisitor visitor = new DoGAE1PVisitor(Path.GetDirectoryName(nameFile));

			visitor.Visit(context as IParseTree);
			var listSufObjs = visitor.listSufObjs;
			var mapAtrs = visitor.mapAtrsPalette;
		}

		static void testLoadL3P(DetectInstall detectInstall)
		{
//			var nameFileL3P = @"D:\home\tomotaco-doga\Unity\work\data\dogal3\l3_samp\parts\ball_particle_samp.L3P";
//			var nameFileL3P = @"C:\home\tomotaco-doga\Unity\work\data\unity3d\testdir\dogal3-sample\animation\demo1data\base.l3p";
//			var nameFileL3P = @"C:\home\tomotaco-doga\Unity\work\data\texture\doga-tank.l3p";
//			var nameFileL3P = @"D:\Home\tomotaco-doga\Unity\work\data\bugreport\20141018-texture-test\texture\doga-tank.l3p";
//			var nameFileL3P = @"C:\H\VRW\背景作成\objs\back\river.l3p";
//			var nameFileL3P = @"C:\H\VRW\背景作成\objs\back\riverbridge.l3p";
			var nameFileL3P = @"C:\H\VRW\背景作成\objs\back\tree.l3p";
			var context = TotekanParse.parseE1P(nameFileL3P);
			DoGAE1PVisitor visitor = new DoGAE1PVisitor(Path.GetDirectoryName(nameFileL3P));

			visitor.Visit(context as IParseTree);
			var listSufObjs = visitor.listSufObjs;
			var mapAtrs = visitor.mapAtrsPalette;

			var nameProduct = "DOGA-L3";
			var infoInstall = detectInstall.tableInfosInstall[nameProduct];
			var infoAtr = new DoGAAtrInfo(nameProduct, infoInstall.pathInstalled + infoInstall.pathRelativeExe, infoInstall.profiles);

//			var atr0 = infoAtr.getAtrFromPalette("bodym", "c20", "no", "m4");
			var atr0 = infoAtr.getAtrFromPalette("grayg", "#01", "no", "m4", mapAtrs);
			var pathAtr = atr0.path;

		}

		static void testLoadSuf()
		{
			//			var context = TotekanParse.parseSuf(@"D:\home\tomotaco-doga\Unity\work\data\pame\EG06.SUF");
			var tableNamesSuf = new string[] {
				/*
				@"BridgeFoot01.SUF",
				@"BridgeUp.SUF",
				*/
				@"Dote_20.suf",
				/*
				@"roadside09.SUF",
				@"sDote_m20.SUF",
				@"trees\t01.SUF",
				@"trees\t02.SUF",
				@"trees\t03.SUF",
				@"trees\t05.SUF",
				@"trees\t06.SUF",
				@"trees\t07.SUF"
				*/
			};
			foreach (var nameSuf in tableNamesSuf) {
				var context = TotekanParse.parseSuf(@"C:/H/VRW/背景作成/objs/back" + @"\" + nameSuf);
				var visitor = new DoGASufVisitor();
				visitor.Visit(context as IParseTree);
			}
		}

		static void testLoadAtr()
		{
			//			var nameFileAtr = @"c:\home\tomotaco-doga\Unity\work\data\pame\palette05.atr";
			var nameFileAtr = @"C:/H/VRW/背景作成/objs/back/Dote_20.atr";
			var context = TotekanParse.parseAtr(nameFileAtr);
			var visitor = new DoGAAtrVisitor(Path.GetDirectoryName(nameFileAtr));
			visitor.Visit(context as IParseTree);
			var atrs = visitor.atrs;
		}

		static void testLoadIni(string pathInstalled)
		{
//			var pathIni = pathInstalled + @"dogal3.ini";
			var pathIni = pathInstalled + @"..\common\atr\atr.ini";
			var profileIni = new IniProfile(pathIni);
		}

		static void testLoadPaletteAtr(DetectInstall detectInstall)
		{
			var nameProduct = @"TOTEKAN2";
			// var nameProduct = "PLAYAnimation";
			var infoInstall = detectInstall.tableInfosInstall[nameProduct];
			var infoAtr = new DoGAAtrInfo(nameProduct, infoInstall.pathInstalled + infoInstall.pathRelativeExe, infoInstall.profiles);

//          var atr = infoAtr.getAtrFromPalette("BodyM".ToLower(), "RGB_FFF716".ToLower(), "NO".ToLower(), "Mechanic".ToLower());			
//			var atr = infoAtr.getAtrFromPalette("bodym", "material02silver", "material02silver", "material02silver");		
//			var atr = infoAtr.getAtrFromPalette("bodym", "pattern02redcheck", "pattern02redcheck", "pattern02redcheck");
			//, nameAtrInSuf=
//			var atr = infoAtr.getAtrFromPalette("bodyd", "rgb_4f2b23", "rough", "m4");
//			var atr = infoAtr.getAtrFromPalette("bodym", null, null, null);
			var atr = infoAtr.getAtrFromPalette("grayg", "rgb_e1e1e1", "no", "m1");



			var path = atr.path;
			
		}

		static void testLoadPaletteAtr2(DetectInstall detectInstall)
		{
			var tableNamesProduct = new string[] {
				@"PLAYAnimation_ENU", 
				@"TOTEKAN2", 
				@"DOGA-E1", 
				@"DOGA-L3", 
				@"DOGA-L3PRO", 
				@"DOGA-L2", 
				// @"DOGA-L1"
			};
	


			foreach (var nameProduct in tableNamesProduct) {
				if (!detectInstall.tableInfosInstall.ContainsKey(nameProduct)) continue;
				var infoInstall = detectInstall.tableInfosInstall[nameProduct];
				var infoAtr = new DoGAAtrInfo(nameProduct, infoInstall.pathInstalled + infoInstall.pathRelativeExe, infoInstall.profiles);
			}
/*			var atr0 = infoAtr.getAtrFromPalette("bodym", "c20", "no", "m4");
			var atr1 = infoAtr.getAtrFromPalette("bodym", "c10b", "no", "m1"); // no texture
			var atr2 = infoAtr.getAtrFromPalette("bodym", "c10b", "t10", "m1"); // meisai
			var atr3 = infoAtr.getAtrFromPalette("bodym", "c10b", "t28", "m4"); // hana

			var name = atr0.name;
*/
		}

		static void testYanagisawaPic2()
		{
//			var bytes = File.ReadAllBytes(@"D:\Program Files (x86)\DOGACGA\DOGA-L3\common\atr\AMI1.PIC");
//			var bytes = File.ReadAllBytes(@"C:\Program Files (x86)\DoGA\DOGA-L3\common\atr\AMI1.PIC");
//			var bytes = File.ReadAllBytes(@"D:\Program Files (x86)\DOGACGA\DOGA-L3\common\atr\HANAGARA.PIC");
//			var bytes = File.ReadAllBytes(@"C:\Program Files (x86)\DoGA\DOGA-L3\common\atr\HANAGARA.PIC");
			var bytes = File.ReadAllBytes(@"C:\Program Files (x86)\DoGA\DOGA-L3\common\atr\BAKUHATU.PIC");
//			var bytes = File.ReadAllBytes(@"C:\Program Files (x86)\DoGA\DOGA-L3\common\atr\MEISAI1.PIC");
			var loader = new YanagisawaPicLoader(bytes, YanagisawaPicLoader.PixelOrder.BGRA, YanagisawaPicLoader.DirectionY.TopToBottom);

			var result = loader.extract();
			var bitmap = new Bitmap(loader.width, loader.height, PixelFormat.Format32bppArgb);
			var dataBitmap = bitmap.LockBits(new Rectangle(0, 0, loader.width, loader.height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
			Marshal.Copy(loader.bytesImageExtracted, 0, dataBitmap.Scan0, loader.bytesImageExtracted.Length);
			bitmap.UnlockBits(dataBitmap);

/*
			var bitmap = new Bitmap(loader.width, loader.height, PixelFormat.Format32bppArgb);

			for (var y = 0; y < loader.height; y++) {
				//				var dataBitmap = bitmap.LockBits(new Rectangle(0, y, loader.width, 1), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
				//				var result = loader.extractOneLine();
				//				Marshal.Copy(loader.bytesImageExtracted, 0, dataBitmap.Scan0, loader.bytesImageExtracted.Length);
				//				bitmap.UnlockBits(dataBitmap);
			}
			var dataBitmap = bitmap.LockBits(new Rectangle(0, 0, loader.width, loader.height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
			Marshal.Copy(loader.bytesImageExtracted, 0, dataBitmap.Scan0, loader.bytesImageExtracted.Length);
			bitmap.UnlockBits(dataBitmap);
*/
//			bitmap.Save(@"d:\temp\cap\out.bmp");
			bitmap.Save(@"c:\temp\cap\out.png");
		}


		[DllImport("kernel32", CharSet = CharSet.Ansi, SetLastError = true)]
		internal static extern IntPtr LoadLibrary(string lpFileName);
		[DllImport("kernel32", SetLastError = true)]
		internal static extern bool FreeLibrary(IntPtr hModule);
		[DllImport("kernel32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = false)]
		internal static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

		private delegate int DoGALxDLL_Get([In, MarshalAs(UnmanagedType.LPStr)] string product,
			[MarshalAs(UnmanagedType.LPStr)] StringBuilder name,
			[MarshalAs(UnmanagedType.LPStr)] StringBuilder id,
			[MarshalAs(UnmanagedType.LPStr)] StringBuilder pass);

		private delegate int DoGALxDLL_Check([In, MarshalAs(UnmanagedType.LPStr)] string product,
			[In, MarshalAs(UnmanagedType.LPStr)] string name,
			[In, MarshalAs(UnmanagedType.LPStr)] string id,
			[In, MarshalAs(UnmanagedType.LPStr)] string pass,
			[MarshalAs(UnmanagedType.U4)] uint limit);

		public static string FormatMessage(int code)
		{
			byte[] b = BitConverter.GetBytes(code);
			Array.Reverse(b);//リトルエンディアンなので
			string bs = "0x" + BitConverter.ToString(b, 0).Replace("-", "");

			return string.Format("{0}({1})", new System.ComponentModel.Win32Exception(code).Message, bs);
		}
		
		static void testAuth()
		{
			Assembly assembly = Assembly.GetExecutingAssembly();
			var path = Path.GetDirectoryName(assembly.Location) + Path.DirectorySeparatorChar;
			
			var tableNamesFileDll = new string[] {
//				path + @"DoGALxDll.dll",
//				@"D:\Program Files (x86)\DOGACGA\DoGA-L3\DoGALxDll_x86.dll",
				path + @"DoGALxDll_x86.dll",
//				path + @"DoGALxDll_x64.dll"
//				@"C:\Program Files (x86)\DoGA\DoGA-L2\dogal2\DOGALx.DLL",		// L2
//				@"C:\Program Files (x86)\DoGA\DoGA-L3\dogal3\DOGALx.DLL",		// L3
//				@"C:\Program Files (x86)\DoGA\DoGA-E1\exe\DOGALx.DLL",			// E1
//				@"C:\Program Files (x86)\DoGA\PLAYAnimation\exe\DOGALx.DLL"	// PlayAnimation
//				@"D:\Program Files (x86)\DOGACGA\DoGA-L2\dogal2\DOGALx.DLL",		// L2
//				@"D:\Program Files (x86)\DOGACGA\DoGA-L3\dogal3\DOGALx.DLL",		// L3
//				@"D:\Program Files (x86)\DOGACGA\DoGAE\exe\DOGALx.DLL",			// E1
//				@"D:\DOGACGA\PLAYAnimation\exe\DOGALx.DLL"	// PlayAnimation
			};

			var tableIdProductAuth = new string[] {
				@"PLAYAnimation_ENU",
				@"TOTEKAN2",
				@"DOGA-E1",
				@"DOGA-L3",
				@"DOGA-L3PRO",
				@"DOGA-L2",
				@"DOGAL1"
			};

			
			foreach (var nameFileDll in tableNamesFileDll) {
				Console.WriteLine("Dll: " + nameFileDll);

				int errorno = Marshal.GetLastWin32Error();
				var message = Program.FormatMessage(errorno);
				
				IntPtr handle = LoadLibrary(nameFileDll);

				errorno = Marshal.GetLastWin32Error();
				message = Program.FormatMessage(errorno);

				IntPtr pFuncCheck = GetProcAddress(handle, "Check");

				errorno = Marshal.GetLastWin32Error();
				message = Program.FormatMessage(errorno);
				
				IntPtr pFuncGet = GetProcAddress(handle, "Get");

				errorno = Marshal.GetLastWin32Error();
				message = Program.FormatMessage(errorno);
				
			
				var funcGet = Marshal.GetDelegateForFunctionPointer(pFuncGet, typeof(DoGALxDLL_Get)) as DoGALxDLL_Get;
				var funcCheck = Marshal.GetDelegateForFunctionPointer(pFuncCheck, typeof(DoGALxDLL_Check)) as DoGALxDLL_Check;

				foreach (var idProductAuth in tableIdProductAuth) {
					var name = new StringBuilder(1024);
					var id = new StringBuilder(1024);
					var pass = new StringBuilder(1024);

					int resultGet = funcGet(idProductAuth, name, id, pass);
					uint datetime = 0;
					int resultCheck = funcCheck(idProductAuth, name.ToString(), id.ToString(), pass.ToString(), datetime);

					Console.WriteLine("Product: " +  idProductAuth + ", " +
						"name: " + name.ToString() + ", id: " + id.ToString() + ", pass: " + pass.ToString() + ", " +
						"resultGet: " + resultGet + ", " +
						"resultCheck: " + resultCheck);
				}
				FreeLibrary(handle);
			}
		}

		static void Main(string[] args)
		{
//			testAuth();
//			return;
			var detectInstall = new DetectInstall();
			

			try {
//				testLoadE1P();
/*
				if (args.Length == 0) {
					Console.WriteLine("Usage: Program [dir]");
					return;
				}
				testLoadPartsAssemblerSubdir(args[0]);
*/
//				testLoadL2P(detectInstall);
				testLoadL3P(detectInstall);
//				testLoadSuf();
//				testLoadAtr();
//				testLoadIni(pathInstalled);

//				testLoadPaletteAtr(detectInstall);
//				testLoadPaletteAtr2(detectInstall);

//				testYanagisawaPic2();

			} catch (Exception ex) {
				Console.WriteLine("Error: " + ex);
			}
		}
	}
}
