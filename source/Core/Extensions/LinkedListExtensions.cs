// <copyright file="LinkedListExtensions.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using Zaaml.Core.Utils;

namespace Zaaml.Core.Extensions
{
	internal static class LinkedListExtensions
	{
		public static void AddFirst<TNode>(this ILinkedList<TNode> linkedList, TNode node)
			where TNode : class, ILinkedListNode<TNode>
		{
			LinkedListUtils.AddFirst(linkedList, node);
		}

		public static void AddLast<TNode>(this ILinkedList<TNode> linkedList, TNode node)
			where TNode : class, ILinkedListNode<TNode>
		{
			LinkedListUtils.AddLast(linkedList, node);
		}

		public static void Clear<TNode>(this ILinkedList<TNode> linkedList)
			where TNode : class, ILinkedListNode<TNode>
		{
			LinkedListUtils.Clear(linkedList);
		}

		public static void Clear<TNode>(this ILinkedList<TNode> linkedList, Action<TNode> clearAction)
			where TNode : class, ILinkedListNode<TNode>
		{
			LinkedListUtils.Clear(linkedList, clearAction);
		}

		public static TNode Find<TNode>(this ILinkedList<TNode> linkedList, Func<TNode, bool> predicate)
			where TNode : class, ILinkedListNode<TNode>
		{
			return LinkedListUtils.Find(linkedList, predicate);
		}

		public static LinkedListEnumerator<TNode> GetEnumerator<TNode>(this ILinkedList<TNode> linkedList)
			where TNode : class, ILinkedListNode<TNode>
		{
			return new(linkedList);
		}

		public static void Remove<TNode>(this ILinkedList<TNode> linkedList, TNode node)
			where TNode : class, ILinkedListNode<TNode>
		{
			LinkedListUtils.Remove(linkedList, node);
		}
	}
}