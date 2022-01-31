using System;
using System.Collections.Generic;
using dnlib.DotNet.Emit;
using FVM.Helpers.Emulator;
using FVM.Helpers.MethodBlocks;

namespace FVM.Protections.Mutation
{
	// Token: 0x0200000B RID: 11
	public static class BlockHandler
	{
		// Token: 0x0600002C RID: 44 RVA: 0x0000747C File Offset: 0x0000567C
		public static Block GenerateBlock(Int32Local local, BlockType type)
		{
			List<Instruction> instructions = new List<Instruction>();
			if (type == BlockType.Arithmethic)
			{
				instructions.Add(OpCodes.Nop.ToInstruction());
				instructions.Add(OpCodes.Ldloc_S.ToInstruction(local.Local));
				instructions.Add(OpCodes.Ldc_I4.ToInstruction(Utils.RandomInt32()));
				instructions.Add(Utils.GetCode(false).ToOpCode().ToInstruction());
				instructions.Add(OpCodes.Stloc_S.ToInstruction(local.Local));
			}
			local.Value = BlockHandler.Emulate(instructions, local);
			return new Block(instructions.ToArray(), false, true, false, local.Value);
		}

		// Token: 0x0600002D RID: 45 RVA: 0x0000752C File Offset: 0x0000572C
		public static int Emulate(List<Instruction> instructions, Int32Local local)
		{
			Emulator emu = new Emulator(instructions, new List<Local>
			{
				local.Local
			});
			emu._context.SetLocalValue(local.Local, local.Value);
			return emu.Emulate();
		}

		// Token: 0x0600002E RID: 46 RVA: 0x0000757C File Offset: 0x0000577C
		public static List<Instruction> GenerateBranch(int value, Local local, Instruction target)
		{
			bool hasElse = Utils.RandomBoolean();
			int num = Utils.rnd.Next(0, 1);
			int num2 = num;
			List<Instruction> result;
			if (num2 != 0)
			{
				result = null;
			}
			else
			{
				result = BlockHandler.GenerateBrFalse(value, local, target, hasElse);
			}
			return result;
		}

		// Token: 0x0600002F RID: 47 RVA: 0x000075B8 File Offset: 0x000057B8
		public static List<Instruction> GenerateBrFalse(int value, Local local, Instruction target, bool fake)
		{
			int newValue = Utils.RandomInt32();
			Code code = Utils.GetCode(true);
			int r = BlockHandler.Calculate(value, newValue, code, false);
			List<Instruction> result = new List<Instruction>();
			Instruction nopTarget = OpCodes.Nop.ToInstruction();
			result.Add(OpCodes.Ldloc_S.ToInstruction(local));
			result.Add(OpCodes.Ldc_I4.ToInstruction(newValue));
			result.Add(code.ToOpCode().ToInstruction());
			result.Add(OpCodes.Ldc_I4.ToInstruction(r));
			result.Add(OpCodes.Ceq.ToInstruction());
			result.Add(OpCodes.Brfalse_S.ToInstruction(nopTarget));
			result.Add(OpCodes.Br.ToInstruction(target));
			result.Add(nopTarget);
			return result;
		}

		// Token: 0x06000030 RID: 48 RVA: 0x00007680 File Offset: 0x00005880
		public static List<Instruction> GenerateBrTrue(int value, Local local, Instruction target, bool fake)
		{
			int newValue = Utils.RandomInt32();
			Code code = Utils.GetCode(true);
			int r = BlockHandler.Calculate(value, newValue, code, false);
			List<Instruction> result = new List<Instruction>();
			Instruction brTarget = OpCodes.Br.ToInstruction(target);
			result.Add(OpCodes.Ldloc_S.ToInstruction(local));
			result.Add(OpCodes.Ldc_I4.ToInstruction(newValue));
			result.Add(code.ToOpCode().ToInstruction());
			result.Add(OpCodes.Ldc_I4.ToInstruction(r));
			result.Add(OpCodes.Ceq.ToInstruction());
			result.Add(OpCodes.Brtrue_S.ToInstruction(brTarget));
			result.Add(brTarget);
			return result;
		}

		// Token: 0x06000031 RID: 49 RVA: 0x00007734 File Offset: 0x00005934
		public static Block GenerateBrBlock(Instruction target)
		{
			return new Block(new List<Instruction>
			{
				OpCodes.Br.ToInstruction(target)
			}.ToArray(), false, true, true, -1);
		}

		// Token: 0x06000032 RID: 50 RVA: 0x00007770 File Offset: 0x00005970
		public static int Calculate(int n1, int n2, Code code, bool reverse = true)
		{
			int result;
			if (code != Code.Add)
			{
				if (code != Code.Sub)
				{
					if (code != Code.Xor)
					{
						result = 0;
					}
					else
					{
						result = (n1 ^ n2);
					}
				}
				else
				{
					result = (reverse ? (n1 + n2) : (n1 - n2));
				}
			}
			else
			{
				result = (reverse ? (n1 - n2) : (n1 + n2));
			}
			return result;
		}
	}
}
