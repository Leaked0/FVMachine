using System;
using System.IO;
using dnlib.DotNet;

namespace FVM.Helpers.DynConverter
{
	// Token: 0x02000031 RID: 49
	public static class Extension
	{
		// Token: 0x060000BB RID: 187 RVA: 0x0000C490 File Offset: 0x0000A690
		public static void ConvertToBytes(this BinaryWriter writer, MethodDef method)
		{
			new Converter(method, writer).ConvertToBytes();
		}
	}
}
