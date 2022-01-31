using System;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace FVM.Protections
{
	// Token: 0x02000009 RID: 9
	public class RenamerProtection : Protection
	{
		// Token: 0x06000024 RID: 36 RVA: 0x000068D4 File Offset: 0x00004AD4
		public override void Execute(Context context)
		{
			foreach (TypeDef type in context.Module.GetTypes())
			{
				foreach (MethodDef method in type.Methods)
				{
					bool flag = !method.HasBody;
					if (!flag)
					{
						bool flag2 = !method.Body.HasInstructions;
						if (!flag2)
						{
							foreach (ParamDef paramDef in method.ParamDefs)
							{
								paramDef.Name = Utils.GetRealString();
							}
							foreach (Local local in method.Body.Variables)
							{
								local.Name = Utils.GetRealString();
							}
							TypeDef declType = method.DeclaringType;
							bool flag3 = RenamerProtection.CanRename(declType, method);
							if (flag3)
							{
								method.Name = Utils.GetRealString();
							}
							bool flag4 = RenamerProtection.CanRename(declType);
							if (flag4)
							{
								declType.Name = Utils.GetRealString();
							}
							bool flag5 = !declType.Namespace.StartsWith("<");
							if (flag5)
							{
								declType.Namespace = Utils.GetRealString();
							}
							foreach (FieldDef field in declType.Fields)
							{
								bool flag6 = RenamerProtection.CanRename(field);
								if (flag6)
								{
									field.Name = Utils.GetRealString();
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06000025 RID: 37 RVA: 0x00006B40 File Offset: 0x00004D40
		public static bool CanRename(FieldDef field)
		{
			bool isSpecialName = field.IsSpecialName;
			bool result;
			if (isSpecialName)
			{
				result = false;
			}
			else
			{
				bool isRuntimeSpecialName = field.IsRuntimeSpecialName;
				result = !isRuntimeSpecialName;
			}
			return result;
		}

		// Token: 0x06000026 RID: 38 RVA: 0x00006B70 File Offset: 0x00004D70
		public static bool CanRename(TypeDef declType)
		{
			bool isGlobalModuleType = declType.IsGlobalModuleType;
			bool result;
			if (isGlobalModuleType)
			{
				result = false;
			}
			else
			{
				bool isInterface = declType.IsInterface;
				if (isInterface)
				{
					result = false;
				}
				else
				{
					bool isAbstract = declType.IsAbstract;
					if (isAbstract)
					{
						result = false;
					}
					else
					{
						bool isEnum = declType.IsEnum;
						if (isEnum)
						{
							result = false;
						}
						else
						{
							bool isRuntimeSpecialName = declType.IsRuntimeSpecialName;
							if (isRuntimeSpecialName)
							{
								result = false;
							}
							else
							{
								bool isSpecialName = declType.IsSpecialName;
								result = !isSpecialName;
							}
						}
					}
				}
			}
			return result;
		}

		// Token: 0x06000027 RID: 39 RVA: 0x00006BE0 File Offset: 0x00004DE0
		public static bool CanRename(TypeDef declType, MethodDef method)
		{
			bool isInterface = declType.IsInterface;
			bool result;
			if (isInterface)
			{
				result = false;
			}
			else
			{
				bool flag = declType.IsDelegate || declType.IsAbstract;
				if (flag)
				{
					result = false;
				}
				else
				{
					bool flag2 = method.IsSetter || method.IsGetter;
					if (flag2)
					{
						result = false;
					}
					else
					{
						bool flag3 = method.IsSpecialName || method.IsRuntimeSpecialName;
						if (flag3)
						{
							result = false;
						}
						else
						{
							bool isConstructor = method.IsConstructor;
							if (isConstructor)
							{
								result = false;
							}
							else
							{
								bool isVirtual = method.IsVirtual;
								if (isVirtual)
								{
									result = false;
								}
								else
								{
									bool isNative = method.IsNative;
									if (isNative)
									{
										result = false;
									}
									else
									{
										bool flag4 = method.IsPinvokeImpl || method.IsUnmanaged || method.IsUnmanagedExport;
										if (flag4)
										{
											result = false;
										}
										else
										{
											bool hasImplMap = method.HasImplMap;
											result = !hasImplMap;
										}
									}
								}
							}
						}
					}
				}
			}
			return result;
		}
	}
}
