// <copyright file="TreeEnumerator.Visit.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;

namespace Zaaml.Core.Trees
{
	internal static partial class TreeEnumerator
	{
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

		public static void ReverseVisit<T>(T root, Func<T, IEnumerator<T>> childrenFactory, Action<T> visitor)
		{
			using var enumerator = GetReverseEnumerator(root, childrenFactory);

			while (enumerator.MoveNext())
				visitor(enumerator.Current);
		}

		public static void ReverseVisit<T>(IEnumerable<T> treeItems, Func<T, IEnumerator<T>> childrenFactory, Action<T> visitor)
		{
			using var enumerator = GetReverseEnumerator(treeItems, childrenFactory);

			while (enumerator.MoveNext())
				visitor(enumerator.Current);
		}

		public static void ReverseVisit<T>(IEnumerator<T> treeItemsEnumerator, Func<T, IEnumerator<T>> childrenFactory, Action<T> visitor)
		{
			using var enumerator = GetReverseEnumerator(treeItemsEnumerator, childrenFactory);

			while (enumerator.MoveNext())
				visitor(enumerator.Current);
		}

		public static void ReverseVisit<T>(T root, Func<T, IEnumerator<T>> childrenFactory, Action<T, AncestorsEnumerator<T>> visitor)
		{
			using var enumerator = GetReverseEnumerator(root, childrenFactory);

			while (enumerator.MoveNext())
				visitor(enumerator.Current, enumerator.CurrentAncestors);
		}

		public static void ReverseVisit<T>(IEnumerable<T> treeItems, Func<T, IEnumerator<T>> childrenFactory, Action<T, AncestorsEnumerator<T>> visitor)
		{
			using var enumerator = GetReverseEnumerator(treeItems, childrenFactory);

			while (enumerator.MoveNext())
				visitor(enumerator.Current, enumerator.CurrentAncestors);
		}

		public static void ReverseVisit<T>(IEnumerator<T> treeItemsEnumerator, Func<T, IEnumerator<T>> childrenFactory, Action<T, AncestorsEnumerator<T>> visitor)
		{
			using var enumerator = GetReverseEnumerator(treeItemsEnumerator, childrenFactory);

			while (enumerator.MoveNext())
				visitor(enumerator.Current, enumerator.CurrentAncestors);
		}

		public static void Visit<T>(T root, Func<T, IEnumerator<T>> childrenFactory, Action<T> visitor)
		{
			using var enumerator = GetEnumerator(root, childrenFactory);

			while (enumerator.MoveNext())
				visitor(enumerator.Current);
		}

		public static void Visit<T>(IEnumerable<T> treeItems, Func<T, IEnumerator<T>> childrenFactory, Action<T> visitor)
		{
			using var enumerator = GetEnumerator(treeItems, childrenFactory);

			while (enumerator.MoveNext())
				visitor(enumerator.Current);
		}

		public static void Visit<T>(IEnumerator<T> treeItemsEnumerator, Func<T, IEnumerator<T>> childrenFactory, Action<T> visitor)
		{
			using var enumerator = GetEnumerator(treeItemsEnumerator, childrenFactory);

			while (enumerator.MoveNext())
				visitor(enumerator.Current);
		}

		public static void Visit<T>(T root, Func<T, IEnumerator<T>> childrenFactory, Action<T, AncestorsEnumerator<T>> visitor)
		{
			using var enumerator = GetEnumerator(root, childrenFactory);

			while (enumerator.MoveNext())
				visitor(enumerator.Current, enumerator.CurrentAncestors);
		}

		public static void Visit<T>(IEnumerable<T> treeItems, Func<T, IEnumerator<T>> childrenFactory, Action<T, AncestorsEnumerator<T>> visitor)
		{
			using var enumerator = GetEnumerator(treeItems, childrenFactory);

			while (enumerator.MoveNext())
				visitor(enumerator.Current, enumerator.CurrentAncestors);
		}

		public static void Visit<T>(IEnumerator<T> treeItemsEnumerator, Func<T, IEnumerator<T>> childrenFactory, Action<T, AncestorsEnumerator<T>> visitor)
		{
			using var enumerator = GetEnumerator(treeItemsEnumerator, childrenFactory);

			while (enumerator.MoveNext())
				visitor(enumerator.Current, enumerator.CurrentAncestors);
		}

		public static void ReverseVisit<T>(T root, Func<T, IEnumerable<T>> childrenFactory, Action<T> visitor)
		{
			using var enumerator = GetReverseEnumerator(root, childrenFactory);

			while (enumerator.MoveNext())
				visitor(enumerator.Current);
		}

		public static void ReverseVisit<T>(IEnumerable<T> treeItems, Func<T, IEnumerable<T>> childrenFactory, Action<T> visitor)
		{
			using var enumerator = GetReverseEnumerator(treeItems, childrenFactory);

			while (enumerator.MoveNext())
				visitor(enumerator.Current);
		}

		public static void ReverseVisit<T>(IEnumerator<T> treeItemsEnumerator, Func<T, IEnumerable<T>> childrenFactory, Action<T> visitor)
		{
			using var enumerator = GetReverseEnumerator(treeItemsEnumerator, childrenFactory);

			while (enumerator.MoveNext())
				visitor(enumerator.Current);
		}

		public static void ReverseVisit<T>(T root, Func<T, IEnumerable<T>> childrenFactory, Action<T, AncestorsEnumerator<T>> visitor)
		{
			using var enumerator = GetReverseEnumerator(root, childrenFactory);

			while (enumerator.MoveNext())
				visitor(enumerator.Current, enumerator.CurrentAncestors);
		}

		public static void ReverseVisit<T>(IEnumerable<T> treeItems, Func<T, IEnumerable<T>> childrenFactory, Action<T, AncestorsEnumerator<T>> visitor)
		{
			using var enumerator = GetReverseEnumerator(treeItems, childrenFactory);

			while (enumerator.MoveNext())
				visitor(enumerator.Current, enumerator.CurrentAncestors);
		}

		public static void ReverseVisit<T>(IEnumerator<T> treeItemsEnumerator, Func<T, IEnumerable<T>> childrenFactory, Action<T, AncestorsEnumerator<T>> visitor)
		{
			using var enumerator = GetReverseEnumerator(treeItemsEnumerator, childrenFactory);

			while (enumerator.MoveNext())
				visitor(enumerator.Current, enumerator.CurrentAncestors);
		}

		public static void Visit<T>(T root, Func<T, IEnumerable<T>> childrenFactory, Action<T> visitor)
		{
			using var enumerator = GetEnumerator(root, childrenFactory);

			while (enumerator.MoveNext())
				visitor(enumerator.Current);
		}

		public static void Visit<T>(IEnumerable<T> treeItems, Func<T, IEnumerable<T>> childrenFactory, Action<T> visitor)
		{
			using var enumerator = GetEnumerator(treeItems, childrenFactory);

			while (enumerator.MoveNext())
				visitor(enumerator.Current);
		}

		public static void Visit<T>(IEnumerator<T> treeItemsEnumerator, Func<T, IEnumerable<T>> childrenFactory, Action<T> visitor)
		{
			using var enumerator = GetEnumerator(treeItemsEnumerator, childrenFactory);

			while (enumerator.MoveNext())
				visitor(enumerator.Current);
		}

		public static void Visit<T>(T root, Func<T, IEnumerable<T>> childrenFactory, Action<T, AncestorsEnumerator<T>> visitor)
		{
			using var enumerator = GetEnumerator(root, childrenFactory);

			while (enumerator.MoveNext())
				visitor(enumerator.Current, enumerator.CurrentAncestors);
		}

		public static void Visit<T>(IEnumerable<T> treeItems, Func<T, IEnumerable<T>> childrenFactory, Action<T, AncestorsEnumerator<T>> visitor)
		{
			using var enumerator = GetEnumerator(treeItems, childrenFactory);

			while (enumerator.MoveNext())
				visitor(enumerator.Current, enumerator.CurrentAncestors);
		}

		public static void Visit<T>(IEnumerator<T> treeItemsEnumerator, Func<T, IEnumerable<T>> childrenFactory, Action<T, AncestorsEnumerator<T>> visitor)
		{
			using var enumerator = GetEnumerator(treeItemsEnumerator, childrenFactory);

			while (enumerator.MoveNext())
				visitor(enumerator.Current, enumerator.CurrentAncestors);
		}
	}
}