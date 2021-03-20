﻿// <copyright file="SparseLinkedListManager.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Diagnostics;

namespace Zaaml.Core.Collections
{
	internal class SparseLinkedListManager<T>
	{
		public SparseLinkedListManager(SparseMemoryAllocator<T> sparseMemoryAllocator)
		{
			SparseMemoryAllocator = sparseMemoryAllocator;
		}

		private Stack<SparseLinkedListBase<T>.GapNode> GapNodePool { get; } = new Stack<SparseLinkedListBase<T>.GapNode>();

		private Stack<SparseLinkedListBase<T>.RealizedNode> RealizedNodePool { get; } = new Stack<SparseLinkedListBase<T>.RealizedNode>();

		public SparseMemoryAllocator<T> SparseMemoryAllocator { get; }

		public Memory<T> Allocate()
		{
			return SparseMemoryAllocator.Allocate();
		}

		public SparseLinkedListBase<T>.GapNode GetGapNode()
		{
			var gapNode = GapNodePool.Count > 0 ? GapNodePool.Pop() : new SparseLinkedListBase<T>.GapNode();

			MountNode(gapNode);

			return gapNode;
		}

		public SparseLinkedListBase<T>.RealizedNode GetRealizedNode()
		{
			var realizedNode = RealizedNodePool.Count > 0
				? RealizedNodePool.Pop()
				: new SparseLinkedListBase<T>.RealizedNode();

			MountNode(realizedNode);
			realizedNode.Mount(SparseMemoryAllocator.Allocate());

			return realizedNode;
		}

		private static void MountNode(SparseLinkedListBase<T>.NodeBase node)
		{
			Debug.Assert(node.Size == -1);

			node.Next = null;
			node.Prev = null;
			node.Size = 0;
		}

		protected virtual void OnNodeReleased(SparseLinkedListBase<T>.NodeBase node)
		{
		}

		protected virtual void OnNodeReleasing(SparseLinkedListBase<T>.NodeBase node)
		{
		}

		public void ReleaseNode(SparseLinkedListBase<T>.NodeBase node)
		{
			OnNodeReleasing(node);

			node.Release();

			if (node is SparseLinkedListBase<T>.GapNode gapNode)
				GapNodePool.Push(gapNode);
			else if (node is SparseLinkedListBase<T>.RealizedNode realizedNode)
				RealizedNodePool.Push(realizedNode);

			OnNodeReleased(node);
		}

		public void ReleaseRealizedNode(SparseLinkedListBase<T>.RealizedNode realizedNode)
		{
			RealizedNodePool.Push(realizedNode);
		}
	}
}