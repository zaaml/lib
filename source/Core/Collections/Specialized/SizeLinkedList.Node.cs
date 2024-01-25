// <copyright file="SizeLinkedList.Node.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.Core.Collections.Specialized
{
	internal partial class SizeLinkedList
	{
		private static bool EqualSize(double size1, double size2)
		{
			return size1.Equals(size2);
		}

		private readonly struct NodeCursor
		{
			public static NodeCursor Empty => new NodeCursor(-1, 0, null, 0, 0, null);

			public NodeCursor(SizeLinkedList list)
			{
				IndexPrivate = 0;
				OffsetPrivate = 0;
				NodePrivate = list.Head;
				NodeIndexOffsetPrivate = 0;
				NodeSizeOffsetPrivate = 0;
				List = list;
				StructureVersion = list.StructureVersion;
			}

			public NodeCursor(long index, double offset, Node node, long nodeIndexOffset, double nodeSizeOffset, SizeLinkedList list)
			{
				IndexPrivate = index;
				OffsetPrivate = offset;
				NodePrivate = node;
				NodeIndexOffsetPrivate = nodeIndexOffset;
				NodeSizeOffsetPrivate = nodeSizeOffset;
				List = list;
				StructureVersion = list?.StructureVersion ?? -1;

				if (IsEmpty)
					return;

				if (index < nodeIndexOffset)
					throw new InvalidOperationException();
			}

			public bool IsEmpty => List == null;

			public bool IsValid => IsEmpty == false && StructureVersion == List.StructureVersion;

			public long StructureVersion { get; }

			public long Index
			{
				get
				{
					Verify();

					return IndexPrivate;
				}
			}

			public double Offset
			{
				get
				{
					Verify();

					return OffsetPrivate;
				}
			}

			public Node Node
			{
				get
				{
					Verify();

					return NodePrivate;
				}
			}

			public SizeLinkedList List { get; }

			public long NodeIndexOffset
			{
				get
				{
					Verify();

					return NodeIndexOffsetPrivate;
				}
			}

			public long LocalIndex
			{
				get
				{
					Verify();

					return IndexPrivate - NodeIndexOffsetPrivate;
				}
			}

			public double NodeSizeOffset
			{
				get
				{
					Verify();

					return NodeSizeOffsetPrivate;
				}
			}

			private long IndexPrivate { get; }

			private double OffsetPrivate { get; }

			private Node NodePrivate { get; }

			private long NodeIndexOffsetPrivate { get; }

			private double NodeSizeOffsetPrivate { get; }

			public bool ContainsIndex(long index)
			{
				Verify();

				return ContainsIndexPrivate(index);
			}

			private bool ContainsIndexPrivate(long index)
			{
				return index >= NodeIndexOffsetPrivate && index < NodeIndexOffsetPrivate + NodePrivate.Count;
			}

			public bool ContainsOffset(double offset)
			{
				Verify();

				return ContainsOffsetPrivate(offset);
			}

			private bool ContainsOffsetPrivate(double offset)
			{
				return offset >= NodeSizeOffsetPrivate && offset < NodeSizeOffsetPrivate + NodePrivate.GlobalSize;
			}

			private void Verify()
			{
				if (StructureVersion != List.StructureVersion)
					throw new InvalidOperationException();
			}

			public NodeCursor Prev
			{
				get
				{
					Verify();

					return PrevPrivate;
				}
			}

			public NodeCursor Next
			{
				get
				{
					Verify();

					return NextPrivate;
				}
			}

			private NodeCursor PrevPrivate
			{
				get
				{
					var prevNode = NodePrivate.Prev;

					if (prevNode == null)
						return Empty;

					return new NodeCursor(NodeIndexOffsetPrivate - Math.Min(1, prevNode.Count), NodeSizeOffsetPrivate - prevNode.Size, prevNode, NodeIndexOffsetPrivate - prevNode.Count, NodeSizeOffsetPrivate - prevNode.GlobalSize, List);
				}
			}

			private NodeCursor NextPrivate
			{
				get
				{
					var nextNode = NodePrivate.Next;

					if (nextNode == null)
						return Empty;

					return new NodeCursor(NodeIndexOffsetPrivate + NodePrivate.Count, NodeSizeOffsetPrivate + NodePrivate.GlobalSize, nextNode, NodeIndexOffsetPrivate + NodePrivate.Count, NodeSizeOffsetPrivate + NodePrivate.GlobalSize, List);
				}
			}

			public NodeCursor NavigateToIndex(long index)
			{
				Verify();

				if (ContainsIndexPrivate(index))
					return WithIndexPrivate(index);

				var cursor = this;

				if (index >= IndexPrivate)
				{
					cursor = cursor.NextPrivate;

					while (cursor.IsValid && cursor.ContainsIndexPrivate(index) == false)
						cursor = cursor.NextPrivate;
				}
				else
				{
					cursor = cursor.PrevPrivate;

					while (cursor.IsValid && cursor.ContainsIndexPrivate(index) == false)
						cursor = cursor.PrevPrivate;
				}

				if (cursor.IsValid == false)
					throw new InvalidOperationException();

				return cursor.WithIndexPrivate(index);
			}

			private NodeCursor WithIndexPrivate(long index)
			{
				return IsEmpty ? this : new NodeCursor(index, (index - NodeIndexOffsetPrivate) * Node.Size, NodePrivate, NodeIndexOffsetPrivate, NodeSizeOffsetPrivate, List);
			}

			public NodeCursor NavigateToOffset(double offset)
			{
				Verify();

				if (ContainsOffsetPrivate(offset))
					return NavigateToOffset(this, offset);

				var cursor = this;

				if (offset > NodeSizeOffsetPrivate)
				{
					cursor = cursor.NextPrivate;

					while (cursor.IsValid && cursor.ContainsOffsetPrivate(offset) == false)
						cursor = cursor.NextPrivate;
				}
				else
				{
					cursor = cursor.PrevPrivate;

					while (cursor.IsValid && cursor.ContainsOffsetPrivate(offset) == false)
						cursor = cursor.PrevPrivate;
				}

				return NavigateToOffset(cursor, offset);
			}

			private static NodeCursor NavigateToOffset(NodeCursor cursor, double offset)
			{
				if (cursor.IsEmpty)
					return cursor;

				var localOffset = offset - cursor.NodeSizeOffsetPrivate;
				var localIndex = (long) (localOffset / cursor.NodePrivate.Size);
				var index = cursor.NodeIndexOffsetPrivate + localIndex;

				return new NodeCursor(index, cursor.NodeSizeOffsetPrivate + localIndex * cursor.NodePrivate.Size, cursor.NodePrivate, cursor.NodeIndexOffsetPrivate, cursor.NodeSizeOffsetPrivate, cursor.List);
			}

			public NodeCursor UpdateStructureVersion()
			{
				return IsEmpty ? this : new NodeCursor(IndexPrivate, OffsetPrivate, NodePrivate, NodeIndexOffsetPrivate, NodeSizeOffsetPrivate, List);
			}
		}

		private sealed class Node
		{
			public long Count { get; set; }

			public double GlobalSize => Size * Count;

			public Node Next { get; set; }

			public Node Prev { get; set; }

			public double Size { get; set; }

			public bool EqualSize(double size)
			{
				return SizeLinkedList.EqualSize(Size, size);
			}
		}
	}
}