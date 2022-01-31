using System;
using dnlib.DotNet.Emit;

namespace FVM.Helpers.Emulator
{
	// Token: 0x02000022 RID: 34
	internal abstract class EmuInstruction
	{
		// Token: 0x1700000F RID: 15
		// (get) Token: 0x06000083 RID: 131
		internal abstract OpCode OpCode { get; }

		// Token: 0x06000084 RID: 132
		internal abstract void Emulate(EmuContext context, Instruction instr);
	}
}
