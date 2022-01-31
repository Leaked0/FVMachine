using System;
using dnlib.DotNet.Emit;

namespace FVM.Protections.Mutation
{
	// Token: 0x0200000D RID: 13
	public class Int32Local
	{
		// Token: 0x06000033 RID: 51 RVA: 0x000077BE File Offset: 0x000059BE
		public Int32Local(Local local, int value)
		{
			this.Local = local;
			this.Value = value;
		}

		// Token: 0x0400000C RID: 12
		public Local Local;

		// Token: 0x0400000D RID: 13
		public int Value;
	}
}
