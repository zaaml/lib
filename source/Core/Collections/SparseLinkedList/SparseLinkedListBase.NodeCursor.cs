// <copyright file="SparseLinkedListBase.NodeCursor.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Diagnostics.CodeAnalysis;

namespace Zaaml.Core.Collections
{
	internal partial class SparseLinkedListBase<T>
	{
		[SuppressMessage("ReSharper", "InconsistentNaming")]
		internal readonly struct NodeCursor
		{
			private readonly NodeBase NodePrivate;
			private readonly long NodeOffsetPrivate;
			private readonly long IndexPrivate;
			public readonly SparseLinkedListBase<T> List;
			public readonly int StructureVersion;

			public static NodeCursor Empty => new NodeCursor(-1, null, null, -1);

			public bool IsEmpty => List == null;

			public bool IsValid => IsEmpty == false && StructureVersion == List.StructureVersion;

			public NodeBase Node
			{
				get
				{
					VerifyValid();

					return NodePrivate;
				}
			}

			public long NodeOffset
			{
				get
				{
					VerifyValid();

					return NodeOffsetPrivate;
				}
			}

			public long Index
			{
				get
				{
					VerifyValid();

					return IndexPrivate;
				}
			}

			public int LocalIndex
			{
				get
				{
					VerifyValid();

					return (int) (IndexPrivate - NodeOffsetPrivate);
				}
			}

			public NodeCursor GetNext()
			{
				VerifyValid();

				if (NodePrivate.Next == null)
					return Empty;

				return new NodeCursor(NodeOffsetPrivate + NodePrivate.Size, List, NodePrivate.Next,
					NodeOffsetPrivate + NodePrivate.Size);
			}

			public long NodeSize
			{
				get
				{
					VerifyValid();

					return NodePrivate.Size;
				}
			}

			public NodeCursor WithIndex(long index)
			{
				if (Contains(index, true) == false)
				{
					if (index < NodeOffsetPrivate)
						index = NodeOffsetPrivate;
					else
						index = NodeOffsetPrivate + NodeSize;
				}

				return new NodeCursor(index, List, NodePrivate, NodeOffsetPrivate);
			}

			public NodeCursor UpdateStructureVersion()
			{
				return new NodeCursor(IndexPrivate, List, NodePrivate, NodeOffsetPrivate);
			}

			private void VerifyValid()
			{
				if (IsValid == false)
					throw new InvalidOperationException();
			}

			public NodeCursor GetPrev()
			{
				VerifyValid();

				if (NodePrivate.Prev == null)
					return Empty;

				return new NodeCursor(NodeOffsetPrivate - 1, List, NodePrivate.Prev, NodeOffsetPrivate - NodePrivate.Prev.Size);
			}

			private NodeCursor(long index, SparseLinkedListBase<T> list)
			{
				IndexPrivate = index;
				List = list;
				NodePrivate = list.HeadNode;
				NodeOffsetPrivate = 0;
				StructureVersion = list.StructureVersion;
			}

			public NodeCursor(long index, SparseLinkedListBase<T> list, NodeBase node, long nodeOffset)
			{
				IndexPrivate = index;
				List = list;
				NodePrivate = node;
				NodeOffsetPrivate = nodeOffset;
				StructureVersion = list?.StructureVersion ?? -1;
			}

			public NodeCursor NavigateTo(long index)
			{
				VerifyValid();

				var node = NodePrivate;
				var nodeOffset = NodeOffsetPrivate;
				var cursorIndex = IndexPrivate;

				if (Contains(index))
					return new NodeCursor(index, List, node, NodeOffsetPrivate);

				if (index >= cursorIndex)
				{
					while ((index >= nodeOffset && index < nodeOffset + node.Size) == false)
					{
						nodeOffset += node.Size;
						node = node.Next;

						if (node == null)
							break;
					}
				}
				else
				{
					while ((index >= nodeOffset && index < nodeOffset + node.Size) == false)
					{
						node = node.Prev;

						if (node == null)
							break;

						nodeOffset -= node.Size;
					}
				}

				return node == null ? Empty : new NodeCursor(index, List, node, nodeOffset);
			}

			public bool Contains(long index)
			{
				if (IsEmpty)
					return false;

				VerifyValid();

				return index >= NodeOffsetPrivate && index < NodeOffsetPrivate + NodePrivate.Size;
			}

			public bool Contains(long index, bool includeEnd)
			{
				if (IsEmpty)
					return false;

				VerifyValid();

				if (includeEnd)
					return index >= NodeOffsetPrivate && index <= NodeOffsetPrivate + NodePrivate.Size;

				return index >= NodeOffsetPrivate && index < NodeOffsetPrivate + NodePrivate.Size;
			}

			public bool Equals(NodeCursor other)
			{
				if (IsEmpty && other.IsEmpty)
					return true;

				VerifyValid();
				other.VerifyValid();

				return Equals(NodePrivate, other.NodePrivate) && NodeOffsetPrivate == other.NodeOffsetPrivate &&
				       IndexPrivate == other.IndexPrivate &&
				       Equals(List, other.List) && StructureVersion == other.StructureVersion;
			}

			public override string ToString()
			{
				if (IsEmpty)
					return "Empty";

				return $"Index: {IndexPrivate}, NodeOffset: {NodeOffsetPrivate}, Node: {NodePrivate}";
			}
		}
	}
}