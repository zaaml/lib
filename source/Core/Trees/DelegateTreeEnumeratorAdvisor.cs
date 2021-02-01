// <copyright file="DelegateTreeEnumeratorAdvisor.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;

namespace Zaaml.Core.Trees
{
	internal class DelegateTreeEnumeratorAdvisor<T> : ITreeEnumeratorAdvisor<T>
	{
		private readonly Func<T, IEnumerator<T>> _childrenFactory;

		public DelegateTreeEnumeratorAdvisor(Func<T, IEnumerator<T>> childrenFactory)
		{
			_childrenFactory = childrenFactory;
		}

		public IEnumerator<T> GetChildren(T node) => _childrenFactory(node);
	}

	internal class DelegateTreeEnumerableAdvisor<T> : ITreeEnumeratorAdvisor<T>
	{
		private readonly Func<T, IEnumerable<T>> _childrenFactory;

		public DelegateTreeEnumerableAdvisor(Func<T, IEnumerable<T>> childrenFactory)
		{
			_childrenFactory = childrenFactory;
		}

		public IEnumerator<T> GetChildren(T node) => _childrenFactory(node).GetEnumerator();
	}

	internal static class TreeAdvisorExtensions
	{
		public static DelegateTreeEnumerableAdvisor<T> AsAdvisor<T>(this Func<T, IEnumerable<T>> childrenFactory)
		{
			return new(childrenFactory);
		}

		public static DelegateTreeEnumeratorAdvisor<T> AsAdvisor<T>(this Func<T, IEnumerator<T>> childrenFactory)
		{
			return new(childrenFactory);
		}
	}
}