// <copyright file="TreeEnumeratorAdvisorExtensions.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using Zaaml.Core.Trees;

namespace Zaaml.Core.Extensions
{
	internal static class TreeEnumeratorAdvisorExtensions
	{
		#region  Methods

		public static ITreeEnumerator<T> GetEnumerator<T>(this ITreeEnumeratorAdvisor<T> advisor, T root) where T : class
		{
			return TreeEnumerator.GetEnumerator(root, advisor);
		}

		public static ITreeEnumerator<T> GetEnumerator<T>(this ITreeEnumeratorAdvisor<T> advisor, IEnumerable<T> treeItems) where T : class
		{
			return TreeEnumerator.GetEnumerator(treeItems, advisor);
		}

		public static ITreeEnumerator<T> GetEnumerator<T>(this ITreeEnumeratorAdvisor<T> advisor, IEnumerator<T> treeItemsEnumerator) where T : class
		{
			return TreeEnumerator.GetEnumerator(treeItemsEnumerator, advisor);
		}

		public static ITreeEnumerator<T> GetReverseEnumerator<T>(this ITreeEnumeratorAdvisor<T> advisor, T root) where T : class
		{
			return TreeEnumerator.GetReverseEnumerator(root, advisor);
		}

		public static ITreeEnumerator<T> GetReverseEnumerator<T>(this ITreeEnumeratorAdvisor<T> advisor, IEnumerable<T> treeItems) where T : class
		{
			return TreeEnumerator.GetReverseEnumerator(treeItems, advisor);
		}

		public static ITreeEnumerator<T> GetReverseEnumerator<T>(this ITreeEnumeratorAdvisor<T> advisor, IEnumerator<T> treeItemsEnumerator) where T : class
		{
			return TreeEnumerator.GetReverseEnumerator(treeItemsEnumerator, advisor);
		}


		public static void ReverseVisit<T>(this ITreeEnumeratorAdvisor<T> iteratorAdvisor, T root, Action<T> visitor) where T : class
		{
			using var enumerator = TreeEnumerator.GetReverseEnumerator(root, iteratorAdvisor);

			while (enumerator.MoveNext())
				visitor(enumerator.Current);
		}

		public static void ReverseVisit<T>(this ITreeEnumeratorAdvisor<T> iteratorAdvisor, IEnumerable<T> treeItems, Action<T> visitor) where T : class
		{
			using var enumerator = TreeEnumerator.GetReverseEnumerator(treeItems, iteratorAdvisor);

			while (enumerator.MoveNext())
				visitor(enumerator.Current);
		}

		public static void ReverseVisit<T>(this ITreeEnumeratorAdvisor<T> iteratorAdvisor, IEnumerator<T> treeItemsEnumerator, Action<T> visitor) where T : class
		{
			using var enumerator = TreeEnumerator.GetReverseEnumerator(treeItemsEnumerator, iteratorAdvisor);

			while (enumerator.MoveNext())
				visitor(enumerator.Current);
		}

		public static void ReverseVisit<T>(this ITreeEnumeratorAdvisor<T> iteratorAdvisor, T root, Action<T, AncestorsEnumerator<T>> visitor) where T : class
		{
			using var enumerator = TreeEnumerator.GetReverseEnumerator(root, iteratorAdvisor);

			while (enumerator.MoveNext())
				visitor(enumerator.Current, enumerator.CurrentAncestors);
		}

		public static void ReverseVisit<T>(this ITreeEnumeratorAdvisor<T> iteratorAdvisor, IEnumerable<T> treeItems, Action<T, AncestorsEnumerator<T>> visitor) where T : class
		{
			using var enumerator = TreeEnumerator.GetReverseEnumerator(treeItems, iteratorAdvisor);

			while (enumerator.MoveNext())
				visitor(enumerator.Current, enumerator.CurrentAncestors);
		}

		public static void ReverseVisit<T>(this ITreeEnumeratorAdvisor<T> iteratorAdvisor, IEnumerator<T> treeItemsEnumerator, Action<T, AncestorsEnumerator<T>> visitor) where T : class
		{
			using var enumerator = TreeEnumerator.GetReverseEnumerator(treeItemsEnumerator, iteratorAdvisor);
			while (enumerator.MoveNext())
				visitor(enumerator.Current, enumerator.CurrentAncestors);
		}

		public static void Visit<T>(this ITreeEnumeratorAdvisor<T> iteratorAdvisor, T root, Action<T> visitor) where T : class
		{
			using var enumerator = TreeEnumerator.GetEnumerator(root, iteratorAdvisor);

			while (enumerator.MoveNext())
				visitor(enumerator.Current);
		}

		public static void Visit<T>(this ITreeEnumeratorAdvisor<T> iteratorAdvisor, IEnumerable<T> treeItems, Action<T> visitor) where T : class
		{
			using var enumerator = TreeEnumerator.GetEnumerator(treeItems, iteratorAdvisor);

			while (enumerator.MoveNext())
				visitor(enumerator.Current);
		}

		public static void Visit<T>(this ITreeEnumeratorAdvisor<T> iteratorAdvisor, IEnumerator<T> treeItemsEnumerator, Action<T> visitor) where T : class
		{
			using var enumerator = TreeEnumerator.GetEnumerator(treeItemsEnumerator, iteratorAdvisor);

			while (enumerator.MoveNext())
				visitor(enumerator.Current);
		}

		public static void Visit<T>(this ITreeEnumeratorAdvisor<T> iteratorAdvisor, T root, Action<T, AncestorsEnumerator<T>> visitor) where T : class
		{
			using var enumerator = TreeEnumerator.GetEnumerator(root, iteratorAdvisor);

			while (enumerator.MoveNext())
				visitor(enumerator.Current, enumerator.CurrentAncestors);
		}

		public static void Visit<T>(this ITreeEnumeratorAdvisor<T> iteratorAdvisor, IEnumerable<T> treeItems, Action<T, AncestorsEnumerator<T>> visitor) where T : class
		{
			using var enumerator = TreeEnumerator.GetEnumerator(treeItems, iteratorAdvisor);

			while (enumerator.MoveNext())
				visitor(enumerator.Current, enumerator.CurrentAncestors);
		}

		public static void Visit<T>(this ITreeEnumeratorAdvisor<T> iteratorAdvisor, IEnumerator<T> treeItemsEnumerator, Action<T, AncestorsEnumerator<T>> visitor) where T : class
		{
			using var enumerator = TreeEnumerator.GetEnumerator(treeItemsEnumerator, iteratorAdvisor);

			while (enumerator.MoveNext())
				visitor(enumerator.Current, enumerator.CurrentAncestors);
		}

		#endregion
	}
}
