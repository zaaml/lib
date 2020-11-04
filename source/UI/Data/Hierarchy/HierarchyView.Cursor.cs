// <copyright file="HierarchyView.Cursor.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Diagnostics;
using Zaaml.Core.Utils;

namespace Zaaml.UI.Data.Hierarchy
{
	internal interface IHierarchyViewFilter<THierarchy, TNodeCollection, TNode> 
		where THierarchy : HierarchyView<THierarchy, TNodeCollection, TNode>
		where TNodeCollection : HierarchyNodeViewCollection<THierarchy, TNodeCollection, TNode>
		where TNode : HierarchyNodeView<THierarchy, TNodeCollection, TNode>
	{
		bool Pass(TNode node);

		bool IsEnabled { get; }
	}

	internal abstract partial class HierarchyView<THierarchy, TNodeCollection, TNode>
	{
		#region  Nested Types

		private class NodeCursor
		{
			#region Fields

			private Data[] _data = new Data[4];
			private int _depth;
			private bool _dirty = true;

			#endregion

			#region Ctors

			public NodeCursor(THierarchy hierarchy)
			{
				Hierarchy = hierarchy;
			}

			#endregion

			#region Properties

			private TNode CurrentNode => _data[_depth].Node;

			private int FlatIndex { get; set; }

			private THierarchy Hierarchy { get; }

			#endregion

			#region  Methods

			private void EnsureDataSize(int size)
			{
				ArrayUtils.EnsureArrayLength(ref _data, size, true);
			}

			public int FindDataIndex(object data)
			{
				Reset();
				Ensure();

				var index = 0;

				while (ReferenceEquals(data, CurrentNode?.Data) == false)
				{
					if (MoveNext() == false)
						return -1;

					index++;
				}

				return ReferenceEquals(data, CurrentNode?.Data) ? index : -1;
			}

			public int FindIndex(TNode treeNode)
			{
				Reset();
				Ensure();

				var index = 0;

				while (ReferenceEquals(treeNode, CurrentNode) == false)
				{
					if (MoveNext() == false)
						return -1;

					index++;
				}

				return ReferenceEquals(treeNode, CurrentNode) ? index : -1;
			}

			private bool MoveNext()
			{
				Ensure();

				var data = _data[_depth];

				if (data.Node == null)
					return false;

				if (data.Node.IsExpanded && data.Node.Nodes.Count > 0)
				{
					var next = data.Node.Nodes[0];

					_depth++;

					EnsureDataSize(_depth + 1);

					_data[_depth] = new Data
					{
						Index = 0,
						Node = next
					};

					FlatIndex++;

					return true;
				}

				while (true)
				{
					var parentNodes = _depth == 0 ? Hierarchy.Nodes : _data[_depth - 1].Node.Nodes;

					if (data.Index + 1 < parentNodes.Count)
					{
						data.Index++;
						data.Node = parentNodes[data.Index];

						_data[_depth] = data;

						FlatIndex++;

						return true;
					}

					if (_depth == 0)
						return false;

					var hasNext = false;

					for (var depth = _depth; depth >= 0; depth--)
					{
						if (_data[depth].Index + 1 >= (depth == 0 ? Hierarchy.Nodes : _data[depth - 1].Node.Nodes).Count)
							continue;

						hasNext = true;

						break;
					}

					if (hasNext == false)
						return false;

					_data[_depth] = new Data();
					_depth--;
					
					data = _data[_depth];
				}
			}

			private bool MovePrev()
			{
				Ensure();

				var data = _data[_depth];

				if (data.Node == null)
					return false;

				var parentNodes = _depth == 0 ? Hierarchy.Nodes : _data[_depth - 1].Node.Nodes;

				if (data.Index - 1 >= 0)
				{
					data.Index--;
					data.Node = parentNodes[data.Index];

					_data[_depth] = data;

					while (data.Node.IsExpanded && data.Node.Nodes.Count > 0)
					{
						var index = data.Node.Nodes.Count - 1;
						var next = data.Node.Nodes[index];

						_depth++;

						EnsureDataSize(_depth + 1);

						data = new Data
						{
							Index = index,
							Node = next
						};

						_data[_depth] = data;
					}

					FlatIndex--;

					return true;
				}

				if (_depth == 0)
					return false;

				_data[_depth] = new Data();
				_depth--;

				FlatIndex--;

				return true;
			}

			public TNode NavigateTo(int flatIndex)
			{
				Ensure();

				if (Hierarchy.VisibleFlatCount == 0)
					return null;

				if (flatIndex < 0 || flatIndex >= Hierarchy.VisibleFlatCount)
					return null;

				if (flatIndex == FlatIndex)
					return CurrentNode;

				if (flatIndex > FlatIndex)
				{
					while (flatIndex > FlatIndex)
					{
						if (MoveNext() == false)
							return null;
					}
				}
				else
				{
					while (flatIndex < FlatIndex)
					{
						if (MovePrev() == false)
							return null;
					}
				}

				Debug.Assert(FlatIndex == flatIndex);

				return CurrentNode;
			}

			private void Ensure()
			{
				if (_dirty == false)
					return;

				_dirty = false;

				FlatIndex = 0;

				_depth = 0;

				for (var i = 0; i < _data.Length; i++)
					_data[i] = new Data();

				if (Hierarchy.Nodes.Count > 0)
				{
					_data[0] = new Data
					{
						Index = 0,
						Node = Hierarchy.Nodes[0]
					};
				}
			}

			public void Reset()
			{
				_dirty = true;
			}

			#endregion

			#region  Nested Types

			private struct Data
			{
				public TNode Node { get; set; }

				public int Index { get; set; }

				public override string ToString()
				{
					return Node?.ToString() ?? "Empty";
				}
			}

			#endregion

			//public int CalculateCount()
			//{
			//	Ensure();

			//	if (CurrentNode == null)
			//		return 0;

			//	var count = 1;

			//	while (MoveNext()) 
			//		count++;

			//	return count;
			//}

			//public int CalculateBackCount()
			//{
			//	Ensure();

			//	if (CurrentNode == null)
			//		return 0;

			//	var count = 1;

			//	while (MovePrev()) 
			//		count++;

			//	return count;
			//}
		}

		#endregion
	}
}