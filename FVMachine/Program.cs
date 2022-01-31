using System;
using FVM.Protections;
using FVM.Protections.VM;

namespace FVM
{
	// Token: 0x02000005 RID: 5
	internal class Program
	{
		// Token: 0x06000015 RID: 21 RVA: 0x00002DDC File Offset: 0x00000FDC
		private static void Main(string[] args)
		{
			Context context = new Context(args[0]);
			Protection[] protections = new Protection[]
			{
				new Virtualization(),
				new RenamerProtection()
			};
			foreach (Protection protection in protections)
			{
				protection.Execute(context);
			}
			context.Save();
		}
	}
}
