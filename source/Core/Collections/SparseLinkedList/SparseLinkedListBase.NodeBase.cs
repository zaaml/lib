// <copyright file="SparseLinkedListBase - Copy (2).Realize.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Diagnostics;

namespace Zaaml.Core.Collections
{
	internal partial class SparseLinkedListBase<T>
	{
		[DebuggerDisplay(("{Dump()}"))]
		internal abstract class NodeBase
		{
//#if TEST
//			~NodeBase()
//			{
//				if (Size != -1)
//					throw new InvalidOperationException();
//			}
//#endif

			public long Size { get; set; } = -1;

			public NodeBase Next { get; set; }

			public NodeBase Prev { get; set; }

			internal long GetGlobalIndex()
			{
				var current = Prev;
				var globalIndex = 0L;

				while (current != null)
				{
					globalIndex += current.Size;
					current = current.Prev;
				}

				return globalIndex;
			}

			protected bool ContainsLocal(long index)
			{
				return index >= 0 && index < Size;
			}

			internal abstract T GetLocalItem(int index);

			internal abstract T GetItem(ref NodeCursor cursor);

			internal abstract void SetItem(ref NodeCursor cursor, T item);

			public virtual void Release()
			{
				Next = null;
				Prev = null;
				Size = -1;
			}

			public string Dump()
			{
				var index = GetGlobalIndex();
				var range = Size == 0 ? $"[{index}]" : $"[{index}..{index + Size - 1}]";

				return this is GapNode ? $"gap{range}" : $"real{range}";
			}

			public override string ToString()
			{
				return Dump();
			}
		}
	}
}