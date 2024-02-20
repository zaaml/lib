// <copyright file="TreeEnumerator.Enumerable.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;

namespace Zaaml.Core.Trees
{
	internal static partial class TreeEnumerator
	{
		public static IEnumerable<T> GetEnumerable<T>(T root, ITreeEnumeratorAdvisor<T> treeAdvisor)
		{
			return new DirectTreeEnumerator<T>(root, treeAdvisor).Enumerate();
		}

		public static IEnumerable<T> GetEnumerable<T>(IEnumerable<T> treeItems, ITreeEnumeratorAdvisor<T> treeAdvisor)
		{
			return new DirectTreeEnumerator<T>(treeItems, treeAdvisor).Enumerate();
		}

		public static IEnumerable<T> GetEnumerable<T>(IEnumerator<T> treeItemsEnumerator, ITreeEnumeratorAdvisor<T> treeAdvisor)
		{
			return new DirectTreeEnumerator<T>(treeItemsEnumerator, treeAdvisor).Enumerate();
		}

		public static IEnumerable<T> GetReverseEnumerable<T>(T root, ITreeEnumeratorAdvisor<T> treeAdvisor)
		{
			return new ReverseTreeEnumerator<T>(root, treeAdvisor).Enumerate();
		}

		public static IEnumerable<T> GetReverseEnumerable<T>(IEnumerable<T> treeItems, ITreeEnumeratorAdvisor<T> treeAdvisor)
		{
			return new ReverseTreeEnumerator<T>(treeItems, treeAdvisor).Enumerate();
		}

		public static IEnumerable<T> GetReverseEnumerable<T>(IEnumerator<T> treeItemsEnumerator, ITreeEnumeratorAdvisor<T> treeAdvisor)
		{
			return new ReverseTreeEnumerator<T>(treeItemsEnumerator, treeAdvisor).Enumerate();
		}		
		
		public static IEnumerable<T> GetEnumerable<T>(T root, Func<T, IEnumerable<T>> childrenFactory)
		{
			return GetEnumerator(root, childrenFactory).Enumerate();
		}

		public static IEnumerable<T> GetEnumerable<T>(IEnumerable<T> treeItems, Func<T, IEnumerable<T>> childrenFactory)
		{
			return GetEnumerator(treeItems, childrenFactory).Enumerate();
		}

		public static IEnumerable<T> GetEnumerable<T>(IEnumerator<T> treeItemsEnumerator, Func<T, IEnumerable<T>> childrenFactory)
		{
			return GetEnumerator(treeItemsEnumerator, childrenFactory).Enumerate();
		}

		public static IEnumerable<T> GetReverseEnumerable<T>(T root, Func<T, IEnumerable<T>> childrenFactory)
		{
			return GetReverseEnumerator(root, childrenFactory).Enumerate();
		}

		public static IEnumerable<T> GetReverseEnumerable<T>(IEnumerable<T> treeItems, Func<T, IEnumerable<T>> childrenFactory)
		{
			return GetReverseEnumerator(treeItems, childrenFactory).Enumerate();
		}

		public static IEnumerable<T> GetReverseEnumerable<T>(IEnumerator<T> treeItemsEnumerator, Func<T, IEnumerable<T>> childrenFactory)
		{
			return GetReverseEnumerator(treeItemsEnumerator, childrenFactory).Enumerate();
		}		
		
		public static IEnumerable<T> GetEnumerable<T>(T root, Func<T, IEnumerator<T>> childrenFactory)
		{
			return GetEnumerator(root, childrenFactory).Enumerate();
		}

		public static IEnumerable<T> GetEnumerable<T>(IEnumerable<T> treeItems, Func<T, IEnumerator<T>> childrenFactory)
		{
			return GetEnumerator(treeItems, childrenFactory).Enumerate();
		}

		public static IEnumerable<T> GetEnumerable<T>(IEnumerator<T> treeItemsEnumerator, Func<T, IEnumerator<T>> childrenFactory)
		{
			return GetEnumerator(treeItemsEnumerator, childrenFactory).Enumerate();
		}

		public static IEnumerable<T> GetReverseEnumerable<T>(T root, Func<T, IEnumerator<T>> childrenFactory)
		{
			return GetReverseEnumerator(root, childrenFactory).Enumerate();
		}

		public static IEnumerable<T> GetReverseEnumerable<T>(IEnumerable<T> treeItems, Func<T, IEnumerator<T>> childrenFactory)
		{
			return GetReverseEnumerator(treeItems, childrenFactory).Enumerate();
		}

		public static IEnumerable<T> GetReverseEnumerable<T>(IEnumerator<T> treeItemsEnumerator, Func<T, IEnumerator<T>> childrenFactory)
		{
			return GetReverseEnumerator(treeItemsEnumerator, childrenFactory).Enumerate();
		}
	}
}