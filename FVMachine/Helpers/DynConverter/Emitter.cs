using System;
using System.Collections.Generic;
using System.IO;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace FVM.Helpers.DynConverter
{
	// Token: 0x0200002F RID: 47
	public static class Emitter
	{
		// Token: 0x060000AA RID: 170 RVA: 0x0000BF94 File Offset: 0x0000A194
		public static void EmitNone(this BinaryWriter writer)
		{
			writer.Write(0);
		}

		// Token: 0x060000AB RID: 171 RVA: 0x0000BF9F File Offset: 0x0000A19F
		public static void EmitString(this BinaryWriter writer, Instruction instr)
		{
			writer.Write(1);
			writer.Write(instr.Operand.ToString());
		}

		// Token: 0x060000AC RID: 172 RVA: 0x0000BFBC File Offset: 0x0000A1BC
		public static void EmitR(this BinaryWriter writer, Instruction instr)
		{
			writer.Write(2);
			writer.Write((double)instr.Operand);
		}

		// Token: 0x060000AD RID: 173 RVA: 0x0000BFD9 File Offset: 0x0000A1D9
		public static void EmitI8(this BinaryWriter writer, Instruction instr)
		{
			writer.Write(3);
			writer.Write((long)instr.Operand);
		}

		// Token: 0x060000AE RID: 174 RVA: 0x0000BFF6 File Offset: 0x0000A1F6
		public static void EmitI(this BinaryWriter writer, Instruction instr)
		{
			writer.Write(4);
			writer.Write(instr.GetLdcI4Value());
		}

		// Token: 0x060000AF RID: 175 RVA: 0x0000C00E File Offset: 0x0000A20E
		public static void EmitShortR(this BinaryWriter writer, Instruction instr)
		{
			writer.Write(5);
			writer.Write((float)instr.Operand);
		}

		// Token: 0x060000B0 RID: 176 RVA: 0x0000C02B File Offset: 0x0000A22B
		public static void EmitShortI(this BinaryWriter writer, Instruction instr)
		{
			writer.Write(6);
			writer.Write((byte)instr.GetLdcI4Value());
		}

		// Token: 0x060000B1 RID: 177 RVA: 0x0000C044 File Offset: 0x0000A244
		public static void EmitType(this BinaryWriter writer, Instruction instr)
		{
			writer.Write(7);
			ITypeDefOrRef typeDeforRef = instr.Operand as ITypeDefOrRef;
			writer.Write(typeDeforRef.MDToken.ToInt32());
		}

		// Token: 0x060000B2 RID: 178 RVA: 0x0000C07C File Offset: 0x0000A27C
		public static void EmitField(this BinaryWriter writer, Instruction instr)
		{
			writer.Write(8);
			IField field = instr.Operand as IField;
			writer.Write(field.MDToken.ToInt32());
		}

		// Token: 0x060000B3 RID: 179 RVA: 0x0000C0B4 File Offset: 0x0000A2B4
		public static void EmitMethod(this BinaryWriter writer, Instruction instr)
		{
			writer.Write(9);
			MethodSpec spec = instr.Operand as MethodSpec;
			bool flag = spec != null;
			if (flag)
			{
				writer.Write(spec.MDToken.ToInt32());
			}
			else
			{
				IMethodDefOrRef method = instr.Operand as IMethodDefOrRef;
				writer.Write(method.MDToken.ToInt32());
			}
		}

		// Token: 0x060000B4 RID: 180 RVA: 0x0000C11C File Offset: 0x0000A31C
		public static void EmitTok(this BinaryWriter writer, Instruction instr)
		{
			writer.Write(10);
			object operand = instr.Operand;
			IField field = operand as IField;
			bool flag = field != null;
			if (flag)
			{
				writer.Write(field.MDToken.ToInt32());
				writer.Write(0);
			}
			else
			{
				ITypeDefOrRef type = operand as ITypeDefOrRef;
				bool flag2 = type != null;
				if (flag2)
				{
					writer.Write(type.MDToken.ToInt32());
					writer.Write(1);
				}
				else
				{
					writer.Write((operand as IMethodDefOrRef).MDToken.ToInt32());
					writer.Write(2);
				}
			}
		}

		// Token: 0x060000B5 RID: 181 RVA: 0x0000C1C3 File Offset: 0x0000A3C3
		public static void EmitBr(this BinaryWriter writer, int index)
		{
			writer.Write(11);
			writer.Write(index);
		}

		// Token: 0x060000B6 RID: 182 RVA: 0x0000C1D8 File Offset: 0x0000A3D8
		public static void EmitVar(this BinaryWriter writer, Instruction instr)
		{
			writer.Write(12);
			Local local = instr.Operand as Local;
			bool flag = local != null;
			if (flag)
			{
				writer.Write(local.Index);
				writer.Write(0);
			}
			else
			{
				Parameter param = instr.Operand as Parameter;
				bool flag2 = param != null;
				if (!flag2)
				{
					throw new NotSupportedException();
				}
				writer.Write(param.Index);
				writer.Write(1);
			}
		}

		// Token: 0x060000B7 RID: 183 RVA: 0x0000C250 File Offset: 0x0000A450
		public static void EmitSwitch(this BinaryWriter writer, List<Instruction> instrs, Instruction instr)
		{
			writer.Write(13);
			Instruction[] instructions = instr.Operand as Instruction[];
			writer.Write(instructions.Length);
			foreach (Instruction i in instructions)
			{
				int index = instrs.IndexOf(i);
				writer.Write(index);
			}
		}
	}
}
