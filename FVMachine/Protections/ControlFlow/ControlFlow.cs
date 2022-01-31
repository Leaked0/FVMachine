using System;
using System.Linq;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using dnlib.DotNet.Pdb;

namespace FVM.Protections.ControlFlow
{
	// Token: 0x0200000F RID: 15
	public class ControlFlow
	{
		// Token: 0x06000036 RID: 54 RVA: 0x00007C1C File Offset: 0x00005E1C
		public static void PhaseControlFlow(MethodDef method, Context context)
		{
			CilBody body = method.Body;
			body.SimplifyBranches();
			BlockParser.ScopeBlock root = BlockParser.ParseBody(body);
			new SwitchMangler().Mangle(body, root, context, method, method.ReturnType);
			body.Instructions.Clear();
			root.ToBody(body);
			bool flag = body.PdbMethod != null;
			if (flag)
			{
				body.PdbMethod = new PdbMethod
				{
					Scope = new PdbScope
					{
						Start = body.Instructions.First<Instruction>(),
						End = body.Instructions.Last<Instruction>()
					}
				};
			}
			method.CustomDebugInfos.RemoveWhere((PdbCustomDebugInfo cdi) => cdi is PdbStateMachineHoistedLocalScopesCustomDebugInfo);
			foreach (ExceptionHandler eh in body.ExceptionHandlers)
			{
				int index = body.Instructions.IndexOf(eh.TryEnd) + 1;
				eh.TryEnd = ((index < body.Instructions.Count) ? body.Instructions[index] : null);
				index = body.Instructions.IndexOf(eh.HandlerEnd) + 1;
				eh.HandlerEnd = ((index < body.Instructions.Count) ? body.Instructions[index] : null);
			}
		}
	}
}
