using System;
using dnlib.DotNet.Emit;

namespace FVM.Helpers.Emulator.Instructions
{
	// Token: 0x0200002D RID: 45
	internal class Xor : EmuInstruction
	{
		// Token: 0x17000019 RID: 25
		// (get) Token: 0x060000A3 RID: 163 RVA: 0x0000BBD1 File Offset: 0x00009DD1
		internal override OpCode OpCode
		{
			get
			{
				return OpCodes.Xor;
			}
		}

		// Token: 0x060000A4 RID: 164 RVA: 0x0000BBD8 File Offset: 0x00009DD8
		internal override void Emulate(EmuContext context, Instruction instr)
		{
			int right = (int)context.Stack.Pop();
			int left = (int)context.Stack.Pop();
			context.Stack.Push(left ^ right);
		}
	}
}
