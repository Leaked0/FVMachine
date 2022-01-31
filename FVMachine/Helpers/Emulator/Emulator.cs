using System;
using System.Collections.Generic;
using System.Linq;
using dnlib.DotNet.Emit;

namespace FVM.Helpers.Emulator
{
	// Token: 0x02000023 RID: 35
	public class Emulator
	{
		// Token: 0x06000086 RID: 134 RVA: 0x0000B7C8 File Offset: 0x000099C8
		public Emulator(List<Instruction> instructions, List<Local> locals)
		{
			this._context = new EmuContext(instructions, locals);
			this._emuInstructions = new Dictionary<OpCode, EmuInstruction>();
			List<EmuInstruction> emuInstructions = (from t in typeof(EmuInstruction).Assembly.GetTypes()
			where t.IsSubclassOf(typeof(EmuInstruction)) && !t.IsAbstract
			select (EmuInstruction)Activator.CreateInstance(t)).ToList<EmuInstruction>();
			foreach (EmuInstruction instrEmu in emuInstructions)
			{
				this._emuInstructions.Add(instrEmu.OpCode, instrEmu);
			}
		}

		// Token: 0x06000087 RID: 135 RVA: 0x0000B8A8 File Offset: 0x00009AA8
		internal int Emulate()
		{
			for (int i = this._context.InstructionPointer; i < this._context.Instructions.Count; i++)
			{
				Instruction current = this._context.Instructions[i];
				bool flag = current.OpCode == OpCodes.Stloc_S;
				if (flag)
				{
					break;
				}
				bool flag2 = current.OpCode != OpCodes.Nop;
				if (flag2)
				{
					this._emuInstructions[current.OpCode].Emulate(this._context, current);
				}
			}
			return (int)this._context.Stack.Pop();
		}

		// Token: 0x0400003C RID: 60
		public EmuContext _context;

		// Token: 0x0400003D RID: 61
		private Dictionary<OpCode, EmuInstruction> _emuInstructions;
	}
}
