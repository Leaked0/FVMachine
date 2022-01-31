using System;
using System.Collections.Generic;

namespace FVM.Protections.ControlFlow
{
	// Token: 0x02000018 RID: 24
	public static class Utils
	{
		// Token: 0x06000061 RID: 97 RVA: 0x00009F34 File Offset: 0x00008134
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

		// Token: 0x06000062 RID: 98 RVA: 0x00009F84 File Offset: 0x00008184
		public static IList<T> RemoveWhere<T>(this IList<T> self, Predicate<T> match)
		{
			for (int i = self.Count - 1; i >= 0; i--)
			{
				bool flag = match(self[i]);
				if (flag)
				{
					self.RemoveAt(i);
				}
			}
			return self;
		}
	}
}
