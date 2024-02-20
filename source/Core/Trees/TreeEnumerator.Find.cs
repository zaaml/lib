// <copyright file="TreeEnumerator.Find.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;

namespace Zaaml.Core.Trees
{
	internal static partial class TreeEnumerator
	{
		public static T Find<T>(T root, ITreeEnumeratorAdvisor<T> treeAdvisor, Func<T, bool> predicate)
		{
			return GetEnumerable(root, treeAdvisor).FirstOrDefault(predicate, default);
		}

		public static T Find<T>(T root, ITreeEnumeratorAdvisor<T> treeAdvisor, Func<T, bool> predicate, T defaultValue)
		{
			return GetEnumerable(root, treeAdvisor).FirstOrDefault(predicate, defaultValue);
		}

		public static T Find<T>(IEnumerable<T> treeItems, ITreeEnumeratorAdvisor<T> treeAdvisor, Func<T, bool> predicate)
		{
			return GetEnumerable(treeItems, treeAdvisor).FirstOrDefault(predicate, default);
		}

		public static T Find<T>(IEnumerable<T> treeItems, ITreeEnumeratorAdvisor<T> treeAdvisor, Func<T, bool> predicate, T defaultValue)
		{
			return GetEnumerable(treeItems, treeAdvisor).FirstOrDefault(predicate, defaultValue);
		}

		public static T Find<T>(IEnumerator<T> treeItemsEnumerator, ITreeEnumeratorAdvisor<T> treeAdvisor, Func<T, bool> predicate)
		{
			return GetEnumerable(treeItemsEnumerator, treeAdvisor).FirstOrDefault(predicate, default);
		}

		public static T Find<T>(IEnumerator<T> treeItemsEnumerator, ITreeEnumeratorAdvisor<T> treeAdvisor, Func<T, bool> predicate, T defaultValue)
		{
			return GetEnumerable(treeItemsEnumerator, treeAdvisor).FirstOrDefault(predicate, defaultValue);
		}

		public static T FindReverse<T>(T root, ITreeEnumeratorAdvisor<T> treeAdvisor, Func<T, bool> predicate)
		{
			return GetReverseEnumerable(root, treeAdvisor).FirstOrDefault(predicate, default);
		}

		public static T FindReverse<T>(T root, ITreeEnumeratorAdvisor<T> treeAdvisor, Func<T, bool> predicate, T defaultValue)
		{
			return GetReverseEnumerable(root, treeAdvisor).FirstOrDefault(predicate, defaultValue);
		}

		public static T FindReverse<T>(IEnumerable<T> treeItems, ITreeEnumeratorAdvisor<T> treeAdvisor, Func<T, bool> predicate)
		{
			return GetReverseEnumerable(treeItems, treeAdvisor).FirstOrDefault(predicate, default);
		}

		public static T FindReverse<T>(IEnumerable<T> treeItems, ITreeEnumeratorAdvisor<T> treeAdvisor, Func<T, bool> predicate, T defaultValue)
		{
			return GetReverseEnumerable(treeItems, treeAdvisor).FirstOrDefault(predicate, defaultValue);
		}

		public static T FindReverse<T>(IEnumerator<T> treeItemsEnumerator, ITreeEnumeratorAdvisor<T> treeAdvisor, Func<T, bool> predicate)
		{
			return GetReverseEnumerable(treeItemsEnumerator, treeAdvisor).FirstOrDefault(predicate, default);
		}

		public static T FindReverse<T>(IEnumerator<T> treeItemsEnumerator, ITreeEnumeratorAdvisor<T> treeAdvisor, Func<T, bool> predicate, T defaultValue)
		{
			return GetReverseEnumerable(treeItemsEnumerator, treeAdvisor).FirstOrDefault(predicate, defaultValue);
		}
		
		public static T Find<T>(T root, Func<T, IEnumerable<T>> childrenFactory, Func<T, bool> predicate)
		{
			return GetEnumerable(root, childrenFactory).FirstOrDefault(predicate, default);
		}

		public static T Find<T>(T root, Func<T, IEnumerable<T>> childrenFactory, Func<T, bool> predicate, T defaultValue)
		{
			return GetEnumerable(root, childrenFactory).FirstOrDefault(predicate, defaultValue);
		}

		public static T Find<T>(IEnumerable<T> treeItems, Func<T, IEnumerable<T>> childrenFactory, Func<T, bool> predicate)
		{
			return GetEnumerable(treeItems, childrenFactory).FirstOrDefault(predicate, default);
		}

		public static T Find<T>(IEnumerable<T> treeItems, Func<T, IEnumerable<T>> childrenFactory, Func<T, bool> predicate, T defaultValue)
		{
			return GetEnumerable(treeItems, childrenFactory).FirstOrDefault(predicate, defaultValue);
		}

		public static T Find<T>(IEnumerator<T> treeItemsEnumerator, Func<T, IEnumerable<T>> childrenFactory, Func<T, bool> predicate)
		{
			return GetEnumerable(treeItemsEnumerator, childrenFactory).FirstOrDefault(predicate, default);
		}

		public static T Find<T>(IEnumerator<T> treeItemsEnumerator, Func<T, IEnumerable<T>> childrenFactory, Func<T, bool> predicate, T defaultValue)
		{
			return GetEnumerable(treeItemsEnumerator, childrenFactory).FirstOrDefault(predicate, defaultValue);
		}

		public static T FindReverse<T>(T root, Func<T, IEnumerable<T>> childrenFactory, Func<T, bool> predicate)
		{
			return GetReverseEnumerable(root, childrenFactory).FirstOrDefault(predicate, default);
		}

		public static T FindReverse<T>(T root, Func<T, IEnumerable<T>> childrenFactory, Func<T, bool> predicate, T defaultValue)
		{
			return GetReverseEnumerable(root, childrenFactory).FirstOrDefault(predicate, defaultValue);
		}

		public static T FindReverse<T>(IEnumerable<T> treeItems, Func<T, IEnumerable<T>> childrenFactory, Func<T, bool> predicate)
		{
			return GetReverseEnumerable(treeItems, childrenFactory).FirstOrDefault(predicate, default);
		}

		public static T FindReverse<T>(IEnumerable<T> treeItems, Func<T, IEnumerable<T>> childrenFactory, Func<T, bool> predicate, T defaultValue)
		{
			return GetReverseEnumerable(treeItems, childrenFactory).FirstOrDefault(predicate, defaultValue);
		}

		public static T FindReverse<T>(IEnumerator<T> treeItemsEnumerator, Func<T, IEnumerable<T>> childrenFactory, Func<T, bool> predicate)
		{
			return GetReverseEnumerable(treeItemsEnumerator, childrenFactory).FirstOrDefault(predicate, default);
		}

		public static T FindReverse<T>(IEnumerator<T> treeItemsEnumerator, Func<T, IEnumerable<T>> childrenFactory, Func<T, bool> predicate, T defaultValue)
		{
			return GetReverseEnumerable(treeItemsEnumerator, childrenFactory).FirstOrDefault(predicate, defaultValue);
		}

		public static T Find<T>(T root, Func<T, IEnumerator<T>> childrenFactory, Func<T, bool> predicate)
		{
			return GetEnumerable(root, childrenFactory).FirstOrDefault(predicate, default);
		}

		public static T Find<T>(T root, Func<T, IEnumerator<T>> childrenFactory, Func<T, bool> predicate, T defaultValue)
		{
			return GetEnumerable(root, childrenFactory).FirstOrDefault(predicate, defaultValue);
		}

		public static T Find<T>(IEnumerable<T> treeItems, Func<T, IEnumerator<T>> childrenFactory, Func<T, bool> predicate)
		{
			return GetEnumerable(treeItems, childrenFactory).FirstOrDefault(predicate, default);
		}

		public static T Find<T>(IEnumerable<T> treeItems, Func<T, IEnumerator<T>> childrenFactory, Func<T, bool> predicate, T defaultValue)
		{
			return GetEnumerable(treeItems, childrenFactory).FirstOrDefault(predicate, defaultValue);
		}

		public static T Find<T>(IEnumerator<T> treeItemsEnumerator, Func<T, IEnumerator<T>> childrenFactory, Func<T, bool> predicate)
		{
			return GetEnumerable(treeItemsEnumerator, childrenFactory).FirstOrDefault(predicate, default);
		}

		public static T Find<T>(IEnumerator<T> treeItemsEnumerator, Func<T, IEnumerator<T>> childrenFactory, Func<T, bool> predicate, T defaultValue)
		{
			return GetEnumerable(treeItemsEnumerator, childrenFactory).FirstOrDefault(predicate, defaultValue);
		}

		public static T FindReverse<T>(T root, Func<T, IEnumerator<T>> childrenFactory, Func<T, bool> predicate)
		{
			return GetReverseEnumerable(root, childrenFactory).FirstOrDefault(predicate, default);
		}

		public static T FindReverse<T>(T root, Func<T, IEnumerator<T>> childrenFactory, Func<T, bool> predicate, T defaultValue)
		{
			return GetReverseEnumerable(root, childrenFactory).FirstOrDefault(predicate, defaultValue);
		}

		public static T FindReverse<T>(IEnumerable<T> treeItems, Func<T, IEnumerator<T>> childrenFactory, Func<T, bool> predicate)
		{
			return GetReverseEnumerable(treeItems, childrenFactory).FirstOrDefault(predicate, default);
		}

		public static T FindReverse<T>(IEnumerable<T> treeItems, Func<T, IEnumerator<T>> childrenFactory, Func<T, bool> predicate, T defaultValue)
		{
			return GetReverseEnumerable(treeItems, childrenFactory).FirstOrDefault(predicate, defaultValue);
		}

		public static T FindReverse<T>(IEnumerator<T> treeItemsEnumerator, Func<T, IEnumerator<T>> childrenFactory, Func<T, bool> predicate)
		{
			return GetReverseEnumerable(treeItemsEnumerator, childrenFactory).FirstOrDefault(predicate, default);
		}

		public static T FindReverse<T>(IEnumerator<T> treeItemsEnumerator, Func<T, IEnumerator<T>> childrenFactory, Func<T, bool> predicate, T defaultValue)
		{
			return GetReverseEnumerable(treeItemsEnumerator, childrenFactory).FirstOrDefault(predicate, defaultValue);
		}
	}
}