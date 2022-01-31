using System;
using System.Collections.Generic;
using System.Linq;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace FVM.Protections.ControlFlow
{
	// Token: 0x02000017 RID: 23
	internal class SwitchMangler : ManglerBase
	{
		// Token: 0x17000009 RID: 9
		// (get) Token: 0x06000055 RID: 85 RVA: 0x00008B32 File Offset: 0x00006D32
		// (set) Token: 0x06000056 RID: 86 RVA: 0x00008B3A File Offset: 0x00006D3A
		public ModuleDefMD ctx { get; set; }

		// Token: 0x06000057 RID: 87 RVA: 0x00008B44 File Offset: 0x00006D44
		private static OpCode InverseBranch(OpCode opCode)
		{
			OpCode result;
			switch (opCode.Code)
			{
			case Code.Brfalse:
				result = OpCodes.Brtrue;
				break;
			case Code.Brtrue:
				result = OpCodes.Brfalse;
				break;
			case Code.Beq:
				result = OpCodes.Bne_Un;
				break;
			case Code.Bge:
				result = OpCodes.Blt;
				break;
			case Code.Bgt:
				result = OpCodes.Ble;
				break;
			case Code.Ble:
				result = OpCodes.Bgt;
				break;
			case Code.Blt:
				result = OpCodes.Bge;
				break;
			case Code.Bne_Un:
				result = OpCodes.Beq;
				break;
			case Code.Bge_Un:
				result = OpCodes.Blt_Un;
				break;
			case Code.Bgt_Un:
				result = OpCodes.Ble_Un;
				break;
			case Code.Ble_Un:
				result = OpCodes.Bgt_Un;
				break;
			case Code.Blt_Un:
				result = OpCodes.Bge_Un;
				break;
			default:
				throw new NotSupportedException();
			}
			return result;
		}

		// Token: 0x06000058 RID: 88 RVA: 0x00008C00 File Offset: 0x00006E00
		public override void Mangle(CilBody body, BlockParser.ScopeBlock root, Context ctx, MethodDef Method, TypeSig retType)
		{
			SwitchMangler.<>c__DisplayClass7_0 CS$<>8__locals1 = new SwitchMangler.<>c__DisplayClass7_0();
			this.ctx = ctx.Module;
			CS$<>8__locals1.trace = new SwitchMangler.Trace(body, retType.RemoveModifiers().ElementType != ElementType.Void);
			Local local = new Local(Method.Module.CorLibTypes.UInt32);
			Local arraylocal = new Local(Method.Module.ImportAsTypeSig(typeof(uint[])));
			body.Variables.Add(arraylocal);
			body.Variables.Add(local);
			body.InitLocals = true;
			body.MaxStack += 2;
			IPredicate predicate = new Predicate(ctx.Module);
			using (IEnumerator<BlockParser.InstrBlock> enumerator = ManglerBase.GetAllBlocks(root).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SwitchMangler.<>c__DisplayClass7_1 CS$<>8__locals2 = new SwitchMangler.<>c__DisplayClass7_1();
					CS$<>8__locals2.CS$<>8__locals1 = CS$<>8__locals1;
					CS$<>8__locals2.block = enumerator.Current;
					LinkedList<Instruction[]> statements = this.SplitStatements(CS$<>8__locals2.block, CS$<>8__locals2.CS$<>8__locals1.trace);
					bool isInstanceConstructor = Method.IsInstanceConstructor;
					if (isInstanceConstructor)
					{
						List<Instruction> newStatement = new List<Instruction>();
						while (statements.First != null)
						{
							newStatement.AddRange(statements.First.Value);
							Instruction lastInstr = statements.First.Value.Last<Instruction>();
							statements.RemoveFirst();
							bool flag = lastInstr.OpCode == OpCodes.Call && ((IMethod)lastInstr.Operand).Name == ".ctor";
							if (flag)
							{
								break;
							}
						}
						statements.AddFirst(newStatement.ToArray());
					}
					bool flag2 = statements.Count < 3;
					if (!flag2)
					{
						int[] keyId = Enumerable.Range(0, statements.Count).ToArray<int>();
						this.Shuffle<int>(keyId);
						int[] key = new int[keyId.Length];
						int i;
						for (i = 0; i < key.Length; i++)
						{
							int q = new Random().Next() & int.MaxValue;
							key[i] = q - q % statements.Count + keyId[i];
						}
						Dictionary<Instruction, int> statementKeys = new Dictionary<Instruction, int>();
						LinkedListNode<Instruction[]> current = statements.First;
						i = 0;
						while (current != null)
						{
							bool flag3 = i != 0;
							if (flag3)
							{
								statementKeys[current.Value[0]] = key[i];
							}
							i++;
							current = current.Next;
						}
						HashSet<Instruction> statementLast = new HashSet<Instruction>(from st in statements
						select st.Last<Instruction>());
						Func<Instruction, bool> <>9__4;
						Func<Instruction, bool> <>9__5;
						Func<Instruction, bool> <>9__2;
						Func<IList<Instruction>, bool> hasUnknownSource = delegate(IList<Instruction> instrs)
						{
							Func<Instruction, bool> predicate2;
							if ((predicate2 = <>9__2) == null)
							{
								predicate2 = (<>9__2 = delegate(Instruction instr)
								{
									bool flag16 = CS$<>8__locals2.CS$<>8__locals1.trace.HasMultipleSources(instr.Offset);
									bool result;
									if (flag16)
									{
										result = true;
									}
									else
									{
										List<Instruction> srcs;
										bool flag17 = CS$<>8__locals2.CS$<>8__locals1.trace.BrRefs.TryGetValue(instr.Offset, out srcs);
										if (flag17)
										{
											bool flag18 = srcs.Any((Instruction src) => src.Operand is Instruction[]);
											if (flag18)
											{
												return true;
											}
											IEnumerable<Instruction> source = srcs;
											Func<Instruction, bool> predicate3;
											if ((predicate3 = <>9__4) == null)
											{
												predicate3 = (<>9__4 = ((Instruction src) => src.Offset <= statements.First.Value.Last<Instruction>().Offset || src.Offset >= CS$<>8__locals2.block.Instructions.Last<Instruction>().Offset));
											}
											bool flag19 = source.Any(predicate3);
											if (flag19)
											{
												return true;
											}
											IEnumerable<Instruction> source2 = srcs;
											Func<Instruction, bool> predicate4;
											if ((predicate4 = <>9__5) == null)
											{
												predicate4 = (<>9__5 = ((Instruction src) => statementLast.Contains(src)));
											}
											bool flag20 = source2.Any(predicate4);
											if (flag20)
											{
												return true;
											}
										}
										result = false;
									}
									return result;
								});
							}
							return instrs.Any(predicate2);
						};
						Instruction switchInstr = new Instruction(OpCodes.Switch);
						List<Instruction> switchHdr = new List<Instruction>();
						bool flag4 = predicate != null;
						if (flag4)
						{
							predicate.Init(body);
							switchHdr.Add(Instruction.CreateLdcI4(predicate.GetSwitchKey(key[1])));
							predicate.EmitSwitchLoad(switchHdr);
						}
						else
						{
							switchHdr.Add(Instruction.CreateLdcI4(key[1]));
						}
						switchHdr.Add(Instruction.Create(OpCodes.Dup));
						switchHdr.Add(Instruction.Create(OpCodes.Stloc, local));
						switchHdr.Add(Instruction.Create(OpCodes.Ldc_I4, statements.Count));
						switchHdr.Add(Instruction.Create(OpCodes.Rem_Un));
						switchHdr.Add(switchInstr);
						this.AddJump(switchHdr, statements.Last.Value[0], Method);
						this.AddJunk(switchHdr, Method);
						Instruction[] operands = new Instruction[statements.Count];
						current = statements.First;
						i = 0;
						while (current.Next != null)
						{
							List<Instruction> newStatement2 = new List<Instruction>(current.Value);
							bool flag5 = i != 0;
							if (flag5)
							{
								bool converted = false;
								bool flag6 = newStatement2.Last<Instruction>().IsBr();
								if (flag6)
								{
									Instruction target = (Instruction)newStatement2.Last<Instruction>().Operand;
									int brKey;
									bool flag7 = !CS$<>8__locals2.CS$<>8__locals1.trace.IsBranchTarget(newStatement2.Last<Instruction>().Offset) && statementKeys.TryGetValue(target, out brKey);
									if (flag7)
									{
										int targetKey = (predicate != null) ? predicate.GetSwitchKey(brKey) : brKey;
										bool unkSrc = hasUnknownSource(newStatement2);
										newStatement2.RemoveAt(newStatement2.Count - 1);
										bool flag8 = unkSrc;
										if (flag8)
										{
											newStatement2.Add(Instruction.Create(OpCodes.Ldc_I4, targetKey));
										}
										else
										{
											int thisKey = key[i];
											int r = SwitchMangler.rnd.Next(1000, 2000);
											Local tempLocal = new Local(Method.Module.CorLibTypes.UInt32);
											Local tempLocal2 = new Local(Method.Module.CorLibTypes.UInt32);
											body.Variables.Add(tempLocal);
											newStatement2.Add(Instruction.Create(OpCodes.Ldloc, local));
											newStatement2.Add(Instruction.Create(OpCodes.Ldc_I4, r));
											newStatement2.Add(Instruction.Create(OpCodes.Div));
											newStatement2.Add(Instruction.Create(OpCodes.Stloc, tempLocal));
											newStatement2.Add(Instruction.Create(OpCodes.Ldloc, tempLocal));
											newStatement2.Add(Instruction.Create(OpCodes.Ldc_I4, thisKey / r - targetKey));
											newStatement2.Add(Instruction.Create(OpCodes.Sub));
										}
										this.AddJump(newStatement2, switchHdr[1], Method);
										this.AddJunk(newStatement2, Method);
										operands[keyId[i]] = newStatement2[0];
										converted = true;
									}
								}
								else
								{
									bool flag9 = newStatement2.Last<Instruction>().IsConditionalBranch();
									if (flag9)
									{
										Instruction target2 = (Instruction)newStatement2.Last<Instruction>().Operand;
										int brKey2;
										bool flag10 = !CS$<>8__locals2.CS$<>8__locals1.trace.IsBranchTarget(newStatement2.Last<Instruction>().Offset) && statementKeys.TryGetValue(target2, out brKey2);
										if (flag10)
										{
											bool unkSrc2 = hasUnknownSource(newStatement2);
											int nextKey = key[i + 1];
											OpCode condBr = newStatement2.Last<Instruction>().OpCode;
											newStatement2.RemoveAt(newStatement2.Count - 1);
											bool flag11 = Convert.ToBoolean(SwitchMangler.rnd.Next(0, 2));
											if (flag11)
											{
												condBr = SwitchMangler.InverseBranch(condBr);
												int tmp = brKey2;
												brKey2 = nextKey;
												nextKey = tmp;
											}
											int thisKey2 = key[i];
											int r2 = 0;
											int xorKey = 0;
											bool flag12 = !unkSrc2;
											if (flag12)
											{
												r2 = SwitchMangler.rnd.Next(1000, 2000);
												xorKey = thisKey2 / r2;
											}
											Instruction brKeyInstr = Instruction.CreateLdcI4(xorKey ^ ((predicate != null) ? predicate.GetSwitchKey(brKey2) : brKey2));
											Instruction nextKeyInstr = Instruction.CreateLdcI4(xorKey ^ ((predicate != null) ? predicate.GetSwitchKey(nextKey) : nextKey));
											Instruction pop = Instruction.Create(OpCodes.Pop);
											newStatement2.Add(Instruction.Create(condBr, brKeyInstr));
											newStatement2.Add(nextKeyInstr);
											newStatement2.Add(Instruction.Create(OpCodes.Dup));
											newStatement2.Add(Instruction.Create(OpCodes.Br, pop));
											newStatement2.Add(brKeyInstr);
											newStatement2.Add(Instruction.Create(OpCodes.Dup));
											newStatement2.Add(pop);
											bool flag13 = !unkSrc2;
											if (flag13)
											{
												newStatement2.Add(Instruction.Create(OpCodes.Ldloc, local));
												newStatement2.Add(Instruction.Create(OpCodes.Ldc_I4, r2));
												newStatement2.Add(Instruction.Create(OpCodes.Div));
												newStatement2.Add(Instruction.Create(OpCodes.Xor));
											}
											this.AddJump(newStatement2, switchHdr[1], Method);
											this.AddJunk(newStatement2, Method);
											operands[keyId[i]] = newStatement2[0];
											converted = true;
										}
									}
								}
								bool flag14 = !converted;
								if (flag14)
								{
									int targetKey2 = (predicate != null) ? predicate.GetSwitchKey(key[i + 1]) : key[i + 1];
									bool flag15 = !hasUnknownSource(newStatement2);
									if (flag15)
									{
										int thisKey3 = key[i];
										int[] tarray = SwitchMangler.GenerateArray();
										int r3 = tarray[tarray.Length - 1];
										Local tempLocal3 = new Local(Method.Module.CorLibTypes.UInt32);
										Local tempLocal4 = new Local(Method.Module.CorLibTypes.UInt32);
										body.Variables.Add(tempLocal3);
										body.Variables.Add(tempLocal4);
										newStatement2.Add(Instruction.Create(OpCodes.Ldloc, local));
										newStatement2.Add(Instruction.Create(OpCodes.Stloc, tempLocal4));
										SwitchMangler.InjectArray(Method, tarray, ref newStatement2, arraylocal);
										newStatement2.Add(Instruction.Create(OpCodes.Ldloc, tempLocal4));
										newStatement2.Add(OpCodes.Ldloc_S.ToInstruction(arraylocal));
										newStatement2.Add(OpCodes.Ldc_I4.ToInstruction(tarray.Length - 1));
										newStatement2.Add(OpCodes.Ldelem_I4.ToInstruction());
										newStatement2.Add(Instruction.Create(OpCodes.Div));
										newStatement2.Add(Instruction.Create(OpCodes.Stloc, tempLocal3));
										newStatement2.Add(Instruction.Create(OpCodes.Ldloc, tempLocal3));
										newStatement2.Add(Instruction.Create(OpCodes.Ldc_I4, thisKey3 / r3 - targetKey2));
										newStatement2.Add(Instruction.Create(OpCodes.Sub));
									}
									else
									{
										newStatement2.Add(Instruction.Create(OpCodes.Ldc_I4, targetKey2));
									}
									this.AddJump(newStatement2, switchHdr[1], Method);
									this.AddJunk(newStatement2, Method);
									operands[keyId[i]] = newStatement2[0];
								}
							}
							else
							{
								operands[keyId[i]] = switchHdr[0];
							}
							current.Value = newStatement2.ToArray();
							current = current.Next;
							i++;
						}
						operands[keyId[i]] = current.Value[0];
						switchInstr.Operand = operands;
						Instruction[] first = statements.First.Value;
						statements.RemoveFirst();
						Instruction[] last = statements.Last.Value;
						statements.RemoveLast();
						List<Instruction[]> newStatements = statements.ToList<Instruction[]>();
						this.Shuffle<Instruction[]>(newStatements);
						CS$<>8__locals2.block.Instructions.Clear();
						CS$<>8__locals2.block.Instructions.AddRange(first);
						CS$<>8__locals2.block.Instructions.AddRange(switchHdr);
						foreach (Instruction[] statement in newStatements)
						{
							CS$<>8__locals2.block.Instructions.AddRange(statement);
						}
						CS$<>8__locals2.block.Instructions.AddRange(last);
					}
				}
			}
		}

		// Token: 0x06000059 RID: 89 RVA: 0x000097C0 File Offset: 0x000079C0
		private static int[] GenerateArray()
		{
			int[] array = new int[SwitchMangler.rnd.Next(3, 6)];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = SwitchMangler.rnd.Next(100, 500);
			}
			return array;
		}

		// Token: 0x0600005A RID: 90 RVA: 0x00009810 File Offset: 0x00007A10
		private static void InjectArray(MethodDef method, int[] array, ref List<Instruction> toInject, Local local)
		{
			List<Instruction> lista = new List<Instruction>
			{
				OpCodes.Ldc_I4.ToInstruction(array.Length),
				OpCodes.Newarr.ToInstruction(method.Module.CorLibTypes.UInt32),
				OpCodes.Stloc_S.ToInstruction(local)
			};
			for (int i = 0; i < array.Length; i++)
			{
				bool flag = i == 0;
				if (flag)
				{
					lista.Add(OpCodes.Ldloc_S.ToInstruction(local));
					lista.Add(OpCodes.Ldc_I4.ToInstruction(i));
					lista.Add(OpCodes.Ldc_I4.ToInstruction(array[i]));
					lista.Add(OpCodes.Stelem_I4.ToInstruction());
					lista.Add(OpCodes.Nop.ToInstruction());
				}
				else
				{
					int currentValue = array[i];
					lista.Add(OpCodes.Ldloc_S.ToInstruction(local));
					lista.Add(OpCodes.Ldc_I4.ToInstruction(i));
					lista.Add(OpCodes.Ldc_I4.ToInstruction(currentValue));
					int index = lista.Count - 1;
					for (int j = i - 1; j >= 0; j--)
					{
						OpCode opcode = null;
						int num = SwitchMangler.rnd.Next(0, 2);
						int num2 = num;
						if (num2 != 0)
						{
							if (num2 == 1)
							{
								currentValue -= array[j];
								opcode = OpCodes.Add;
							}
						}
						else
						{
							currentValue += array[j];
							opcode = OpCodes.Sub;
						}
						lista.Add(OpCodes.Ldloc_S.ToInstruction(local));
						lista.Add(OpCodes.Ldc_I4.ToInstruction(j));
						lista.Add(OpCodes.Ldelem_I4.ToInstruction());
						lista.Add(opcode.ToInstruction());
					}
					lista[index].OpCode = OpCodes.Ldc_I4;
					lista[index].Operand = currentValue;
					lista.Add(OpCodes.Stelem_I4.ToInstruction());
					lista.Add(OpCodes.Nop.ToInstruction());
				}
			}
			for (int k = 0; k < lista.Count; k++)
			{
				toInject.Add(lista[k]);
			}
		}

		// Token: 0x0600005B RID: 91 RVA: 0x00009A50 File Offset: 0x00007C50
		private LinkedList<Instruction[]> SplitStatements(BlockParser.InstrBlock block, SwitchMangler.Trace trace)
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

		// Token: 0x0600005C RID: 92 RVA: 0x00009C48 File Offset: 0x00007E48
		public void Shuffle<T>(IList<T> list)
		{
			for (int i = list.Count - 1; i > 1; i--)
			{
				int j = new Random().Next(i + 1);
				T tmp = list[j];
				list[j] = list[i];
				list[i] = tmp;
			}
		}

		// Token: 0x0600005D RID: 93 RVA: 0x00009CA0 File Offset: 0x00007EA0
		public void AddJump(IList<Instruction> instrs, Instruction target, MethodDef Method)
		{
			bool flag = !Method.Module.IsClr40 && !Method.DeclaringType.HasGenericParameters && !Method.HasGenericParameters && (instrs[0].OpCode.FlowControl == FlowControl.Call || instrs[0].OpCode.FlowControl == FlowControl.Next);
			if (flag)
			{
				bool addDefOk = false;
				bool flag2 = Convert.ToBoolean(new Random().Next(0, 2));
				if (flag2)
				{
					TypeDef randomType = Method.Module.Types[new Random().Next(Method.Module.Types.Count)];
					bool hasMethods = randomType.HasMethods;
					if (hasMethods)
					{
						instrs.Add(Instruction.Create(OpCodes.Ldtoken, randomType.Methods[new Random().Next(randomType.Methods.Count)]));
						instrs.Add(Instruction.Create(OpCodes.Box, Method.Module.CorLibTypes.GetTypeRef("System", "RuntimeMethodHandle")));
						addDefOk = true;
					}
				}
				bool flag3 = !addDefOk;
				if (flag3)
				{
					instrs.Add(Instruction.Create(OpCodes.Ldc_I4, Convert.ToBoolean(new Random().Next(0, 2)) ? 0 : 1));
					instrs.Add(Instruction.Create(OpCodes.Box, Method.Module.CorLibTypes.Int32.TypeDefOrRef));
				}
				Instruction pop = Instruction.Create(OpCodes.Pop);
				instrs.Add(Instruction.Create(OpCodes.Brfalse, instrs[0]));
				instrs.Add(Instruction.Create(OpCodes.Ldc_I4, Convert.ToBoolean(new Random().Next(0, 2)) ? 0 : 1));
				instrs.Add(pop);
			}
			instrs.Add(Instruction.Create(OpCodes.Br, target));
		}

		// Token: 0x0600005E RID: 94 RVA: 0x00009E80 File Offset: 0x00008080
		public void AddJunk(IList<Instruction> instrs, MethodDef Method)
		{
			bool isClr = Method.Module.IsClr40;
			if (!isClr)
			{
				instrs.Add(Instruction.Create(OpCodes.Pop));
				instrs.Add(Instruction.Create(OpCodes.Dup));
				instrs.Add(Instruction.Create(OpCodes.Throw));
				instrs.Add(Instruction.Create(OpCodes.Ldarg, new Parameter(255)));
				instrs.Add(Instruction.Create(OpCodes.Ldloc, new Local(null, null, 255)));
				instrs.Add(Instruction.Create(OpCodes.Ldtoken, Method));
			}
		}

		// Token: 0x04000020 RID: 32
		private static Random rnd = new Random();

		// Token: 0x0200003D RID: 61
		private struct Trace
		{
			// Token: 0x060000E8 RID: 232 RVA: 0x0000CCB4 File Offset: 0x0000AEB4
			private static void Increment(Dictionary<uint, int> counts, uint key)
			{
				int value;
				bool flag = !counts.TryGetValue(key, out value);
				if (flag)
				{
					value = 0;
				}
				counts[key] = value + 1;
			}

			// Token: 0x060000E9 RID: 233 RVA: 0x0000CCE0 File Offset: 0x0000AEE0
			public Trace(CilBody body, bool hasReturnValue)
			{
				this.RefCount = new Dictionary<uint, int>();
				this.BrRefs = new Dictionary<uint, List<Instruction>>();
				this.BeforeStack = new Dictionary<uint, int>();
				this.AfterStack = new Dictionary<uint, int>();
				body.UpdateInstructionOffsets();
				foreach (ExceptionHandler eh in body.ExceptionHandlers)
				{
					this.BeforeStack[eh.TryStart.Offset] = 0;
					this.BeforeStack[eh.HandlerStart.Offset] = ((eh.HandlerType != ExceptionHandlerType.Finally) ? 1 : 0);
					bool flag = eh.FilterStart != null;
					if (flag)
					{
						this.BeforeStack[eh.FilterStart.Offset] = 1;
					}
				}
				int currentStack = 0;
				int i = 0;
				while (i < body.Instructions.Count)
				{
					Instruction instr = body.Instructions[i];
					bool flag2 = this.BeforeStack.ContainsKey(instr.Offset);
					if (flag2)
					{
						currentStack = this.BeforeStack[instr.Offset];
					}
					this.BeforeStack[instr.Offset] = currentStack;
					instr.UpdateStack(ref currentStack, hasReturnValue);
					this.AfterStack[instr.Offset] = currentStack;
					switch (instr.OpCode.FlowControl)
					{
					case FlowControl.Branch:
					{
						uint offset = ((Instruction)instr.Operand).Offset;
						bool flag3 = !this.BeforeStack.ContainsKey(offset);
						if (flag3)
						{
							this.BeforeStack[offset] = currentStack;
						}
						SwitchMangler.Trace.Increment(this.RefCount, offset);
						this.BrRefs.AddListEntry(offset, instr);
						currentStack = 0;
						break;
					}
					case FlowControl.Break:
					case FlowControl.Meta:
					case FlowControl.Next:
						goto IL_2F9;
					case FlowControl.Call:
					{
						bool flag4 = instr.OpCode.Code == Code.Jmp;
						if (flag4)
						{
							currentStack = 0;
						}
						goto IL_2F9;
					}
					case FlowControl.Cond_Branch:
					{
						bool flag5 = instr.OpCode.Code == Code.Switch;
						if (flag5)
						{
							foreach (Instruction target in (Instruction[])instr.Operand)
							{
								bool flag6 = !this.BeforeStack.ContainsKey(target.Offset);
								if (flag6)
								{
									this.BeforeStack[target.Offset] = currentStack;
								}
								SwitchMangler.Trace.Increment(this.RefCount, target.Offset);
								this.BrRefs.AddListEntry(target.Offset, instr);
							}
						}
						else
						{
							uint offset = ((Instruction)instr.Operand).Offset;
							bool flag7 = !this.BeforeStack.ContainsKey(offset);
							if (flag7)
							{
								this.BeforeStack[offset] = currentStack;
							}
							SwitchMangler.Trace.Increment(this.RefCount, offset);
							this.BrRefs.AddListEntry(offset, instr);
						}
						goto IL_2F9;
					}
					case FlowControl.Phi:
						goto IL_2F3;
					case FlowControl.Return:
					case FlowControl.Throw:
						break;
					default:
						goto IL_2F3;
					}
					IL_337:
					i++;
					continue;
					IL_2F9:
					bool flag8 = i + 1 < body.Instructions.Count;
					if (flag8)
					{
						uint offset = body.Instructions[i + 1].Offset;
						SwitchMangler.Trace.Increment(this.RefCount, offset);
					}
					goto IL_337;
					IL_2F3:
					throw new Exception();
				}
			}

			// Token: 0x060000EA RID: 234 RVA: 0x0000D054 File Offset: 0x0000B254
			public bool IsBranchTarget(uint offset)
			{
				List<Instruction> src;
				bool flag = this.BrRefs.TryGetValue(offset, out src);
				return flag && src.Count > 0;
			}

			// Token: 0x060000EB RID: 235 RVA: 0x0000D088 File Offset: 0x0000B288
			public bool HasMultipleSources(uint offset)
			{
				int src;
				bool flag = this.RefCount.TryGetValue(offset, out src);
				return flag && src > 1;
			}

			// Token: 0x04000063 RID: 99
			public Dictionary<uint, int> RefCount;

			// Token: 0x04000064 RID: 100
			public Dictionary<uint, List<Instruction>> BrRefs;

			// Token: 0x04000065 RID: 101
			public Dictionary<uint, int> BeforeStack;

			// Token: 0x04000066 RID: 102
			public Dictionary<uint, int> AfterStack;
		}
	}
}
