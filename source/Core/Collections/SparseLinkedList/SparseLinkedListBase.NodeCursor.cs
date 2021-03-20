// <copyright file="SparseLinkedListBase.NodeCursor.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Zaaml.Core.Collections
{
	internal partial class SparseLinkedListBase<T>
	{
		[SuppressMessage("ReSharper", "InconsistentNaming")]
		internal struct NodeCursor
		{
			public static readonly NodeCursor Empty = new NodeCursor(-1, null, null, -1, 0);
			public readonly SparseLinkedListBase<T> List;
			private long IndexPrivate;
			private long NodeOffsetPrivate;
			private NodeBase NodePrivate;
			public ulong StructureVersion;

			public NodeCursor(SparseLinkedListBase<T> list)
			{
				IndexPrivate = 0;
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
				StructureVersion = list.StructureVersion;
			}

			private NodeCursor(long index, SparseLinkedListBase<T> list, NodeBase node, long nodeOffset, ulong structureVersion)
			{
				IndexPrivate = index;
				List = list;
				NodePrivate = node;
				NodeOffsetPrivate = nodeOffset;
				StructureVersion = structureVersion;
			}

			public long Index
			{
				get
				{
					VerifyValid();

					return IndexPrivate;
				}
				set
				{
					VerifyValid();

					IndexPrivate = value;
				}
			}

			public bool IsEmpty => NodePrivate == null;

			public bool IsValid => IsEmpty == false && StructureVersion == List.StructureVersion;

			public int LocalIndex
			{
				get
				{
					VerifyValid();

					return (int) (IndexPrivate - NodeOffsetPrivate);
				}
			}

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

			public long NodeSize
			{
				get
				{
					VerifyValid();

					return NodePrivate.Size;
				}
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public bool Contains(long index)
			{
				VerifyValid();

				return index >= NodeOffsetPrivate && index < NodeOffsetPrivate + NodePrivate.Size;
			}

			public NodeCursor GetNext()
			{
				VerifyValid();

				if (NodePrivate.Next == null)
					return Empty;

				return new NodeCursor(NodeOffsetPrivate + NodePrivate.Size, List, NodePrivate.Next,
					NodeOffsetPrivate + NodePrivate.Size);
			}

			public NodeCursor GetPrev()
			{
				VerifyValid();

				if (NodePrivate.Prev == null)
					return Empty;

				return new NodeCursor(NodeOffsetPrivate - 1, List, NodePrivate.Prev, NodeOffsetPrivate - NodePrivate.Prev.Size);
			}

			public void MoveNext()
			{
				VerifyValid();

				if (NodePrivate.Next == null)
				{
					this = Empty;

					return;
				}

				IndexPrivate = NodeOffsetPrivate + NodePrivate.Size;
				NodeOffsetPrivate += NodePrivate.Size;
				NodePrivate = NodePrivate.Next;
			}

			public void MovePrev()
			{
				VerifyValid();

				if (NodePrivate.Prev == null)
				{
					this = Empty;

					return;
				}

				IndexPrivate = NodeOffsetPrivate - 1;
				NodePrivate = NodePrivate.Prev;
				NodeOffsetPrivate -= NodePrivate.Size;
			}

			public void NavigateTo(long index)
			{
				VerifyValid();

				var node = NodePrivate;
				var nodeOffset = NodeOffsetPrivate;
				var cursorIndex = IndexPrivate;

				if (Contains(index))
				{
					IndexPrivate = index;

					return;
				}

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

				if (node == null)
				{
					this = Empty;
				}
				else
				{
					IndexPrivate = index;
					NodePrivate = node;
					NodeOffsetPrivate = nodeOffset;
				}
			}

			public void NavigateToInsert(long index)
			{
				VerifyValid();

				var node = NodePrivate;
				var nodeOffset = NodeOffsetPrivate;
				var cursorIndex = IndexPrivate;

				if (Contains(index))
				{
					IndexPrivate = index;

					return;
				}

				if (index >= cursorIndex)
				{
					while (node != null)
					{
						var next = node.Next;
						var nextNodeOffset = nodeOffset + node.Size;

						if (index >= nodeOffset && index <= nodeOffset + node.Size)
						{
							if (next != null && index >= nextNodeOffset && index < nextNodeOffset + next.Size)
							{
								node = next;
								nodeOffset = nextNodeOffset;
							}

							break;
						}

						node = next;
						nodeOffset = nextNodeOffset;
					}
				}
				else
				{
					while (node != null)
					{
						var prev = node.Prev;
						var prevNodeOffset = nodeOffset - prev?.Size ?? 0;

						if (index >= nodeOffset && index <= nodeOffset + node.Size)
						{
							if (prev != null && index >= prevNodeOffset && index < prevNodeOffset + prev.Size)
							{
								node = prev;
								nodeOffset = prevNodeOffset;
							}

							break;
						}

						node = prev;
						nodeOffset = prevNodeOffset;
					}
				}

				if (node == null)
					this = Empty;
				else
				{
					IndexPrivate = index;
					NodePrivate = node;
					NodeOffsetPrivate = nodeOffset;
				}
			}

			public override string ToString()
			{
				if (IsEmpty)
					return "Empty";

				return $"Index: {IndexPrivate}, NodeOffset: {NodeOffsetPrivate}, Node: {NodePrivate}";
			}

			public void UpdateStructureVersion()
			{
				StructureVersion = List.ActualStructureVersion;
			}

			public void UpgradeWithNode(NodeBase node)
			{
				NodePrivate = node;
				StructureVersion = List.ActualStructureVersion;
			}

			[Conditional("COLLECTION_VERIFY")]
			private void VerifyValid()
			{
				if (IsValid == false)
					throw new InvalidOperationException();
			}
		}
	}
}