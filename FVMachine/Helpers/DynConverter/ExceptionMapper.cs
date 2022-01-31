using System;
using System.Collections.Generic;
using System.IO;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace FVM.Helpers.DynConverter
{
	// Token: 0x02000030 RID: 48
	public class ExceptionMapper
	{
		// Token: 0x1700001C RID: 28
		// (get) Token: 0x060000B8 RID: 184 RVA: 0x0000C2A6 File Offset: 0x0000A4A6
		private IList<ExceptionHandler> Exceptions { get; }

		// Token: 0x060000B9 RID: 185 RVA: 0x0000C2AE File Offset: 0x0000A4AE
		public ExceptionMapper(MethodDef method)
		{
			this.Exceptions = method.Body.ExceptionHandlers;
		}

		// Token: 0x060000BA RID: 186 RVA: 0x0000C2CC File Offset: 0x0000A4CC
		public void MapAndWrite(BinaryWriter writer, Instruction instr)
		{
			int count = 0;
			List<int> list = new List<int>();
			foreach (ExceptionHandler exception in this.Exceptions)
			{
				bool flag = exception.TryStart == instr;
				if (flag)
				{
					list.Add(0);
					count++;
				}
				else
				{
					bool flag2 = exception.HandlerEnd == instr;
					if (flag2)
					{
						list.Add(5);
						count++;
					}
					else
					{
						bool flag3 = exception.HandlerType == ExceptionHandlerType.Filter;
						if (flag3)
						{
							bool flag4 = exception.FilterStart == instr;
							if (flag4)
							{
								list.Add(1);
								count++;
								continue;
							}
						}
						bool flag5 = exception.HandlerStart == instr;
						if (flag5)
						{
							switch (exception.HandlerType)
							{
							case ExceptionHandlerType.Catch:
							{
								list.Add(2);
								bool flag6 = exception.CatchType == null;
								if (flag6)
								{
									list.Add(-1);
								}
								else
								{
									list.Add(exception.CatchType.MDToken.ToInt32());
								}
								break;
							}
							case ExceptionHandlerType.Finally:
								list.Add(3);
								break;
							case ExceptionHandlerType.Fault:
								list.Add(4);
								break;
							}
							count++;
						}
					}
				}
			}
			writer.Write(count);
			foreach (int i in list)
			{
				writer.Write(i);
			}
		}
	}
}
