using System;

namespace Challenge
{
	// Token: 0x02000002 RID: 2
	internal class Program
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002090 File Offset: 0x00000290
		private static void Main(string[] args)
		{
			Console.WriteLine(sizeof(int));
			Console.WriteLine(Math.Pow(36.0, 33.0));
			Program.Method();
			Console.WriteLine(Program.ReturnMethod());
			Program.CondMethod();
			Program.CmpMethod();
			Program.Cmp2Method();
			Program.LoopMethod();
			Program.CondLoopMethod();
			Console.ReadLine();
		}

		// Token: 0x06000002 RID: 2 RVA: 0x00002050 File Offset: 0x00000250
		public static void Method()
		{
			Console.WriteLine("Method Executing...");
		}

		// Token: 0x06000003 RID: 3 RVA: 0x0000205C File Offset: 0x0000025C
		public static void CondMethod()
		{
			Console.WriteLine("Cond Method Executing...");
			if (Console.ReadLine() == "VMPKEY_SALPASKOPSOAKP")
			{
				Console.WriteLine("VMP KEY OK.");
			}
		}

		// Token: 0x06000004 RID: 4 RVA: 0x000020F4 File Offset: 0x000002F4
		public static void CmpMethod()
		{
			int num = 100;
			if (num > 90)
			{
				Console.WriteLine("Cmp " + num.ToString());
				return;
			}
			Console.WriteLine("Cmp Plus " + num.ToString() + 100.ToString());
		}

		// Token: 0x06000005 RID: 5 RVA: 0x00002144 File Offset: 0x00000344
		public static void Cmp2Method()
		{
			int num = 100;
			if (num < 90)
			{
				Console.WriteLine("Cmp2 " + num.ToString());
				return;
			}
			Console.WriteLine("Cmp2 Plus " + num.ToString() + 100.ToString());
		}

		// Token: 0x06000006 RID: 6 RVA: 0x00002194 File Offset: 0x00000394
		public static void LoopMethod()
		{
			for (int i = 0; i < 2; i++)
			{
				Console.WriteLine(string.Format("Loop {0}", i));
			}
			Console.WriteLine("Finished Loop");
		}

		// Token: 0x06000007 RID: 7 RVA: 0x000021CC File Offset: 0x000003CC
		public static void CondLoopMethod()
		{
			bool flag = false;
			for (int i = 0; i < (flag ? 5 : 6); i++)
			{
				int num = 3;
				while (Convert.ToInt32(flag) > i)
				{
					num--;
					Console.WriteLine(string.Format("CondLoop {0}", num));
				}
			}
		}

		// Token: 0x06000009 RID: 9 RVA: 0x0000208B File Offset: 0x0000028B
		public static int ReturnMethod()
		{
			return 100;
		}
	}
}
