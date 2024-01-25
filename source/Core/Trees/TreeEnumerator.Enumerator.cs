// <copyright file="TreeEnumerator.Enumerator.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;

namespace Zaaml.Core.Trees
{
	internal static partial class TreeEnumerator
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

		public static ITreeEnumerator<T> GetEnumerator<T>(T root, Func<T, IEnumerable<T>> childrenFactory)
		{
			return new DirectTreeEnumerator<T>(root, childrenFactory.AsAdvisor());
		}

		public static ITreeEnumerator<T> GetEnumerator<T>(IEnumerable<T> treeItems, Func<T, IEnumerable<T>> childrenFactory)
		{
			return new DirectTreeEnumerator<T>(treeItems, childrenFactory.AsAdvisor());
		}

		public static ITreeEnumerator<T> GetEnumerator<T>(IEnumerator<T> treeItemsEnumerator, Func<T, IEnumerable<T>> childrenFactory)
		{
			return new DirectTreeEnumerator<T>(treeItemsEnumerator, childrenFactory.AsAdvisor());
		}

		public static ITreeEnumerator<T> GetReverseEnumerator<T>(T root, Func<T, IEnumerable<T>> childrenFactory)
		{
			return new ReverseTreeEnumerator<T>(root, childrenFactory.AsAdvisor());
		}

		public static ITreeEnumerator<T> GetReverseEnumerator<T>(IEnumerable<T> treeItems, Func<T, IEnumerable<T>> childrenFactory)
		{
			return new ReverseTreeEnumerator<T>(treeItems, childrenFactory.AsAdvisor());
		}

		public static ITreeEnumerator<T> GetReverseEnumerator<T>(IEnumerator<T> treeItemsEnumerator, Func<T, IEnumerable<T>> childrenFactory)
		{
			return new ReverseTreeEnumerator<T>(treeItemsEnumerator, childrenFactory.AsAdvisor());
		}
		
		public static ITreeEnumerator<T> GetEnumerator<T>(T root, Func<T, IEnumerator<T>> childrenFactory)
		{
			return new DirectTreeEnumerator<T>(root, childrenFactory.AsAdvisor());
		}

		public static ITreeEnumerator<T> GetEnumerator<T>(IEnumerable<T> treeItems, Func<T, IEnumerator<T>> childrenFactory)
		{
			return new DirectTreeEnumerator<T>(treeItems, childrenFactory.AsAdvisor());
		}

		public static ITreeEnumerator<T> GetEnumerator<T>(IEnumerator<T> treeItemsEnumerator, Func<T, IEnumerator<T>> childrenFactory)
		{
			return new DirectTreeEnumerator<T>(treeItemsEnumerator, childrenFactory.AsAdvisor());
		}

		public static ITreeEnumerator<T> GetReverseEnumerator<T>(T root, Func<T, IEnumerator<T>> childrenFactory)
		{
			return new ReverseTreeEnumerator<T>(root, childrenFactory.AsAdvisor());
		}

		public static ITreeEnumerator<T> GetReverseEnumerator<T>(IEnumerable<T> treeItems, Func<T, IEnumerator<T>> childrenFactory)
		{
			return new ReverseTreeEnumerator<T>(treeItems, childrenFactory.AsAdvisor());
		}

		public static ITreeEnumerator<T> GetReverseEnumerator<T>(IEnumerator<T> treeItemsEnumerator, Func<T, IEnumerator<T>> childrenFactory)
		{
			return new ReverseTreeEnumerator<T>(treeItemsEnumerator, childrenFactory.AsAdvisor());
		}
	}
}