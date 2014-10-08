using System.Collections.Generic;

namespace Telerik.JustMock.Mef
{
	internal static class DictionaryExtensions
	{
		public static bool DictionaryEquals<TKey, TValue>(this IDictionary<TKey, TValue> left, IDictionary<TKey, TValue> right)
		{
			if (left == null && right == null)
			{
				return true;
			}
			if ((left == null) != (right == null))
			{
				return false;
			}

			return DictionaryContainedIn(left, right)
				&& DictionaryContainedIn(right, left);
		}

		private static bool DictionaryContainedIn<TKey, TValue>(IDictionary<TKey, TValue> left, IDictionary<TKey, TValue> right)
		{
			foreach (var kvp in left)
			{
				TValue value;
				if (!right.TryGetValue(kvp.Key, out value) || !object.Equals(kvp.Value, value))
				{
					return false;
				}
			}
			return true;
		}
	}
}
