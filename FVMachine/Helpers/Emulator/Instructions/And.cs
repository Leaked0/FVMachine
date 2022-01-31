using System;
using dnlib.DotNet.Emit;

namespace FVM.Helpers.Emulator.Instructions
{
	// Token: 0x02000025 RID: 37
	internal class And : EmuInstruction
	{
		// Token: 0x17000011 RID: 17
		// (get) Token: 0x0600008B RID: 139 RVA: 0x0000B9A9 File Offset: 0x00009BA9
		internal override OpCode OpCode
		{
			get
			{
				return OpCodes.And;
			}
		}

		// Token: 0x0600008C RID: 140 RVA: 0x0000B9B0 File Offset: 0x00009BB0
		internal override void Emulate(EmuContext context, Instruction instr)
		{
			int right = (int)context.Stack.Pop();
			int left = (int)context.Stack.Pop();
			context.Stack.Push(left & right);
		}
	}
}
