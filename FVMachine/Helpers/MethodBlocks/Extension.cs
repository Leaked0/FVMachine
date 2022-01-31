using System;
using System.Collections.Generic;
using System.Linq;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace FVM.Helpers.MethodBlocks
{
	// Token: 0x0200001F RID: 31
	public static class Extension
	{
		// Token: 0x06000079 RID: 121 RVA: 0x0000AE00 File Offset: 0x00009000
		public static List<Block> GetBlocks(this MethodDef method)
		{
			Extension.<>c__DisplayClass0_0 CS$<>8__locals1 = new Extension.<>c__DisplayClass0_0();
			List<Block> blocks = new List<Block>();
			CilBody body = method.Body;
			body.SimplifyBranches();
			BlockParser.ScopeBlock root = BlockParser.ParseBody(body);
			List<Instruction> branchesInstructions = new List<Instruction>();
			IList<ExceptionHandler> exceptions = method.Body.ExceptionHandlers;
			int exceptionsCount = 0;
			CS$<>8__locals1.trace = new Trace(body, method.ReturnType.RemoveModifiers().ElementType != ElementType.Void);
			using (IEnumerator<BlockParser.InstrBlock> enumerator = Extension.GetAllBlocks(root).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Extension.<>c__DisplayClass0_1 CS$<>8__locals2 = new Extension.<>c__DisplayClass0_1();
					CS$<>8__locals2.CS$<>8__locals1 = CS$<>8__locals1;
					CS$<>8__locals2.block = enumerator.Current;
					Extension.<>c__DisplayClass0_2 CS$<>8__locals3 = new Extension.<>c__DisplayClass0_2();
					CS$<>8__locals3.CS$<>8__locals2 = CS$<>8__locals2;
					CS$<>8__locals3.statements = Extension.SplitStatements(CS$<>8__locals3.CS$<>8__locals2.block, CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.trace);
					foreach (Instruction[] statement in CS$<>8__locals3.statements)
					{
						Extension.<>c__DisplayClass0_3 CS$<>8__locals4 = new Extension.<>c__DisplayClass0_3();
						CS$<>8__locals4.CS$<>8__locals3 = CS$<>8__locals3;
						CS$<>8__locals4.statementLast = new HashSet<Instruction>(from st in CS$<>8__locals4.CS$<>8__locals3.statements
						select st.Last<Instruction>());
						int finished = 0;
						int finishedExceptions = 0;
						Instruction[] array = statement;
						for (int i = 0; i < array.Length; i++)
						{
							Instruction instr = array[i];
							bool flag = instr.Operand is Instruction;
							if (flag)
							{
								branchesInstructions.Add(instr.Operand as Instruction);
							}
							bool flag2 = branchesInstructions.Contains(instr);
							if (flag2)
							{
								branchesInstructions.Remove(instr);
								finished++;
							}
							bool flag3 = exceptions.Count > 1;
							if (!flag3)
							{
								bool flag4 = exceptions.Any((ExceptionHandler x) => x.TryStart == instr);
								if (flag4)
								{
									exceptionsCount++;
								}
								bool flag5 = exceptions.Any((ExceptionHandler x) => x.HandlerEnd == instr);
								if (flag5)
								{
									finishedExceptions++;
									exceptionsCount--;
								}
							}
						}
						bool isInException = exceptionsCount > 0 || finishedExceptions > 0;
						bool isInBranch = branchesInstructions.Count > 0 || finished > 0;
						blocks.Add(new Block(statement, isInException, !CS$<>8__locals4.<GetBlocks>g__hasUnknownSource|1(statement), isInBranch, -1));
					}
				}
			}
			return blocks;
		}

		// Token: 0x0600007A RID: 122 RVA: 0x0000B0E8 File Offset: 0x000092E8
		private static LinkedList<Instruction[]> SplitStatements(BlockParser.InstrBlock block, Trace trace)
		{
			LinkedList<Instruction[]> statements = new LinkedList<Instruction[]>();
			List<Instruction> currentStatement = new List<Instruction>();
			HashSet<Instruction> requiredInstr = new HashSet<Instruction>();
			for (int i = 0; i < block.Instructions.Count; i++)
			{
				Instruction instr = block.Instructions[i];
				currentStatement.Add(instr);
				bool shouldSplit = i + 1 < block.Instructions.Count && trace.HasMultipleSources(block.Instructions[i + 1].Offset);
				FlowControl flowControl = instr.OpCode.FlowControl;
				FlowControl flowControl2 = flowControl;
				if (flowControl2 == FlowControl.Branch || flowControl2 == FlowControl.Cond_Branch || flowControl2 - FlowControl.Return <= 1)
				{
					shouldSplit = true;
					bool flag = trace.AfterStack[instr.Offset] != 0;
					if (flag)
					{
						Instruction targetInstr = instr.Operand as Instruction;
						bool flag2 = targetInstr != null;
						if (flag2)
						{
							requiredInstr.Add(targetInstr);
						}
						else
						{
							Instruction[] targetInstrs = instr.Operand as Instruction[];
							bool flag3 = targetInstrs != null;
							if (flag3)
							{
								foreach (Instruction target in targetInstrs)
								{
									requiredInstr.Add(target);
								}
							}
						}
					}
				}
				requiredInstr.Remove(instr);
				bool flag4 = instr.OpCode.OpCodeType != OpCodeType.Prefix && trace.AfterStack[instr.Offset] == 0 && requiredInstr.Count == 0 && (shouldSplit || 90.0 > new Random().NextDouble()) && (i == 0 || block.Instructions[i - 1].OpCode.Code != Code.Tailcall);
				if (flag4)
				{
					statements.AddLast(currentStatement.ToArray());
					currentStatement.Clear();
				}
			}
			bool flag5 = currentStatement.Count > 0;
			if (flag5)
			{
				statements.AddLast(currentStatement.ToArray());
			}
			return statements;
		}

		// Token: 0x0600007B RID: 123 RVA: 0x0000B2DD File Offset: 0x000094DD
		private static IEnumerable<BlockParser.InstrBlock> GetAllBlocks(BlockParser.ScopeBlock scope)
		{
			foreach (BlockParser.BlockBase child in scope.Children)
			{
				bool flag = child is BlockParser.InstrBlock;
				if (flag)
				{
					yield return (BlockParser.InstrBlock)child;
				}
				else
				{
					foreach (BlockParser.InstrBlock block in Extension.GetAllBlocks((BlockParser.ScopeBlock)child))
					{
						yield return block;
						block = null;
					}
					IEnumerator<BlockParser.InstrBlock> enumerator2 = null;
				}
				child = null;
			}
			List<BlockParser.BlockBase>.Enumerator enumerator = default(List<BlockParser.BlockBase>.Enumerator);
			yield break;
			yield break;
		}
	}
}
