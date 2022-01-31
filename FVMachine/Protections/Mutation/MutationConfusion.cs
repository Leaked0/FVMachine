using System;
using System.Collections.Generic;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using FVM.Helpers.MethodBlocks;

namespace FVM.Protections.Mutation
{
	// Token: 0x0200000E RID: 14
	public class MutationConfusion
	{
		// Token: 0x06000034 RID: 52 RVA: 0x000077D8 File Offset: 0x000059D8
		public static void PhaseMutation(MethodDef method, MethodDef cctor, ModuleDefMD module)
		{
			List<Instruction> instructions = new List<Instruction>();
			List<Block> currentBlocks = method.GetBlocks();
			List<Block> finalBlocks = new List<Block>();
			List<Block> mutationBlocks = new List<Block>();
			Local initLocal = new Local(module.CorLibTypes.Int32);
			int initValue = Utils.RandomBigInt32();
			Int32Local local = new Int32Local(initLocal, initValue);
			FieldDefUser field = Utils.CreateField(new FieldSig(module.CorLibTypes.Int32));
			IList<Instruction> cctorBody = cctor.Body.Instructions;
			module.GlobalType.Fields.Add(field);
			cctorBody.Insert(0, OpCodes.Ldc_I4.ToInstruction(initValue));
			cctorBody.Insert(1, OpCodes.Stsfld.ToInstruction(field));
			method.Body.Variables.Add(initLocal);
			instructions.Add(OpCodes.Nop.ToInstruction());
			instructions.Add(OpCodes.Ldsfld.ToInstruction(field));
			instructions.Add(OpCodes.Stloc_S.ToInstruction(initLocal));
			Block initBlock = new Block(instructions.ToArray(), false, true, false, -1);
			finalBlocks.Add(initBlock);
			foreach (Block block in currentBlocks)
			{
				bool flag = block.IsSafe && !block.IsBranched && !block.IsException;
				if (flag)
				{
					bool tried = false;
					Block randMutationBlock;
					int result;
					for (;;)
					{
						bool flag2 = Utils.RandomBoolean() || mutationBlocks.Count == 0 || tried;
						if (flag2)
						{
							goto Block_8;
						}
						int index = Utils.rnd.Next(0, mutationBlocks.Count);
						randMutationBlock = mutationBlocks[index];
						result = BlockHandler.Emulate(randMutationBlock.Instructions, local);
						bool flag3 = randMutationBlock.Values.Contains(result);
						if (!flag3)
						{
							goto IL_1F5;
						}
						tried = true;
					}
					IL_261:
					goto IL_262;
					IL_1F5:
					local.Value = result;
					randMutationBlock.Values.Add(result);
					List<Instruction> branch = BlockHandler.GenerateBranch(local.Value, initLocal, block.Instructions[0]);
					randMutationBlock.Instructions.AddRange(branch);
					block.Instructions.Insert(0, OpCodes.Br.ToInstruction(randMutationBlock.Instructions[0]));
					goto IL_261;
					Block_8:
					for (int i = 0; i < Utils.rnd.Next(2, 5); i++)
					{
						Block mutationBlock = BlockHandler.GenerateBlock(local, BlockType.Arithmethic);
						finalBlocks.Add(mutationBlock);
						mutationBlocks.Add(mutationBlock);
					}
				}
				IL_262:
				for (int j = 0; j < block.Instructions.Count; j++)
				{
					Instruction instr = block.Instructions[j];
					bool flag4 = !instr.IsLdcI4();
					if (!flag4)
					{
						int value = instr.GetLdcI4Value();
						int currentValue = local.Value;
						Code code = Utils.GetCode(true);
						int replaceValue = BlockHandler.Calculate(value, currentValue, code, true);
						instr.OpCode = OpCodes.Ldc_I4;
						instr.Operand = replaceValue;
						block.Instructions.Insert(j + 1, OpCodes.Ldloc_S.ToInstruction(initLocal));
						block.Instructions.Insert(j + 2, code.ToOpCode().ToInstruction());
						j += 2;
					}
				}
				finalBlocks.Add(block);
			}
			method.Body.Instructions.Clear();
			foreach (Block block2 in finalBlocks)
			{
				foreach (Instruction instr2 in block2.Instructions)
				{
					method.Body.Instructions.Add(instr2);
				}
			}
		}
	}
}
