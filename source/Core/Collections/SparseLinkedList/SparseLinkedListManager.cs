// <copyright file="SparseLinkedListManager.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace Zaaml.Core.Collections
{
	internal class SparseLinkedListManager<T>
	{
		public SparseLinkedListManager(SparseMemoryManager<T> sparseMemoryManager)
		{
			SparseMemoryManager = sparseMemoryManager;
		}

		private Stack<SparseLinkedListBase<T>.GapNode> GapNodePool { get; } = new Stack<SparseLinkedListBase<T>.GapNode>();

		private Stack<SparseLinkedListBase<T>.RealizedNode> RealizedNodePool { get; } =
			new Stack<SparseLinkedListBase<T>.RealizedNode>();

		public SparseMemoryManager<T> SparseMemoryManager { get; }

		public SparseMemorySpan<T> Allocate()
		{
			return SparseMemoryManager.Allocate();
		}

		public SparseLinkedListBase<T>.GapNode GetGapNode()
		{
			return GapNodePool.Count > 0 ? GapNodePool.Pop() : new SparseLinkedListBase<T>.GapNode();
		}

		public SparseLinkedListBase<T>.RealizedNode GetRealizedNode()
		{
			var realizedNode = RealizedNodePool.Count > 0
				? RealizedNodePool.Pop()
				: new SparseLinkedListBase<T>.RealizedNode();

			realizedNode.Mount(SparseMemoryManager.Allocate());

			return realizedNode;
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