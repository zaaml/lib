// <copyright file="EnumeratorUtils.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;

namespace Zaaml.Core.Utils
{
	internal static class EnumeratorUtils
	{
		private static readonly object[] EmptyArray = Array.Empty<object>();

		public static IEnumerator EmptyEnumerator => EmptyArray.GetEnumerator();

		public static JoinEnumerator<T> Concat<T>(T item, IEnumerator<T> second)
		{
			return new JoinEnumerator<T>(new[] { new SingleItemEnumerator<T>(item), second });
		}

		public static JoinEnumerator<T> Concat<T>(IEnumerator<T> first, IEnumerator<T> second)
		{
			return new JoinEnumerator<T>(new[] { first, second });
		}

		public static JoinEnumerator<T> Concat<T>(IEnumerator<T> first, IEnumerator<T> second, IEnumerator<T> third)
		{
			return new JoinEnumerator<T>(new[] { first, second, third });
		}

		public static JoinEnumerator Concat(object item, IEnumerator second)
		{
			return new JoinEnumerator(new[] { new SingleItemEnumerator(item), second });
		}

		public static JoinEnumerator Concat(IEnumerator first, IEnumerator second)
		{
			return new JoinEnumerator(new[] { first, second });
		}

		public static JoinEnumerator Concat(IEnumerator first, IEnumerator second, IEnumerator third)
		{
			return new JoinEnumerator(new[] { first, second, third });
		}

		public static IEnumerable<T> Enumerate<T>(IEnumerator<T> enumerator)
		{
			while (enumerator.MoveNext())
				yield return enumerator.Current;
		}

		public static IEnumerable<object> Enumerate(IEnumerator enumerator)
		{
			while (enumerator.MoveNext())
				yield return enumerator.Current;
		}

		public static void MoveToEnd(IEnumerator enumerator)
		{
			while (enumerator.MoveNext())
			{
			}
		}

		public static void Visit<T>(IEnumerator<T> enumerator, Func<T, bool> visitor)
		{
			while (enumerator.MoveNext())
				if (visitor(enumerator.Current) == false)
					break;
		}

		public static void Visit<T>(IEnumerator<T> enumerator, Action<T> visitor)
		{
			while (enumerator.MoveNext())
				visitor(enumerator.Current);
		}
	}
}