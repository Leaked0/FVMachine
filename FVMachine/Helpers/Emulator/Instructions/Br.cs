using System;
using dnlib.DotNet.Emit;

namespace FVM.Helpers.Emulator.Instructions
{
	// Token: 0x02000027 RID: 39
	internal class Br : EmuInstruction
	{
		// Token: 0x17000013 RID: 19
		// (get) Token: 0x06000091 RID: 145 RVA: 0x0000BA63 File Offset: 0x00009C63
		internal override OpCode OpCode
		{
			get
			{
				return OpCodes.Br;
			}
		}

		// Token: 0x06000092 RID: 146 RVA: 0x0000BA6A File Offset: 0x00009C6A
		internal override void Emulate(EmuContext context, Instruction instr)
		{
			context.InstructionPointer = context.Instructions.IndexOf((Instruction)instr.Operand);
		}
	}
}
