using System;
using System.Collections.Generic;
using dnlib.DotNet.Emit;

namespace FVM.Helpers.Emulator
{
	// Token: 0x02000021 RID: 33
	public class EmuContext
	{
		// Token: 0x06000080 RID: 128 RVA: 0x0000B6F0 File Offset: 0x000098F0
		public EmuContext(List<Instruction> instructions, List<Local> locals)
		{
			this.Stack = new Stack<object>();
			this.Instructions = instructions;
			this._locals = new Dictionary<Local, object>();
			foreach (Local local in locals)
			{
				this._locals.Add(local, null);
			}
		}

		// Token: 0x06000081 RID: 129 RVA: 0x0000B778 File Offset: 0x00009978
		internal object GetLocalValue(Local local)
		{
			Type type = Type.GetType(local.Type.AssemblyQualifiedName);
			return Convert.ChangeType(this._locals[local], type);
		}

		// Token: 0x06000082 RID: 130 RVA: 0x0000B7AD File Offset: 0x000099AD
		internal void SetLocalValue(Local local, object val)
		{
			this._locals[local] = val;
		}

		// Token: 0x04000038 RID: 56
		internal Stack<object> Stack;

		// Token: 0x04000039 RID: 57
		internal List<Instruction> Instructions;

		// Token: 0x0400003A RID: 58
		internal int InstructionPointer = 0;

		// Token: 0x0400003B RID: 59
		public Dictionary<Local, object> _locals;
	}
}
