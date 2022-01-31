using System;
using System.Collections.Generic;
using System.Linq;
using dnlib.DotNet;

namespace FVM
{
	// Token: 0x02000004 RID: 4
	public class Injector
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x0600000E RID: 14 RVA: 0x00002C24 File Offset: 0x00000E24
		public ModuleDefMD TargetModule { get; }

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x0600000F RID: 15 RVA: 0x00002C2C File Offset: 0x00000E2C
		public Type RuntimeType { get; }

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x06000010 RID: 16 RVA: 0x00002C34 File Offset: 0x00000E34
		public List<IDnlibDef> Members { get; }

		// Token: 0x06000011 RID: 17 RVA: 0x00002C3C File Offset: 0x00000E3C
		public Injector(ModuleDefMD targetModule, Type type, bool injectType = true)
		{
			this.TargetModule = targetModule;
			this.RuntimeType = type;
			this.Members = new List<IDnlibDef>();
			if (injectType)
			{
				this.InjectType();
			}
		}

		// Token: 0x06000012 RID: 18 RVA: 0x00002C78 File Offset: 0x00000E78
		public void InjectType()
		{
			ModuleDefMD typeModule = ModuleDefMD.Load(this.RuntimeType.Module);
			TypeDef typeDefs = typeModule.ResolveTypeDef(MDToken.ToRID(this.RuntimeType.MetadataToken));
			this.Members.AddRange(InjectHelper.Inject(typeDefs, this.TargetModule.GlobalType, this.TargetModule).ToList<IDnlibDef>());
		}

		// Token: 0x06000013 RID: 19 RVA: 0x00002CD8 File Offset: 0x00000ED8
		public IDnlibDef FindMember(string name)
		{
			foreach (IDnlibDef member in this.Members)
			{
				bool flag = member.Name == name;
				if (flag)
				{
					return member;
				}
			}
			throw new Exception("Error to find member.");
		}

		// Token: 0x06000014 RID: 20 RVA: 0x00002D48 File Offset: 0x00000F48
		public void Rename()
		{
			foreach (IDnlibDef mem in this.Members)
			{
				MethodDef method = mem as MethodDef;
				bool flag = method != null;
				if (flag)
				{
					bool hasImplMap = method.HasImplMap;
					if (hasImplMap)
					{
						continue;
					}
					bool isDelegate = method.DeclaringType.IsDelegate;
					if (isDelegate)
					{
						continue;
					}
				}
				mem.Name = Utils.GetRealString();
			}
		}
	}
}
