using System;

namespace FVM.Protections.ControlFlow
{
	// Token: 0x02000012 RID: 18
	[Flags]
	public enum ControlFlowBlockType
	{
		// Token: 0x04000013 RID: 19
		Normal = 0,
		// Token: 0x04000014 RID: 20
		Entry = 1,
		// Token: 0x04000015 RID: 21
		Exit = 2
	}
}
