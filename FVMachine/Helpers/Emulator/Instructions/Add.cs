using System;
using dnlib.DotNet.Emit;

namespace FVM.Helpers.Emulator.Instructions
{
	// Token: 0x02000024 RID: 36
	internal class Add : EmuInstruction
	{
		// Token: 0x17000010 RID: 16
		// (get) Token: 0x06000088 RID: 136 RVA: 0x0000B955 File Offset: 0x00009B55
		internal override OpCode OpCode
		{
			get
			{
				return OpCodes.Add;
			}
		}

		// Token: 0x06000089 RID: 137 RVA: 0x0000B95C File Offset: 0x00009B5C
		internal override void Emulate(EmuContext context, Instruction instr)
		{
			int right = (int)context.Stack.Pop();
			int left = (int)context.Stack.Pop();
			context.Stack.Push(left + right);
		}
	}
}
