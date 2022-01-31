using System;
using dnlib.DotNet.Emit;

namespace FVM.Helpers.Emulator.Instructions
{
	// Token: 0x02000029 RID: 41
	internal class Ldloc : EmuInstruction
	{
		// Token: 0x17000015 RID: 21
		// (get) Token: 0x06000097 RID: 151 RVA: 0x0000BAB7 File Offset: 0x00009CB7
		internal override OpCode OpCode
		{
			get
			{
				return OpCodes.Ldloc_S;
			}
		}

		// Token: 0x06000098 RID: 152 RVA: 0x0000BABE File Offset: 0x00009CBE
		internal override void Emulate(EmuContext context, Instruction instr)
		{
			context.Stack.Push(context.GetLocalValue((Local)instr.Operand));
		}
	}
}
