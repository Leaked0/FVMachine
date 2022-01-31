using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using dnlib.DotNet.Emit;

namespace FVM.Protections.ControlFlow
{
	// Token: 0x02000010 RID: 16
	internal static class BlockParser
	{
		// Token: 0x06000038 RID: 56 RVA: 0x00007DA8 File Offset: 0x00005FA8
		public static BlockParser.ScopeBlock ParseBody(CilBody body)
		{
			Dictionary<ExceptionHandler, Tuple<BlockParser.ScopeBlock, BlockParser.ScopeBlock, BlockParser.ScopeBlock>> ehScopes = new Dictionary<ExceptionHandler, Tuple<BlockParser.ScopeBlock, BlockParser.ScopeBlock, BlockParser.ScopeBlock>>();
			foreach (ExceptionHandler eh in body.ExceptionHandlers)
			{
				BlockParser.ScopeBlock tryBlock = new BlockParser.ScopeBlock(BlockParser.BlockType.Try, eh);
				BlockParser.BlockType handlerType = BlockParser.BlockType.Handler;
				bool flag = eh.HandlerType == ExceptionHandlerType.Finally;
				if (flag)
				{
					handlerType = BlockParser.BlockType.Finally;
				}
				else
				{
					bool flag2 = eh.HandlerType == ExceptionHandlerType.Fault;
					if (flag2)
					{
						handlerType = BlockParser.BlockType.Fault;
					}
				}
				BlockParser.ScopeBlock handlerBlock = new BlockParser.ScopeBlock(handlerType, eh);
				bool flag3 = eh.FilterStart != null;
				if (flag3)
				{
					BlockParser.ScopeBlock filterBlock = new BlockParser.ScopeBlock(BlockParser.BlockType.Filter, eh);
					ehScopes[eh] = Tuple.Create<BlockParser.ScopeBlock, BlockParser.ScopeBlock, BlockParser.ScopeBlock>(tryBlock, handlerBlock, filterBlock);
				}
				else
				{
					ehScopes[eh] = Tuple.Create<BlockParser.ScopeBlock, BlockParser.ScopeBlock, BlockParser.ScopeBlock>(tryBlock, handlerBlock, null);
				}
			}
			BlockParser.ScopeBlock root = new BlockParser.ScopeBlock(BlockParser.BlockType.Normal, null);
			Stack<BlockParser.ScopeBlock> scopeStack = new Stack<BlockParser.ScopeBlock>();
			scopeStack.Push(root);
			foreach (Instruction instr in body.Instructions)
			{
				foreach (ExceptionHandler eh2 in body.ExceptionHandlers)
				{
					Tuple<BlockParser.ScopeBlock, BlockParser.ScopeBlock, BlockParser.ScopeBlock> ehScope = ehScopes[eh2];
					bool flag4 = instr == eh2.TryEnd;
					if (flag4)
					{
						scopeStack.Pop();
					}
					bool flag5 = instr == eh2.HandlerEnd;
					if (flag5)
					{
						scopeStack.Pop();
					}
					bool flag6 = eh2.FilterStart != null && instr == eh2.HandlerStart;
					if (flag6)
					{
						Debug.Assert(scopeStack.Peek().Type == BlockParser.BlockType.Filter);
						scopeStack.Pop();
					}
				}
				foreach (ExceptionHandler eh3 in body.ExceptionHandlers.Reverse<ExceptionHandler>())
				{
					Tuple<BlockParser.ScopeBlock, BlockParser.ScopeBlock, BlockParser.ScopeBlock> ehScope2 = ehScopes[eh3];
					BlockParser.ScopeBlock parent = (scopeStack.Count > 0) ? scopeStack.Peek() : null;
					bool flag7 = instr == eh3.TryStart;
					if (flag7)
					{
						bool flag8 = parent != null;
						if (flag8)
						{
							parent.Children.Add(ehScope2.Item1);
						}
						scopeStack.Push(ehScope2.Item1);
					}
					bool flag9 = instr == eh3.HandlerStart;
					if (flag9)
					{
						bool flag10 = parent != null;
						if (flag10)
						{
							parent.Children.Add(ehScope2.Item2);
						}
						scopeStack.Push(ehScope2.Item2);
					}
					bool flag11 = instr == eh3.FilterStart;
					if (flag11)
					{
						bool flag12 = parent != null;
						if (flag12)
						{
							parent.Children.Add(ehScope2.Item3);
						}
						scopeStack.Push(ehScope2.Item3);
					}
				}
				BlockParser.ScopeBlock scope = scopeStack.Peek();
				BlockParser.InstrBlock block = scope.Children.LastOrDefault<BlockParser.BlockBase>() as BlockParser.InstrBlock;
				bool flag13 = block == null;
				if (flag13)
				{
					scope.Children.Add(block = new BlockParser.InstrBlock());
				}
				block.Instructions.Add(instr);
			}
			foreach (ExceptionHandler eh4 in body.ExceptionHandlers)
			{
				bool flag14 = eh4.TryEnd == null;
				if (flag14)
				{
					scopeStack.Pop();
				}
				bool flag15 = eh4.HandlerEnd == null;
				if (flag15)
				{
					scopeStack.Pop();
				}
			}
			Debug.Assert(scopeStack.Count == 1);
			return root;
		}

		// Token: 0x02000037 RID: 55
		internal abstract class BlockBase
		{
			// Token: 0x060000C9 RID: 201 RVA: 0x0000C61F File Offset: 0x0000A81F
			public BlockBase(BlockParser.BlockType type)
			{
				this.Type = type;
			}

			// Token: 0x1700001E RID: 30
			// (get) Token: 0x060000CA RID: 202 RVA: 0x0000C631 File Offset: 0x0000A831
			// (set) Token: 0x060000CB RID: 203 RVA: 0x0000C639 File Offset: 0x0000A839
			public BlockParser.BlockType Type { get; private set; }

			// Token: 0x060000CC RID: 204
			public abstract void ToBody(CilBody body);
		}

		// Token: 0x02000038 RID: 56
		internal enum BlockType
		{
			// Token: 0x0400004F RID: 79
			Normal,
			// Token: 0x04000050 RID: 80
			Try,
			// Token: 0x04000051 RID: 81
			Handler,
			// Token: 0x04000052 RID: 82
			Finally,
			// Token: 0x04000053 RID: 83
			Filter,
			// Token: 0x04000054 RID: 84
			Fault
		}

		// Token: 0x02000039 RID: 57
		internal class ScopeBlock : BlockParser.BlockBase
		{
			// Token: 0x060000CD RID: 205 RVA: 0x0000C642 File Offset: 0x0000A842
			public ScopeBlock(BlockParser.BlockType type, ExceptionHandler handler) : base(type)
			{
				this.Handler = handler;
				this.Children = new List<BlockParser.BlockBase>();
			}

			// Token: 0x1700001F RID: 31
			// (get) Token: 0x060000CE RID: 206 RVA: 0x0000C661 File Offset: 0x0000A861
			// (set) Token: 0x060000CF RID: 207 RVA: 0x0000C669 File Offset: 0x0000A869
			public ExceptionHandler Handler { get; private set; }

			// Token: 0x17000020 RID: 32
			// (get) Token: 0x060000D0 RID: 208 RVA: 0x0000C672 File Offset: 0x0000A872
			// (set) Token: 0x060000D1 RID: 209 RVA: 0x0000C67A File Offset: 0x0000A87A
			public List<BlockParser.BlockBase> Children { get; set; }

			// Token: 0x060000D2 RID: 210 RVA: 0x0000C684 File Offset: 0x0000A884
			public override string ToString()
			{
				StringBuilder ret = new StringBuilder();
				bool flag = base.Type == BlockParser.BlockType.Try;
				if (flag)
				{
					ret.Append("try ");
				}
				else
				{
					bool flag2 = base.Type == BlockParser.BlockType.Handler;
					if (flag2)
					{
						ret.Append("handler ");
					}
					else
					{
						bool flag3 = base.Type == BlockParser.BlockType.Finally;
						if (flag3)
						{
							ret.Append("finally ");
						}
						else
						{
							bool flag4 = base.Type == BlockParser.BlockType.Fault;
							if (flag4)
							{
								ret.Append("fault ");
							}
						}
					}
				}
				ret.AppendLine("{");
				foreach (BlockParser.BlockBase child in this.Children)
				{
					ret.Append(child);
				}
				ret.AppendLine("}");
				return ret.ToString();
			}

			// Token: 0x060000D3 RID: 211 RVA: 0x0000C774 File Offset: 0x0000A974
			public Instruction GetFirstInstr()
			{
				BlockParser.BlockBase firstBlock = this.Children.First<BlockParser.BlockBase>();
				bool flag = firstBlock is BlockParser.ScopeBlock;
				Instruction result;
				if (flag)
				{
					result = ((BlockParser.ScopeBlock)firstBlock).GetFirstInstr();
				}
				else
				{
					result = ((BlockParser.InstrBlock)firstBlock).Instructions.First<Instruction>();
				}
				return result;
			}

			// Token: 0x060000D4 RID: 212 RVA: 0x0000C7C0 File Offset: 0x0000A9C0
			public Instruction GetLastInstr()
			{
				BlockParser.BlockBase firstBlock = this.Children.Last<BlockParser.BlockBase>();
				bool flag = firstBlock is BlockParser.ScopeBlock;
				Instruction result;
				if (flag)
				{
					result = ((BlockParser.ScopeBlock)firstBlock).GetLastInstr();
				}
				else
				{
					result = ((BlockParser.InstrBlock)firstBlock).Instructions.Last<Instruction>();
				}
				return result;
			}

			// Token: 0x060000D5 RID: 213 RVA: 0x0000C80C File Offset: 0x0000AA0C
			public override void ToBody(CilBody body)
			{
				bool flag = base.Type > BlockParser.BlockType.Normal;
				if (flag)
				{
					bool flag2 = base.Type == BlockParser.BlockType.Try;
					if (flag2)
					{
						this.Handler.TryStart = this.GetFirstInstr();
						this.Handler.TryEnd = this.GetLastInstr();
					}
					else
					{
						bool flag3 = base.Type == BlockParser.BlockType.Filter;
						if (flag3)
						{
							this.Handler.FilterStart = this.GetFirstInstr();
						}
						else
						{
							this.Handler.HandlerStart = this.GetFirstInstr();
							this.Handler.HandlerEnd = this.GetLastInstr();
						}
					}
				}
				foreach (BlockParser.BlockBase block in this.Children)
				{
					block.ToBody(body);
				}
			}
		}

		// Token: 0x0200003A RID: 58
		internal class InstrBlock : BlockParser.BlockBase
		{
			// Token: 0x060000D6 RID: 214 RVA: 0x0000C8F0 File Offset: 0x0000AAF0
			public InstrBlock() : base(BlockParser.BlockType.Normal)
			{
				this.Instructions = new List<Instruction>();
			}

			// Token: 0x17000021 RID: 33
			// (get) Token: 0x060000D7 RID: 215 RVA: 0x0000C907 File Offset: 0x0000AB07
			// (set) Token: 0x060000D8 RID: 216 RVA: 0x0000C90F File Offset: 0x0000AB0F
			public List<Instruction> Instructions { get; set; }

			// Token: 0x060000D9 RID: 217 RVA: 0x0000C918 File Offset: 0x0000AB18
			public override string ToString()
			{
				StringBuilder ret = new StringBuilder();
				foreach (Instruction instr in this.Instructions)
				{
					ret.AppendLine(instr.ToString());
				}
				return ret.ToString();
			}

			// Token: 0x060000DA RID: 218 RVA: 0x0000C984 File Offset: 0x0000AB84
			public override void ToBody(CilBody body)
			{
				foreach (Instruction instr in this.Instructions)
				{
					body.Instructions.Add(instr);
				}
			}
		}
	}
}
