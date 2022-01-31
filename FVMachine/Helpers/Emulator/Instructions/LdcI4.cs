using System;
using dnlib.DotNet.Emit;

namespace FVM.Helpers.Emulator.Instructions
{
	// Token: 0x02000028 RID: 40
	internal class LdcI4 : EmuInstruction
	{
		// Token: 0x17000014 RID: 20
		// (get) Token: 0x06000094 RID: 148 RVA: 0x0000BA92 File Offset: 0x00009C92
		internal override OpCode OpCode
		{
			get
			{
				return OpCodes.Ldc_I4;
			}
		}

		// Token: 0x06000095 RID: 149 RVA: 0x0000BA99 File Offset: 0x00009C99
		internal override void Emulate(EmuContext context, Instruction instr)
		{
			context.Stack.Push(instr.Operand);
		}
	}
}
