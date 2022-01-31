using System;
using System.Collections.Generic;
using dnlib.DotNet.Emit;

namespace FVM.Protections.ControlFlow
{
	// Token: 0x02000015 RID: 21
	internal interface IPredicate
	{
		// Token: 0x0600004E RID: 78
		void Init(CilBody body);

		// Token: 0x0600004F RID: 79
		void EmitSwitchLoad(IList<Instruction> instrs);

		// Token: 0x06000050 RID: 80
		int GetSwitchKey(int key);
	}
}
