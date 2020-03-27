#define DOGALX_DLL_LOADLIBRARY
#define DOGALX_DLL_CHECK_USER_REGISTRATION

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using System.Reflection;


namespace TotekanReaderLib
{
	public class DetectInstall
	{
#if DOGALX_DLL_LOADLIBRARY
		[DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
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
#else
		[DllImport("DOGALx.DLL")]
		internal static extern int Get([In, MarshalAs(UnmanagedType.LPStr)] String product,
			[MarshalAs(UnmanagedType.LPStr)] StringBuilder name,
			[MarshalAs(UnmanagedType.LPStr)] StringBuilder id,
			[MarshalAs(UnmanagedType.LPStr)] StringBuilder pass);

		[DllImport("DoGALx.DLL")]
		internal static extern int Check([In, MarshalAs(UnmanagedType.LPStr)] String product,
			[In, MarshalAs(UnmanagedType.LPStr)] String name,
			[In, MarshalAs(UnmanagedType.LPStr)] String id,
			[In, MarshalAs(UnmanagedType.LPStr)] String pass,
			[MarshalAs(UnmanagedType.U4)] uint limit);
#endif
	
		public Dictionary<string, InstallInfo> tableInfosInstall = new Dictionary<string, InstallInfo>();
		private Dictionary<string, ProductInfo> tableInfosProduct = new Dictionary<string, ProductInfo>();

		public DetectInstall()
		{
			this.tableInfosProduct.Add(@"PLAYAnimation_ENU", new ProductInfo(@"PLAYAnimation", @"PLAYAnimation_ENU", @"PLAY Animation", @"exe\", @"Totekan2.ini", @"DOGA-E1", ".e1p"));
			this.tableInfosProduct.Add(@"TOTEKAN2", new ProductInfo(@"DOGA-E1T", @"TOTEKAN2", @"TotekanCG", @"exe\", @"Totekan2.ini", @"DOGA-E1", ".e1p"));
			this.tableInfosProduct.Add(@"DOGA-E1", new ProductInfo(@"DOGA-E1", @"DOGA-E1", @"DOGA-E1", @"exe\", @"dogae1.ini", @"DOGA-E1", ".e1p"));
			this.tableInfosProduct.Add(@"DOGA-L3", new ProductInfo(@"DOGAL3", @"DOGA-L3", @"DOGA-L3", @"dogal3\", @"dogal3.ini", @"DOGA-L3", ".l3p"));
			this.tableInfosProduct.Add(@"DOGA-L3PRO", new ProductInfo(@"DOGAL3", @"DOGA-L3PRO", @"DOGA-L3(Pro)", @"dogal3\", @"dogal3.ini", @"DOGA-L3", ".l3p"));
			this.tableInfosProduct.Add(@"DOGA-L2", new ProductInfo(@"DOGAL2", @"DOGA-L2", @"DOGA-L2", @"dogal2\", @"dogal2.ini", @"DOGA-L2", ".l2p"));
			this.tableInfosProduct.Add(@"DOGA-L1", new ProductInfo(@"DOGAL1", @"DOGA-L1", @"DOGA-L1", @"dogal1\", @"dogal1.ini", @"DOGA-L1", ".fsc"));

			this.retrieveRegistry();
		}

		public IEnumerable<string> getIdProductInstalled()
		{
			return this.tableInfosInstall.Keys;
		}

		public bool getIsInstalled(string idProduct)
		{
			return this.tableInfosInstall.ContainsKey(idProduct) ? true : false;
		}

		public List<string> getIdsProductByExtPartAsm(string nameExt)
		{
			var namesProduct = new List<string>();
			foreach (var pairInfoInstall in this.tableInfosInstall) {
				if (pairInfoInstall.Value.nameExtPartsAsm == nameExt) {
					namesProduct.Add(pairInfoInstall.Key);
				}
			}
			return namesProduct;
		}

		public Dictionary<string, string> getProfilesIni(string idProduct)
		{
			if (!this.tableInfosInstall.ContainsKey(idProduct)) return null;
			return this.tableInfosInstall[idProduct].profiles;
		}

		private bool retrieveRegistry()
		{
			var tablePathRagistry = new string[] {
				@"SOFTWARE\Wow6432Node\",
				@"SOFTWARE\"
			};
			var tableIdsProduct = this.tableInfosProduct.Keys;

			foreach (var pairInfoProduct in this.tableInfosProduct) {
				var infoProduct = pairInfoProduct.Value;
				var idProductAuth = pairInfoProduct.Key;
				var idProductUnInstall = infoProduct.idProductUnInstall;
				var nameKey = @"DOGA-" + idProductUnInstall;
				foreach (var pathRegistry in tablePathRagistry) {
					var builder = new StringBuilder();
					builder.Append(pathRegistry);
					builder.Append(@"Microsoft\Windows\CurrentVersion\Uninstall\");
					builder.Append(nameKey);

					var key = Registry.LocalMachine.OpenSubKey(builder.ToString());
					if (key == null) continue;
					var pathUnInstall = (string)key.GetValue("UninstallString");
					if (pathUnInstall == null) continue;

					var regexp = new Regex(@"uninst[\w\d]*\.exe");
					var matched = regexp.Match(pathUnInstall);
					int indexUninstallExe = matched.Index;
					var pathInstalled = pathUnInstall.Substring(0, indexUninstallExe);
#if DOGALX_DLL_CHECK_USER_REGISTRATION
					if (!checkUserRegistration(pathInstalled, infoProduct)) continue;
#endif
					var infoInstall = new InstallInfo(pathInstalled, infoProduct);
					this.tableInfosInstall.Add(idProductAuth, infoInstall);
					break;
				}
			}
			return true;
		}

		private bool checkUserRegistration(string pathInstalled, ProductInfo infoProduct)
		{
			var idProductAuth = infoProduct.idProductAuth;
			// L1 は登録不要のため有効
			if (idProductAuth == "DOGA-L1") return true;
			// L2 は L3/L3PRO が入っているときに有効
			if (idProductAuth == "DOGA-L2") {
				if (this.tableInfosInstall.ContainsKey("DOGA-L3") || this.tableInfosInstall.ContainsKey("DOGA-L3PRO")) return true;
			}
			Assembly assembly = Assembly.GetExecutingAssembly();
			var path = Path.GetDirectoryName(assembly.Location) + Path.DirectorySeparatorChar;
			bool is64bit = (IntPtr.Size == 8) ? true : false;
			var nameFileDll = is64bit ? @"DoGALxDll_x64.dll" : @"DoGALxDll_x86.dll";

#if DOGALX_DLL_LOADLIBRARY
//			var pathDllDoGA = pathInstalled + infoProduct.pathRelativeExe + @"DoGALx.DLL";
			var pathDllDoGA = path + nameFileDll;
			IntPtr handle = LoadLibrary(pathDllDoGA);
			if (handle == null) return false;
			IntPtr pFuncCheck = GetProcAddress(handle, "Check");
			IntPtr pFuncGet = GetProcAddress(handle, "Get");
			if (!(pFuncCheck != IntPtr.Zero && pFuncGet != IntPtr.Zero)) return false;

			var funcGet = Marshal.GetDelegateForFunctionPointer(pFuncGet, typeof(DoGALxDLL_Get)) as DoGALxDLL_Get;
			var funcCheck = Marshal.GetDelegateForFunctionPointer(pFuncCheck, typeof(DoGALxDLL_Check)) as DoGALxDLL_Check;
#endif
			var name = new StringBuilder(1024);
			var id = new StringBuilder(1024);
			var pass = new StringBuilder(1024);

#if DOGALX_DLL_LOADLIBRARY
			int resultGet = funcGet(idProductAuth, name, id, pass);
			uint datetime = 0;
			int resultCheck = funcCheck(idProductAuth, name.ToString(), id.ToString(), pass.ToString(), datetime);
			FreeLibrary(handle);
#else
			int resultGet = DetectInstall.Get(idProductAuth, name, id, pass);
			uint datetime = 0;
			int resultCheck = DetectInstall.Check(idProductAuth, name.ToString(), id.ToString(), pass.ToString(), datetime);
#endif
			var strName = name.ToString();
			var strId = id.ToString();
			var strPass = pass.ToString();

			return (resultCheck == 1) ? true : false;
		}
	}

}
