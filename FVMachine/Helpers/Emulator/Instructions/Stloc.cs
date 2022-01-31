using System;
using dnlib.DotNet.Emit;

namespace FVM.Helpers.Emulator.Instructions
{
	// Token: 0x0200002B RID: 43
	internal class Stloc : EmuInstruction
	{
		// Token: 0x17000017 RID: 23
		// (get) Token: 0x0600009D RID: 157 RVA: 0x0000BB3D File Offset: 0x00009D3D
		internal override OpCode OpCode
		{
			get
			{
				return OpCodes.Stloc_S;
			}
		}

		// Token: 0x0600009E RID: 158 RVA: 0x0000BB44 File Offset: 0x00009D44
		internal override void Emulate(EmuContext context, Instruction instr)
		{
			object val = context.Stack.Pop();
			context.SetLocalValue((Local)instr.Operand, val);
		}
	}
}
