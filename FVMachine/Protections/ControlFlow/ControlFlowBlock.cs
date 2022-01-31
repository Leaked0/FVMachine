using System;
using System.Collections.Generic;
using System.Linq;
using dnlib.DotNet.Emit;

namespace FVM.Protections.ControlFlow
{
	// Token: 0x02000013 RID: 19
	public class ControlFlowBlock
	{
		// Token: 0x06000045 RID: 69 RVA: 0x000089C0 File Offset: 0x00006BC0
		internal ControlFlowBlock(int id, ControlFlowBlockType type, Instruction header, Instruction footer)
		{
			this.Id = id;
			this.Type = type;
			this.Header = header;
			this.Footer = footer;
			this.Sources = new List<ControlFlowBlock>();
			this.Targets = new List<ControlFlowBlock>();
		}

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x06000046 RID: 70 RVA: 0x000089FF File Offset: 0x00006BFF
		// (set) Token: 0x06000047 RID: 71 RVA: 0x00008A07 File Offset: 0x00006C07
		public IList<ControlFlowBlock> Sources { get; private set; }

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x06000048 RID: 72 RVA: 0x00008A10 File Offset: 0x00006C10
		// (set) Token: 0x06000049 RID: 73 RVA: 0x00008A18 File Offset: 0x00006C18
		public IList<ControlFlowBlock> Targets { get; private set; }

		// Token: 0x0600004A RID: 74 RVA: 0x00008A24 File Offset: 0x00006C24
		public override string ToString()
		{
			return string.Format("Block {0} => {1} {2}", this.Id, this.Type, string.Join(", ", (from block in this.Targets
			select block.Id.ToString()).ToArray<string>()));
		}

		// Token: 0x04000016 RID: 22
		public readonly Instruction Footer;

		// Token: 0x04000017 RID: 23
		public readonly Instruction Header;

		// Token: 0x04000018 RID: 24
		public readonly int Id;

		// Token: 0x04000019 RID: 25
		public readonly ControlFlowBlockType Type;
	}
}
