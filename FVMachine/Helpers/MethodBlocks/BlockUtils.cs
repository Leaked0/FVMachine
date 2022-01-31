using System;
using System.Collections.Generic;

namespace FVM.Helpers.MethodBlocks
{
	// Token: 0x0200001E RID: 30
	public static class BlockUtils
	{
		// Token: 0x06000078 RID: 120 RVA: 0x0000ADB0 File Offset: 0x00008FB0
		public static void AddListEntry<TKey, TValue>(this IDictionary<TKey, List<TValue>> self, TKey key, TValue value)
		{
			bool flag = key == null;
			if (flag)
			{
				throw new ArgumentNullException("key");
			}
			List<TValue> list;
			bool flag2 = !self.TryGetValue(key, out list);
			if (flag2)
			{
				list = (self[key] = new List<TValue>());
			}
			list.Add(value);
		}
	}
}
