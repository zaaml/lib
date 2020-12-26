// <copyright file="HierarchyView.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Zaaml.Core.Extensions;
using Zaaml.Core.Packed;
using Zaaml.Core.Trees;
using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Data.Hierarchy
{
	internal abstract class HierarchyView
	{
		public virtual int FlatCount { get; protected set; }

		public virtual int VisibleFlatCount { get; protected set; }

		public abstract int FindDataIndex(object data);

		public abstract object GetData(int index);

		public abstract IEnumerator<object> GetDataEnumerator();
	}

	internal abstract partial class HierarchyView<THierarchy, TNodeCollection, TNode> : HierarchyView, ITreeEnumeratorAdvisor<TNode>, IDisposable
		where THierarchy : HierarchyView<THierarchy, TNodeCollection, TNode>
		where TNodeCollection : HierarchyNodeViewCollection<THierarchy, TNodeCollection, TNode>
		where TNode : HierarchyNodeView<THierarchy, TNodeCollection, TNode>
	{
		private static readonly IEnumerator<TNode> EmptyEnumerator = Enumerable.Empty<TNode>().GetEnumerator();

		private int _flatCount;
		private TNodeCollection _nodes;

		private uint _packedValue;
		private int _visibleFlatCount;

		protected HierarchyView()
		{
			Cursor = new NodeCursor((THierarchy) this);
			DataPlainListView = new HierarchyDataPlainListView(this);
		}

		private NodeCursor Cursor { get; }

		public HierarchyDataPlainListView DataPlainListView { get; }

		protected abstract IHierarchyViewFilter<THierarchy, TNodeCollection, TNode> FilterCore { get; }

		protected virtual FilteringStrategy<THierarchy, TNodeCollection, TNode> FilteringStrategy => NodesFilteringStrategy<THierarchy, TNodeCollection, TNode>.Instance;

		internal FilteringStrategy<THierarchy, TNodeCollection, TNode> FilteringStrategyInternal => FilteringStrategy;

		internal IHierarchyViewFilter<THierarchy, TNodeCollection, TNode> FilterInternal => FilterCore;

		public override int FlatCount
		{
			get => _flatCount;
			protected set
			{
				_flatCount = value;

				ResetCursor();
			}
		}

		private bool IsFilteredCache
		{
			get => PackedDefinition.IsFilteredCache.GetValue(_packedValue);
			set => PackedDefinition.IsFilteredCache.SetValue(ref _packedValue, value);
		}

		internal bool IsFilteredInternal => FilterCore?.IsEnabled ?? false;

		public TNodeCollection Nodes => _nodes ??= CreateNodeCollection();

		internal long RefreshFilterVersion { get; private set; }

		internal virtual bool ShouldExpand => false;

		public IEnumerable Source
		{
			get => Nodes.Source;
			set
			{
				if (ReferenceEquals(Source, value))
					return;

				IndexedSource = value != null ? new IndexedEnumerable(value) : IndexedEnumerable.Empty;

				Nodes.Source = value;

				LoadNodes();
				RaiseReset();
			}
		}

		internal IndexedEnumerable IndexedSource { get; private set; } = IndexedEnumerable.Empty;

		private bool SuspendCollectionChange
		{
			get => PackedDefinition.SuspendCollectionChange.GetValue(_packedValue);
			set => PackedDefinition.SuspendCollectionChange.SetValue(ref _packedValue, value);
		}

		internal bool SuspendExpansion
		{
			get => PackedDefinition.SuspendExpansion.GetValue(_packedValue);
			private set => PackedDefinition.SuspendExpansion.SetValue(ref _packedValue, value);
		}

		public override int VisibleFlatCount
		{
			get => _visibleFlatCount;
			protected set
			{
				_visibleFlatCount = value;

				ResetCursor();
			}
		}

		public void CollapseAll(ExpandCollapseMode mode)
		{
			var structureChange = ExpandCollapseStructureChange(mode);

			CollapseAllCore(mode, structureChange);

			if (structureChange)
				RaiseReset();
		}

		private void CollapseAllCore(ExpandCollapseMode mode, bool structureChange)
		{
			try
			{
				SuspendCollectionChange = true;
				SuspendExpansion = true;

				if (structureChange)
				{
					FlatCount = 0;
					VisibleFlatCount = 0;
				}

				foreach (var node in Nodes)
				{
					node.CollapseRecursive(mode);

					if (structureChange)
					{
						FlatCount += node.FlatCount + 1;
						VisibleFlatCount += node.VisibleFlatCount + 1;
					}
				}
			}
			finally
			{
				SuspendCollectionChange = false;
				SuspendExpansion = false;
			}
		}

		protected TNode CreateChild(object treeNodeData)
		{
			return CreateChildCore(treeNodeData);
		}

		protected abstract TNode CreateChildCore(object data);

		private TNodeCollection CreateNodeCollection()
		{
			return CreateNodeCollectionCore();
		}

		protected abstract TNodeCollection CreateNodeCollectionCore();

		protected virtual void DisposeCore()
		{
		}

		public void ExpandAll(ExpandCollapseMode mode)
		{
			var structureChange = ExpandCollapseStructureChange(mode);

			ExpandAllCore(mode, structureChange);

			if (structureChange)
				RaiseReset();
		}

		private void ExpandAllCore(ExpandCollapseMode mode, bool structureChange)
		{
			try
			{
				SuspendCollectionChange = true;
				SuspendExpansion = true;

				if (structureChange)
				{
					FlatCount = 0;
					VisibleFlatCount = 0;
				}

				foreach (var node in Nodes)
				{
					node.ExpandRecursive(mode);

					if (structureChange)
					{
						FlatCount += node.FlatCount + 1;
						VisibleFlatCount += node.VisibleFlatCount + 1;
					}
				}
			}
			finally
			{
				SuspendCollectionChange = false;
				SuspendExpansion = false;
			}
		}

		internal bool ExpandCollapseStructureChange(ExpandCollapseMode mode)
		{
			if (mode == ExpandCollapseMode.Auto)
				return true;

			if (mode == ExpandCollapseMode.Default)
				return IsFilteredInternal == false;

			return IsFilteredInternal;
		}

		public void ExpandRoot(TNode node)
		{
			var current = node.Parent;

			while (current != null)
			{
				current.IsExpanded = true;
				current = current.Parent;
			}
		}

		public override int FindDataIndex(object data)
		{
			return Cursor.FindDataIndex(data);
		}

		public int FindIndex(TNode treeNode)
		{
			return Cursor.FindIndex(treeNode);
		}

		public int FindIndex(TNode treeNode, bool resetCursor)
		{
			if (resetCursor)
				ResetCursor();

			return Cursor.FindIndex(treeNode);
		}

		public TNode FindNode(object nodeData)
		{
			TNode result = default;

			var stopSearch = false;

			try
			{
				SuspendCollectionChange = true;

				FlatCount = 0;
				VisibleFlatCount = 0;

				foreach (var node in Nodes)
				{
					if (Equals(node.Data, nodeData))
					{
						result = node;

						stopSearch = true;
					}

					if (stopSearch == false)
						stopSearch = node.FindRecursive(nodeData, out result);

					FlatCount += node.FlatCount + 1;
					VisibleFlatCount += node.VisibleFlatCount + 1;
				}
			}
			finally
			{
				SuspendCollectionChange = false;
			}

			return result;
		}

		public override object GetData(int index)
		{
			return Cursor.NavigateTo(index).Data;
		}

		public override IEnumerator<object> GetDataEnumerator()
		{
			return TreeEnumerator.GetEnumerator(Nodes, this).Enumerate().Select(t => t.Data).GetEnumerator();
		}

		internal IEnumerable GetDataNodes(TNode node)
		{
			return GetDataNodesCore(node);
		}

		protected abstract IEnumerable GetDataNodesCore(TNode node);

		public TNode GetNode(int index)
		{
			return Cursor.NavigateTo(index);
		}

		private void HandleAdd(int newIndex, IList newItems)
		{
			if (newItems == null)
				return;

			SuspendCollectionChange = true;

			Nodes.HandleAdd(newIndex, newItems);

			var visibleFlatCount = VisibleFlatCount;
			var count = newItems.Count;
			var index = newIndex;

			for (var i = index; i < index + count; i++)
			{
				var node = Nodes[i];

				node.IsExpanded = IsDataExpandedCore(node);

				FlatCount += node.FlatCount + 1;
				VisibleFlatCount += node.VisibleFlatCount + 1;
			}

			SuspendCollectionChange = false;

			var raiseIndex = FindIndex(Nodes[index]);

			RaiseChange(raiseIndex, VisibleFlatCount - visibleFlatCount);
		}

		private void HandleMove(int oldIndex, int newIndex)
		{
			Nodes.HandleMove(oldIndex, newIndex);

			RaiseReset();
		}

		private void HandleRemove(int oldIndex, IList oldItems)
		{
			if (oldItems == null)
				return;

			SuspendCollectionChange = true;

			var visibleFlatCount = VisibleFlatCount;
			var count = oldItems.Count;
			var index = oldIndex;

			for (var i = index; i < index + count; i++)
			{
				var node = Nodes[i];

				FlatCount -= node.FlatCount + 1;
				VisibleFlatCount -= node.VisibleFlatCount + 1;
			}

			var raiseIndex = FindIndex(Nodes[index]);

			Nodes.HandleRemove(oldIndex, oldItems);

			SuspendCollectionChange = false;

			RaiseChange(raiseIndex, VisibleFlatCount - visibleFlatCount);
		}

		private void HandleReset()
		{
			Nodes.HandleReset();

			LoadNodes();
			RaiseReset();
		}

		internal bool IsDataExpanded(TNode node)
		{
			return ShouldExpand || IsDataExpandedCore(node);
		}

		protected abstract bool IsDataExpandedCore(TNode node);

		public void LoadAll()
		{
			LoadAllCore();
		}

		private void LoadAllCore()
		{
			try
			{
				SuspendCollectionChange = true;

				FlatCount = 0;
				VisibleFlatCount = 0;

				foreach (var node in Nodes)
				{
					node.LoadRecursive();

					FlatCount += node.FlatCount + 1;
					VisibleFlatCount += node.VisibleFlatCount + 1;
				}
			}
			finally
			{
				SuspendCollectionChange = false;
			}
		}

		private void LoadNodes()
		{
			SuspendCollectionChange = true;

			FlatCount = 0;
			VisibleFlatCount = 0;

			foreach (var node in Nodes)
			{
				node.IsExpanded = IsDataExpandedCore(node);

				FlatCount += node.FlatCount + 1;
				VisibleFlatCount += node.VisibleFlatCount + 1;
			}

			SuspendCollectionChange = false;
		}

		internal void OnDescendantFlatCountChanged(int count)
		{
			if (SuspendCollectionChange)
				return;

			FlatCount += count;
		}

		internal void OnDescendantVisibleFlatCountChanged(int flatIndex, int count)
		{
			if (SuspendCollectionChange)
				return;

			VisibleFlatCount += count;

			RaiseChange(flatIndex, count);
		}

		internal void OnSourceCollectionChangedInternal(NotifyCollectionChangedEventArgs eventArgs)
		{
			switch (eventArgs.Action)
			{
				case NotifyCollectionChangedAction.Add:

					HandleAdd(eventArgs.NewStartingIndex, eventArgs.NewItems);

					break;

				case NotifyCollectionChangedAction.Remove:

					HandleRemove(eventArgs.OldStartingIndex, eventArgs.OldItems);

					break;

				case NotifyCollectionChangedAction.Replace:

					HandleRemove(eventArgs.OldStartingIndex, eventArgs.OldItems);
					HandleAdd(eventArgs.NewStartingIndex, eventArgs.NewItems);

					break;

				case NotifyCollectionChangedAction.Move:

					HandleMove(eventArgs.OldStartingIndex, eventArgs.NewStartingIndex);

					break;

				case NotifyCollectionChangedAction.Reset:

					HandleReset();

					break;

				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private void RaiseChange(int index, int count)
		{
			Cursor.Reset();

			if (SuspendCollectionChange || count == 0)
				return;

			// TODO SelectionController exception
			//DataPlainListView.RaiseChange(index, count);
			DataPlainListView.RaiseReset();
		}

		internal void RaiseReset()
		{
			Cursor.Reset();

			if (SuspendCollectionChange)
				return;

			DataPlainListView.RaiseReset();
		}

		private void Rebuild()
		{
			FlatCount = 0;
			VisibleFlatCount = 0;

			foreach (var node in Nodes)
			{
				node.RebuildRecursive();

				FlatCount += node.FlatCount + 1;
				VisibleFlatCount += node.VisibleFlatCount + 1;
			}
		}

		protected void RefreshFilterCore()
		{
			RefreshFilterVersion++;

			var filteringStrategy = FilteringStrategy;
			var newIsFiltered = IsFilteredInternal;

			IsFilteredCache = newIsFiltered;
			Nodes.RefreshFilter(filteringStrategy);

			if (IsFilteredCache)
			{
				if (ShouldExpand)
					ExpandAllCore(ExpandCollapseMode.Auto, true);
				else
					LoadAllCore();
			}
			else
			{
				Rebuild();
			}

			ResetCursor();
			RaiseReset();
		}
		//private void DebugCount()
		//{
		//	var flatVis = VisibleFlatCount;

		//	Rebuild();

		//	var flatVis1 = VisibleFlatCount;

		//	ResetCursor();

		//	var flatVis2 = Cursor.CalculateCount();
		//	var flatVis3 = Cursor.CalculateBackCount();
		//}

		internal void ResetCursor()
		{
			Cursor.Reset();
		}

		internal void ResetNode(TNode node, int flatCountChange, int visibleFlatCountChange)
		{
			var flatIndex = FindIndex(node, true);

			FlatCount += flatCountChange;
			VisibleFlatCount += visibleFlatCountChange;

			RaiseChange(flatIndex + 1, visibleFlatCountChange);
		}

		public void Dispose()
		{
			DisposeCore();
		}

		IEnumerator<TNode> ITreeEnumeratorAdvisor<TNode>.GetChildren(TNode node)
		{
			return node.IsExpanded == false ? EmptyEnumerator : node.Nodes.GetEnumerator();
		}

		private static class PackedDefinition
		{
			public static readonly PackedBoolItemDefinition SuspendExpansion;
			public static readonly PackedBoolItemDefinition SuspendCollectionChange;
			public static readonly PackedBoolItemDefinition IsFilteredCache;

			static PackedDefinition()
			{
				var allocator = new PackedValueAllocator();

				SuspendExpansion = allocator.AllocateBoolItem();
				SuspendCollectionChange = allocator.AllocateBoolItem();
				IsFilteredCache = allocator.AllocateBoolItem();
			}
		}
	}
}