using System;
using System.Collections.Generic;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace FVM.Protections.ControlFlow
{
	// Token: 0x02000014 RID: 20
	internal abstract class ManglerBase
	{
		// Token: 0x0600004B RID: 75 RVA: 0x00008A8F File Offset: 0x00006C8F
		protected static IEnumerable<BlockParser.InstrBlock> GetAllBlocks(BlockParser.ScopeBlock scope)
		{
			foreach (BlockParser.BlockBase child in scope.Children)
			{
				bool flag = child is BlockParser.InstrBlock;
				if (flag)
				{
					yield return (BlockParser.InstrBlock)child;
				}
				else
				{
					foreach (BlockParser.InstrBlock block in ManglerBase.GetAllBlocks((BlockParser.ScopeBlock)child))
					{
						yield return block;
						block = null;
					}
					IEnumerator<BlockParser.InstrBlock> enumerator2 = null;
				}
				child = null;
			}
			List<BlockParser.BlockBase>.Enumerator enumerator = default(List<BlockParser.BlockBase>.Enumerator);
			yield break;
			yield break;
		}

		// Token: 0x0600004C RID: 76
		public abstract void Mangle(CilBody body, BlockParser.ScopeBlock root, Context ctx, MethodDef method, TypeSig retType);
	}
}
