using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using dnlib.DotNet.Emit;

namespace FVM.Helpers.MethodBlocks
{
	// Token: 0x0200001A RID: 26
	internal static class BlockParser
	{
		// Token: 0x06000065 RID: 101 RVA: 0x0000A0C8 File Offset: 0x000082C8
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

		// Token: 0x02000042 RID: 66
		internal abstract class BlockBase
		{
			// Token: 0x060000F7 RID: 247 RVA: 0x0000D28E File Offset: 0x0000B48E
			public BlockBase(BlockParser.BlockType type)
			{
				this.Type = type;
			}

			// Token: 0x17000024 RID: 36
			// (get) Token: 0x060000F8 RID: 248 RVA: 0x0000D2A0 File Offset: 0x0000B4A0
			// (set) Token: 0x060000F9 RID: 249 RVA: 0x0000D2A8 File Offset: 0x0000B4A8
			public BlockParser.BlockType Type { get; private set; }

			// Token: 0x060000FA RID: 250
			public abstract void ToBody(CilBody body);
		}

		// Token: 0x02000043 RID: 67
		internal enum BlockType
		{
			// Token: 0x04000075 RID: 117
			Normal,
			// Token: 0x04000076 RID: 118
			Try,
			// Token: 0x04000077 RID: 119
			Handler,
			// Token: 0x04000078 RID: 120
			Finally,
			// Token: 0x04000079 RID: 121
			Filter,
			// Token: 0x0400007A RID: 122
			Fault
		}

		// Token: 0x02000044 RID: 68
		internal class ScopeBlock : BlockParser.BlockBase
		{
			// Token: 0x060000FB RID: 251 RVA: 0x0000D2B1 File Offset: 0x0000B4B1
			public ScopeBlock(BlockParser.BlockType type, ExceptionHandler handler) : base(type)
			{
				this.Handler = handler;
				this.Children = new List<BlockParser.BlockBase>();
			}

			// Token: 0x17000025 RID: 37
			// (get) Token: 0x060000FC RID: 252 RVA: 0x0000D2D0 File Offset: 0x0000B4D0
			// (set) Token: 0x060000FD RID: 253 RVA: 0x0000D2D8 File Offset: 0x0000B4D8
			public ExceptionHandler Handler { get; private set; }

			// Token: 0x17000026 RID: 38
			// (get) Token: 0x060000FE RID: 254 RVA: 0x0000D2E1 File Offset: 0x0000B4E1
			// (set) Token: 0x060000FF RID: 255 RVA: 0x0000D2E9 File Offset: 0x0000B4E9
			public List<BlockParser.BlockBase> Children { get; set; }

			// Token: 0x06000100 RID: 256 RVA: 0x0000D2F4 File Offset: 0x0000B4F4
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

			// Token: 0x06000101 RID: 257 RVA: 0x0000D3E4 File Offset: 0x0000B5E4
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

			// Token: 0x06000102 RID: 258 RVA: 0x0000D430 File Offset: 0x0000B630
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

			// Token: 0x06000103 RID: 259 RVA: 0x0000D47C File Offset: 0x0000B67C
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

		// Token: 0x02000045 RID: 69
		internal class InstrBlock : BlockParser.BlockBase
		{
			// Token: 0x06000104 RID: 260 RVA: 0x0000D560 File Offset: 0x0000B760
			public InstrBlock() : base(BlockParser.BlockType.Normal)
			{
				this.Instructions = new List<Instruction>();
			}

			// Token: 0x17000027 RID: 39
			// (get) Token: 0x06000105 RID: 261 RVA: 0x0000D577 File Offset: 0x0000B777
			// (set) Token: 0x06000106 RID: 262 RVA: 0x0000D57F File Offset: 0x0000B77F
			public List<Instruction> Instructions { get; set; }

			// Token: 0x06000107 RID: 263 RVA: 0x0000D588 File Offset: 0x0000B788
			public override string ToString()
			{
				StringBuilder ret = new StringBuilder();
				foreach (Instruction instr in this.Instructions)
				{
					ret.AppendLine(instr.ToString());
				}
				return ret.ToString();
			}

			// Token: 0x06000108 RID: 264 RVA: 0x0000D5F4 File Offset: 0x0000B7F4
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
