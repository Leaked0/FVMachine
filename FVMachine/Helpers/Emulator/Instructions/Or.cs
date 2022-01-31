using System;
using dnlib.DotNet.Emit;

namespace FVM.Helpers.Emulator.Instructions
{
	// Token: 0x0200002A RID: 42
	internal class Or : EmuInstruction
	{
		// Token: 0x17000016 RID: 22
		// (get) Token: 0x0600009A RID: 154 RVA: 0x0000BAE7 File Offset: 0x00009CE7
		internal override OpCode OpCode
		{
			get
			{
				return OpCodes.Or;
			}
		}

		// Token: 0x0600009B RID: 155 RVA: 0x0000BAF0 File Offset: 0x00009CF0
		internal override void Emulate(EmuContext context, Instruction instr)
		{
			int right = (int)context.Stack.Pop();
			int left = (int)context.Stack.Pop();
			context.Stack.Push(left | right);
		}
	}
}
