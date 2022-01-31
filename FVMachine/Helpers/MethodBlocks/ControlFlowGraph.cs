using System;
using System.Collections;
using System.Collections.Generic;
using dnlib.DotNet.Emit;

namespace FVM.Helpers.MethodBlocks
{
	// Token: 0x0200001B RID: 27
	public class ControlFlowGraph : IEnumerable<ControlFlowBlock>, IEnumerable
	{
		// Token: 0x06000066 RID: 102 RVA: 0x0000A4E8 File Offset: 0x000086E8
		private ControlFlowGraph(CilBody body)
		{
			this.body = body;
			this.instrBlocks = new int[body.Instructions.Count];
			this.blocks = new List<ControlFlowBlock>();
			this.indexMap = new Dictionary<Instruction, int>();
			for (int i = 0; i < body.Instructions.Count; i++)
			{
				this.indexMap.Add(body.Instructions[i], i);
			}
		}

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x06000067 RID: 103 RVA: 0x0000A564 File Offset: 0x00008764
		public int Count
		{
			get
			{
				return this.blocks.Count;
			}
		}

		// Token: 0x1700000B RID: 11
		public ControlFlowBlock this[int id]
		{
			get
			{
				return this.blocks[id];
			}
		}

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x06000069 RID: 105 RVA: 0x0000A5A4 File Offset: 0x000087A4
		public CilBody Body
		{
			get
			{
				return this.body;
			}
		}

		// Token: 0x0600006A RID: 106 RVA: 0x0000A5BC File Offset: 0x000087BC
		IEnumerator<ControlFlowBlock> IEnumerable<ControlFlowBlock>.GetEnumerator()
		{
			return this.blocks.GetEnumerator();
		}

		// Token: 0x0600006B RID: 107 RVA: 0x0000A5E0 File Offset: 0x000087E0
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.blocks.GetEnumerator();
		}

		// Token: 0x0600006C RID: 108 RVA: 0x0000A604 File Offset: 0x00008804
		public ControlFlowBlock GetContainingBlock(int instrIndex)
		{
			return this.blocks[this.instrBlocks[instrIndex]];
		}

		// Token: 0x0600006D RID: 109 RVA: 0x0000A62C File Offset: 0x0000882C
		public int IndexOf(Instruction instr)
		{
			return this.indexMap[instr];
		}

		// Token: 0x0600006E RID: 110 RVA: 0x0000A64C File Offset: 0x0000884C
		private void PopulateBlockHeaders(HashSet<Instruction> blockHeaders, HashSet<Instruction> entryHeaders)
		{
			for (int i = 0; i < this.body.Instructions.Count; i++)
			{
				Instruction instr = this.body.Instructions[i];
				bool flag = instr.Operand is Instruction;
				if (flag)
				{
					blockHeaders.Add((Instruction)instr.Operand);
					bool flag2 = i + 1 < this.body.Instructions.Count;
					if (flag2)
					{
						blockHeaders.Add(this.body.Instructions[i + 1]);
					}
				}
				else
				{
					bool flag3 = instr.Operand is Instruction[];
					if (flag3)
					{
						foreach (Instruction target in (Instruction[])instr.Operand)
						{
							blockHeaders.Add(target);
						}
						bool flag4 = i + 1 < this.body.Instructions.Count;
						if (flag4)
						{
							blockHeaders.Add(this.body.Instructions[i + 1]);
						}
					}
					else
					{
						bool flag5 = (instr.OpCode.FlowControl == FlowControl.Throw || instr.OpCode.FlowControl == FlowControl.Return) && i + 1 < this.body.Instructions.Count;
						if (flag5)
						{
							blockHeaders.Add(this.body.Instructions[i + 1]);
						}
					}
				}
			}
			blockHeaders.Add(this.body.Instructions[0]);
			foreach (ExceptionHandler eh in this.body.ExceptionHandlers)
			{
				blockHeaders.Add(eh.TryStart);
				blockHeaders.Add(eh.HandlerStart);
				blockHeaders.Add(eh.FilterStart);
				entryHeaders.Add(eh.HandlerStart);
				entryHeaders.Add(eh.FilterStart);
			}
		}

		// Token: 0x0600006F RID: 111 RVA: 0x0000A870 File Offset: 0x00008A70
		private void SplitBlocks(HashSet<Instruction> blockHeaders, HashSet<Instruction> entryHeaders)
		{
			int nextBlockId = 0;
			int currentBlockId = -1;
			Instruction currentBlockHdr = null;
			for (int i = 0; i < this.body.Instructions.Count; i++)
			{
				Instruction instr = this.body.Instructions[i];
				bool flag = blockHeaders.Contains(instr);
				if (flag)
				{
					bool flag2 = currentBlockHdr != null;
					if (flag2)
					{
						Instruction footer = this.body.Instructions[i - 1];
						ControlFlowBlockType type = ControlFlowBlockType.Normal;
						bool flag3 = entryHeaders.Contains(currentBlockHdr) || currentBlockHdr == this.body.Instructions[0];
						if (flag3)
						{
							type |= ControlFlowBlockType.Entry;
						}
						bool flag4 = footer.OpCode.FlowControl == FlowControl.Return || footer.OpCode.FlowControl == FlowControl.Throw;
						if (flag4)
						{
							type |= ControlFlowBlockType.Exit;
						}
						this.blocks.Add(new ControlFlowBlock(currentBlockId, type, currentBlockHdr, footer));
					}
					currentBlockId = nextBlockId++;
					currentBlockHdr = instr;
				}
				this.instrBlocks[i] = currentBlockId;
			}
			bool flag5 = this.blocks.Count == 0 || this.blocks[this.blocks.Count - 1].Id != currentBlockId;
			if (flag5)
			{
				Instruction footer2 = this.body.Instructions[this.body.Instructions.Count - 1];
				ControlFlowBlockType type2 = ControlFlowBlockType.Normal;
				bool flag6 = entryHeaders.Contains(currentBlockHdr) || currentBlockHdr == this.body.Instructions[0];
				if (flag6)
				{
					type2 |= ControlFlowBlockType.Entry;
				}
				bool flag7 = footer2.OpCode.FlowControl == FlowControl.Return || footer2.OpCode.FlowControl == FlowControl.Throw;
				if (flag7)
				{
					type2 |= ControlFlowBlockType.Exit;
				}
				this.blocks.Add(new ControlFlowBlock(currentBlockId, type2, currentBlockHdr, footer2));
			}
		}

		// Token: 0x06000070 RID: 112 RVA: 0x0000AA54 File Offset: 0x00008C54
		private void LinkBlocks()
		{
			for (int i = 0; i < this.body.Instructions.Count; i++)
			{
				Instruction instr = this.body.Instructions[i];
				bool flag = instr.Operand is Instruction;
				if (flag)
				{
					ControlFlowBlock srcBlock = this.blocks[this.instrBlocks[i]];
					ControlFlowBlock dstBlock = this.blocks[this.instrBlocks[this.indexMap[(Instruction)instr.Operand]]];
					dstBlock.Sources.Add(srcBlock);
					srcBlock.Targets.Add(dstBlock);
				}
				else
				{
					bool flag2 = instr.Operand is Instruction[];
					if (flag2)
					{
						foreach (Instruction target in (Instruction[])instr.Operand)
						{
							ControlFlowBlock srcBlock2 = this.blocks[this.instrBlocks[i]];
							ControlFlowBlock dstBlock2 = this.blocks[this.instrBlocks[this.indexMap[target]]];
							dstBlock2.Sources.Add(srcBlock2);
							srcBlock2.Targets.Add(dstBlock2);
						}
					}
				}
			}
			for (int j = 0; j < this.blocks.Count; j++)
			{
				bool flag3 = this.blocks[j].Footer.OpCode.FlowControl != FlowControl.Branch && this.blocks[j].Footer.OpCode.FlowControl != FlowControl.Return && this.blocks[j].Footer.OpCode.FlowControl != FlowControl.Throw;
				if (flag3)
				{
					this.blocks[j].Targets.Add(this.blocks[j + 1]);
					this.blocks[j + 1].Sources.Add(this.blocks[j]);
				}
			}
		}

		// Token: 0x06000071 RID: 113 RVA: 0x0000AC88 File Offset: 0x00008E88
		public static ControlFlowGraph Construct(CilBody body)
		{
			ControlFlowGraph graph = new ControlFlowGraph(body);
			bool flag = body.Instructions.Count == 0;
			ControlFlowGraph result;
			if (flag)
			{
				result = graph;
			}
			else
			{
				HashSet<Instruction> blockHeaders = new HashSet<Instruction>();
				HashSet<Instruction> entryHeaders = new HashSet<Instruction>();
				graph.PopulateBlockHeaders(blockHeaders, entryHeaders);
				graph.SplitBlocks(blockHeaders, entryHeaders);
				graph.LinkBlocks();
				result = graph;
			}
			return result;
		}

		// Token: 0x04000026 RID: 38
		private readonly List<ControlFlowBlock> blocks;

		// Token: 0x04000027 RID: 39
		private readonly CilBody body;

		// Token: 0x04000028 RID: 40
		private readonly int[] instrBlocks;

		// Token: 0x04000029 RID: 41
		private readonly Dictionary<Instruction, int> indexMap;
	}
}
