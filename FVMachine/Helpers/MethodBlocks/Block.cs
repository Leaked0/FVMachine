using System;
using System.Collections.Generic;
using System.Linq;
using dnlib.DotNet.Emit;

namespace FVM.Helpers.MethodBlocks
{
	// Token: 0x02000019 RID: 25
	public class Block
	{
		// Token: 0x06000063 RID: 99 RVA: 0x00009FCC File Offset: 0x000081CC
		public Block(Instruction[] Instructions, bool IsException = false, bool IsSafe = true, bool IsBranched = false, int initValue = -1)
		{
			this.Instructions = Instructions.ToList<Instruction>();
			this.IsSafe = IsSafe;
			this.IsBranched = IsBranched;
			this.IsException = IsException;
			this.Values = new List<int>
			{
				initValue
			};
		}

		// Token: 0x06000064 RID: 100 RVA: 0x0000A00C File Offset: 0x0000820C
		public static Block Clone(Block block, bool all = false)
		{
			List<Instruction> instructions = new List<Instruction>();
			foreach (Instruction instr in block.Instructions)
			{
				instructions.Add(new Instruction
				{
					OpCode = instr.OpCode,
					Operand = instr.Operand
				});
				bool flag = !all && instr.OpCode == OpCodes.Stloc_S;
				if (flag)
				{
					break;
				}
			}
			return new Block(instructions.ToArray(), block.IsException, block.IsSafe, block.IsBranched, -1);
		}

		// Token: 0x04000021 RID: 33
		public List<Instruction> Instructions;

		// Token: 0x04000022 RID: 34
		public bool IsSafe;

		// Token: 0x04000023 RID: 35
		public bool IsBranched;

		// Token: 0x04000024 RID: 36
		public bool IsException;

		// Token: 0x04000025 RID: 37
		public List<int> Values;
	}
}
