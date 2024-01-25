// <copyright file="ResourceCleanList.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Runtime.CompilerServices;

namespace Zaaml.Core.Collections
{
	internal class ResourceCleaner<TResourceCleaner, TResource>
		where TResourceCleaner : ResourceCleaner<TResourceCleaner, TResource>
		where TResource : ResourceCleaner<TResourceCleaner, TResource>.IResource
	{
		private readonly NodePool _nodePool;
		private Node _head;

		public ResourceCleaner(NodePool nodePool)
		{
			_nodePool = nodePool;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public TResource Add(TResource resource)
		{
			if (resource.CleanerNode != null)
				return resource;

			_head = _nodePool.Rent(resource, (TResourceCleaner)this, _head);

			return resource;
		}

		public void Clean()
		{
			var current = _head;

			while (current != null)
				current = current.Clean();

			_head = null;
		}

		internal interface IResource
		{
			Node CleanerNode { get; set; }

			void Clean();
		}

		internal sealed class NodePool

		{
			private Node _poolHead;

			public Node Rent(TResource resource, TResourceCleaner cleaner, Node cleanerHead)
			{
				if (_poolHead == null)
					return new Node(this).Mount(resource, cleaner, cleanerHead);

				var reference = _poolHead;

				_poolHead = reference.Next;

				return reference.Mount(resource, cleaner, cleanerHead);
			}

			public Node Release(Node node)
			{
				var head = _poolHead;

				_poolHead = node;

				return head;
			}
		}

		internal sealed class Node
		{
			private readonly NodePool _pool;
			private TResource _resource;

			public Node(NodePool pool)
			{
				_pool = pool;
			}

			public Node Next { get; private set; }

			public TResourceCleaner Cleaner { get; private set; }

			public Node Mount(TResource resource, TResourceCleaner cleaner, Node next)
			{
				Cleaner = cleaner;
				Next = next;

				_resource = resource;
				_resource.CleanerNode = this;

				return this;
			}

			internal Node Clean()
			{
				var next = Next;

				_resource.CleanerNode = null;
				_resource.Clean();
				_resource = default;

				Next = _pool.Release(this);

				return next;
			}
		}
	}
}