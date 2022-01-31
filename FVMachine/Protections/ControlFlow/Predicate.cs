using System;
using System.Collections.Generic;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace FVM.Protections.ControlFlow
{
	// Token: 0x02000016 RID: 22
	internal class Predicate : IPredicate
	{
		// Token: 0x06000051 RID: 81 RVA: 0x00008AA8 File Offset: 0x00006CA8
		public Predicate(ModuleDefMD ctx)
		{
			this.ctx = ctx;
		}

		// Token: 0x06000052 RID: 82 RVA: 0x00008ABC File Offset: 0x00006CBC
		public void Init(CilBody body)
		{
			bool flag = this.inited;
			if (!flag)
			{
				this.xorKey = new Random().Next();
				this.inited = true;
			}
		}

		// Token: 0x06000053 RID: 83 RVA: 0x00008AED File Offset: 0x00006CED
		public void EmitSwitchLoad(IList<Instruction> instrs)
		{
			instrs.Add(Instruction.Create(OpCodes.Ldc_I4, this.xorKey));
			instrs.Add(Instruction.Create(OpCodes.Xor));
		}

		// Token: 0x06000054 RID: 84 RVA: 0x00008B18 File Offset: 0x00006D18
		public int GetSwitchKey(int key)
		{
			return key ^ this.xorKey;
		}

		// Token: 0x0400001C RID: 28
		private readonly ModuleDefMD ctx;

		// Token: 0x0400001D RID: 29
		private bool inited;

		// Token: 0x0400001E RID: 30
		private int xorKey;
	}
}
