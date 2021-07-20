// <copyright file="LinkedListExtensions.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using Zaaml.Core.Utils;

namespace Zaaml.Core.Extensions
{
	internal static class LinkedListExtensions
	{
		public static void Append<TNode>(this ILinkedList<TNode> linkedList, TNode node)
			where TNode : class, ILinkedListNode<TNode>
		{
			LinkedListUtils.Append(linkedList, node);
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

		public static int Count<TNode>(this ILinkedList<TNode> linkedList)
			where TNode : class, ILinkedListNode<TNode>
		{
			return LinkedListUtils.Count(linkedList);
		}

		public static TNode Find<TNode>(this ILinkedList<TNode> linkedList, Func<TNode, bool> predicate)
			where TNode : class, ILinkedListNode<TNode>
		{
			return LinkedListUtils.Find(linkedList, predicate);
		}

		public static LinkedListEnumerator<TNode> GetLinkedListEnumerator<TNode>(this ILinkedList<TNode> linkedList, Direction direction)
			where TNode : class, ILinkedListNode<TNode>
		{
			return LinkedListUtils.GetLinkedListEnumerator(linkedList, direction);
		}

		public static void Prepend<TNode>(this ILinkedList<TNode> linkedList, TNode node)
			where TNode : class, ILinkedListNode<TNode>
		{
			LinkedListUtils.Prepend(linkedList, node);
		}

		public static void Remove<TNode>(this ILinkedList<TNode> linkedList, TNode node)
			where TNode : class, ILinkedListNode<TNode>
		{
			LinkedListUtils.Remove(linkedList, node);
		}
	}
}