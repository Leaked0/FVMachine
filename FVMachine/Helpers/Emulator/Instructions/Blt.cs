using System;
using dnlib.DotNet.Emit;

namespace FVM.Helpers.Emulator.Instructions
{
	// Token: 0x02000026 RID: 38
	internal class Blt : EmuInstruction
	{
		// Token: 0x17000012 RID: 18
		// (get) Token: 0x0600008E RID: 142 RVA: 0x0000B9FD File Offset: 0x00009BFD
		internal override OpCode OpCode
		{
			get
			{
				return OpCodes.Blt;
			}
		}

		// Token: 0x0600008F RID: 143 RVA: 0x0000BA04 File Offset: 0x00009C04
		internal override void Emulate(EmuContext context, Instruction instr)
		{
			int right = (int)context.Stack.Pop();
			int left = (int)context.Stack.Pop();
			bool flag = left < right;
			if (flag)
			{
				context.InstructionPointer = context.Instructions.IndexOf((Instruction)instr.Operand);
			}
		}
	}
}
