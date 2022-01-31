using System;
using dnlib.DotNet.Emit;

namespace FVM.Helpers.Emulator.Instructions
{
	// Token: 0x0200002C RID: 44
	internal class Sub : EmuInstruction
	{
		// Token: 0x17000018 RID: 24
		// (get) Token: 0x060000A0 RID: 160 RVA: 0x0000BB7A File Offset: 0x00009D7A
		internal override OpCode OpCode
		{
			get
			{
				return OpCodes.Sub;
			}
		}

		// Token: 0x060000A1 RID: 161 RVA: 0x0000BB84 File Offset: 0x00009D84
		internal override void Emulate(EmuContext context, Instruction instr)
		{
			int right = (int)context.Stack.Pop();
			int left = (int)context.Stack.Pop();
			context.Stack.Push(left - right);
		}
	}
}
