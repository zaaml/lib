// <copyright file="TreeEnumerator.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;

namespace Zaaml.Core.Trees
{
	internal static class TreeEnumerator
	{
		public static ITreeEnumerator<T> GetEnumerator<T>(T root, ITreeEnumeratorAdvisor<T> treeAdvisor)
		{
			return new DirectTreeEnumerator<T>(root, treeAdvisor);
		}

		public static ITreeEnumerator<T> GetEnumerator<T>(IEnumerable<T> treeItems, ITreeEnumeratorAdvisor<T> treeAdvisor)
		{
			return new DirectTreeEnumerator<T>(treeItems, treeAdvisor);
		}

		public static ITreeEnumerator<T> GetEnumerator<T>(IEnumerator<T> treeItemsEnumerator, ITreeEnumeratorAdvisor<T> treeAdvisor)
		{
			return new DirectTreeEnumerator<T>(treeItemsEnumerator, treeAdvisor);
		}

		public static ITreeEnumerator<T> GetReverseEnumerator<T>(T root, ITreeEnumeratorAdvisor<T> treeAdvisor)
		{
			return new ReverseTreeEnumerator<T>(root, treeAdvisor);
		}

		public static ITreeEnumerator<T> GetReverseEnumerator<T>(IEnumerable<T> treeItems, ITreeEnumeratorAdvisor<T> treeAdvisor)
		{
			return new ReverseTreeEnumerator<T>(treeItems, treeAdvisor);
		}

		public static ITreeEnumerator<T> GetReverseEnumerator<T>(IEnumerator<T> treeItemsEnumerator, ITreeEnumeratorAdvisor<T> treeAdvisor)
		{
			return new ReverseTreeEnumerator<T>(treeItemsEnumerator, treeAdvisor);
		}

		public static void ReverseVisit<T>(T root, ITreeEnumeratorAdvisor<T> iteratorAdvisor, Action<T> visitor)
		{
			using var enumerator = GetReverseEnumerator(root, iteratorAdvisor);

			while (enumerator.MoveNext())
				visitor(enumerator.Current);
		}

		public static void ReverseVisit<T>(IEnumerable<T> treeItems, ITreeEnumeratorAdvisor<T> iteratorAdvisor, Action<T> visitor)
		{
			using var enumerator = GetReverseEnumerator(treeItems, iteratorAdvisor);

			while (enumerator.MoveNext())
				visitor(enumerator.Current);
		}

		public static void ReverseVisit<T>(IEnumerator<T> treeItemsEnumerator, ITreeEnumeratorAdvisor<T> iteratorAdvisor, Action<T> visitor)
		{
			using var enumerator = GetReverseEnumerator(treeItemsEnumerator, iteratorAdvisor);

			while (enumerator.MoveNext())
				visitor(enumerator.Current);
		}

		public static void ReverseVisit<T>(T root, ITreeEnumeratorAdvisor<T> iteratorAdvisor, Action<T, AncestorsEnumerator<T>> visitor)
		{
			using var enumerator = GetReverseEnumerator(root, iteratorAdvisor);

			while (enumerator.MoveNext())
				visitor(enumerator.Current, enumerator.CurrentAncestors);
		}

		public static void ReverseVisit<T>(IEnumerable<T> treeItems, ITreeEnumeratorAdvisor<T> iteratorAdvisor, Action<T, AncestorsEnumerator<T>> visitor)
		{
			using var enumerator = GetReverseEnumerator(treeItems, iteratorAdvisor);

			while (enumerator.MoveNext())
				visitor(enumerator.Current, enumerator.CurrentAncestors);
		}

		public static void ReverseVisit<T>(IEnumerator<T> treeItemsEnumerator, ITreeEnumeratorAdvisor<T> iteratorAdvisor, Action<T, AncestorsEnumerator<T>> visitor)
		{
			using var enumerator = GetReverseEnumerator(treeItemsEnumerator, iteratorAdvisor);

			while (enumerator.MoveNext())
				visitor(enumerator.Current, enumerator.CurrentAncestors);
		}

		public static void Visit<T>(T root, ITreeEnumeratorAdvisor<T> iteratorAdvisor, Action<T> visitor)
		{
			using var enumerator = GetEnumerator(root, iteratorAdvisor);

			while (enumerator.MoveNext())
				visitor(enumerator.Current);
		}

		public static void Visit<T>(IEnumerable<T> treeItems, ITreeEnumeratorAdvisor<T> iteratorAdvisor, Action<T> visitor)
		{
			using var enumerator = GetEnumerator(treeItems, iteratorAdvisor);

			while (enumerator.MoveNext())
				visitor(enumerator.Current);
		}

		public static void Visit<T>(IEnumerator<T> treeItemsEnumerator, ITreeEnumeratorAdvisor<T> iteratorAdvisor, Action<T> visitor)
		{
			using var enumerator = GetEnumerator(treeItemsEnumerator, iteratorAdvisor);

			while (enumerator.MoveNext())
				visitor(enumerator.Current);
		}

		public static void Visit<T>(T root, ITreeEnumeratorAdvisor<T> iteratorAdvisor, Action<T, AncestorsEnumerator<T>> visitor)
		{
			using var enumerator = GetEnumerator(root, iteratorAdvisor);

			while (enumerator.MoveNext())
				visitor(enumerator.Current, enumerator.CurrentAncestors);
		}

		public static void Visit<T>(IEnumerable<T> treeItems, ITreeEnumeratorAdvisor<T> iteratorAdvisor, Action<T, AncestorsEnumerator<T>> visitor)
		{
			using var enumerator = GetEnumerator(treeItems, iteratorAdvisor);

			while (enumerator.MoveNext())
				visitor(enumerator.Current, enumerator.CurrentAncestors);
		}

		public static void Visit<T>(IEnumerator<T> treeItemsEnumerator, ITreeEnumeratorAdvisor<T> iteratorAdvisor, Action<T, AncestorsEnumerator<T>> visitor)
		{
			using var enumerator = GetEnumerator(treeItemsEnumerator, iteratorAdvisor);

			while (enumerator.MoveNext())
				visitor(enumerator.Current, enumerator.CurrentAncestors);
		}
	}
}