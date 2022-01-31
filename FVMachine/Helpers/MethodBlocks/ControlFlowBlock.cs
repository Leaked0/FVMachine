using System;
using System.Collections.Generic;
using System.Linq;
using dnlib.DotNet.Emit;

namespace FVM.Helpers.MethodBlocks
{
	// Token: 0x0200001D RID: 29
	public class ControlFlowBlock
	{
		// Token: 0x06000072 RID: 114 RVA: 0x0000ACE0 File Offset: 0x00008EE0
		internal ControlFlowBlock(int id, ControlFlowBlockType type, Instruction header, Instruction footer)
		{
			this.Id = id;
			this.Type = type;
			this.Header = header;
			this.Footer = footer;
			this.Sources = new List<ControlFlowBlock>();
			this.Targets = new List<ControlFlowBlock>();
		}

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x06000073 RID: 115 RVA: 0x0000AD1F File Offset: 0x00008F1F
		// (set) Token: 0x06000074 RID: 116 RVA: 0x0000AD27 File Offset: 0x00008F27
		public IList<ControlFlowBlock> Sources { get; private set; }

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x06000075 RID: 117 RVA: 0x0000AD30 File Offset: 0x00008F30
		// (set) Token: 0x06000076 RID: 118 RVA: 0x0000AD38 File Offset: 0x00008F38
		public IList<ControlFlowBlock> Targets { get; private set; }

		// Token: 0x06000077 RID: 119 RVA: 0x0000AD44 File Offset: 0x00008F44
		public override string ToString()
		{
			return string.Format("Block {0} => {1} {2}", this.Id, this.Type, string.Join(", ", (from block in this.Targets
			select block.Id.ToString()).ToArray<string>()));
		}

		// Token: 0x0400002E RID: 46
		public readonly Instruction Footer;

		// Token: 0x0400002F RID: 47
		public readonly Instruction Header;

		// Token: 0x04000030 RID: 48
		public readonly int Id;

		// Token: 0x04000031 RID: 49
		public readonly ControlFlowBlockType Type;
	}
}
