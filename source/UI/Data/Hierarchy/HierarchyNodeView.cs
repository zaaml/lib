// <copyright file="HierarchyNodeView.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections;
using System.Collections.Specialized;
using Zaaml.Core.Packed;

// ReSharper disable StaticMemberInGenericType

namespace Zaaml.UI.Data.Hierarchy
{
	internal abstract class HierarchyNodeView<THierarchy, TNodeCollection, TNode> : IDisposable
		where THierarchy : HierarchyView<THierarchy, TNodeCollection, TNode>
		where TNodeCollection : HierarchyNodeViewCollection<THierarchy, TNodeCollection, TNode>
		where TNode : HierarchyNodeView<THierarchy, TNodeCollection, TNode>
	{
		private uint _packedValue;

		protected HierarchyNodeView(THierarchy hierarchy, TNode parent)
		{
			Hierarchy = hierarchy;
			Parent = parent;
			Level = parent?.Level + 1 ?? 0;
		}

		public int ActualLevel
		{
			get
			{
				if (Hierarchy == null)
					return 0;

				if (Hierarchy.IsFilteredInternal == false || Hierarchy.FilteringStrategyInternal.CanHideParent == false)
					return Level;

				var effectiveLevel = 0;
				var current = Parent;

				while (current != null)
				{
					if (current.VisibleField)
						effectiveLevel++;

					current = current.Parent;
				}

				return effectiveLevel;
			}
		}

		public TNode ActualParent
		{
			get
			{
				if (Hierarchy == null)
					return null;

				if (Hierarchy.IsFilteredInternal == false || Hierarchy.FilteringStrategyInternal.CanHideParent == false)
					return Parent;

				var current = Parent;

				while (current != null && current.VisibleField == false)
					current = current.Parent;

				return current;
			}
		}

		public object Data { get; set; }

		public int FlatCount { get; private set; }

		public THierarchy Hierarchy { get; private set; }

		private bool IsActualExpandedField
		{
			get => PackedDefinition.IsActualExpandedField.GetValue(_packedValue);
			set
			{
				if (IsActualExpandedField == value)
					return;

				PackedDefinition.IsActualExpandedField.SetValue(ref _packedValue, value);

				OnIsExpandedChanged();
			}
		}

		public bool IsExpanded
		{
			get
			{
				if (Hierarchy == null)
					return false;

				return Hierarchy.IsFilteredInternal ? IsFilterExpandedField : IsExpandedField;
			}
			set
			{
				if (Hierarchy == null)
					return;

				if (IsExpanded == value)
					return;

				if (value)
					Expand();
				else
					Collapse();

				UpdateActualIsExpanded();
			}
		}

		private bool IsExpandedField
		{
			get => PackedDefinition.IsExpanded.GetValue(_packedValue);
			set => PackedDefinition.IsExpanded.SetValue(ref _packedValue, value);
		}

		private bool IsFilterExpandedField
		{
			get => PackedDefinition.IsFilterExpandedField.GetValue(_packedValue);
			set => PackedDefinition.IsFilterExpandedField.SetValue(ref _packedValue, value);
		}

		public bool IsLoaded => Nodes != null;

		private int Level { get; }

		public TNodeCollection Nodes { get; private set; }

		public TNode Parent { get; private set; }

		internal bool PassedFilterField
		{
			get => PackedDefinition.PassedFilterField.GetValue(_packedValue);
			set => PackedDefinition.PassedFilterField.SetValue(ref _packedValue, value);
		}

		internal bool VisibleField
		{
			get => PackedDefinition.VisibleField.GetValue(_packedValue);
			set => PackedDefinition.VisibleField.SetValue(ref _packedValue, value);
		}

		public int VisibleFlatCount { get; private set; }

		private int VisibleFlatCountCache { get; set; }

		private void Collapse()
		{
			var visibleFlatCount = -VisibleFlatCount;

			SetIsExpandedField(false);

			VisibleFlatCount = 0;

			HandleCount(0, visibleFlatCount);
		}

		public void CollapseRecursive(ExpandCollapseMode mode)
		{
			if (Hierarchy == null)
				return;

			CollapseRecursiveImpl(mode, Hierarchy.ExpandCollapseStructureChange(mode));
		}

		private void CollapseRecursiveImpl(ExpandCollapseMode mode, bool structureChange)
		{
			SetIsExpandedField(false, mode);

			if (structureChange)
			{
				VisibleFlatCount = 0;
				VisibleFlatCountCache = 0;
			}

			if (Nodes == null)
				return;

			foreach (var node in Nodes)
				node.CollapseRecursiveImpl(mode, structureChange);

			if (structureChange)
				VisibleFlatCountCache = Nodes.Count;
		}

		protected TNode CreateChildNode(object nodeData)
		{
			return CreateChildNodeCore(nodeData);
		}

		protected abstract TNode CreateChildNodeCore(object nodeData);

		protected abstract TNodeCollection CreateNodeCollectionCore();

		private bool CreateNodes()
		{
			if (Nodes != null)
				return false;

			Nodes = CreateNodeCollectionCore();
			Nodes.Source = Hierarchy.GetDataNodes((TNode) this);

			return true;
		}

		private void Expand(bool notify = true)
		{
			SetIsExpandedField(true);

			if (Nodes == null)
			{
				var flatCount = FlatCount;
				var visibleFlatCount = VisibleFlatCount;

				LoadNodes();

				if (notify)
					HandleCount(FlatCount - flatCount, VisibleFlatCount - visibleFlatCount);
			}
			else
			{
				VisibleFlatCount = VisibleFlatCountCache;

				if (notify)
					HandleCount(0, VisibleFlatCount);
			}
		}

		public void ExpandRecursive(ExpandCollapseMode mode)
		{
			if (Hierarchy == null)
				return;

			ExpandRecursiveImpl(mode, Hierarchy.ExpandCollapseStructureChange(mode));
		}

		private int ExpandRecursiveImpl(ExpandCollapseMode mode, bool structureChange)
		{
			CreateNodes();

			var flatCount = 0;

			foreach (var node in Nodes)
				flatCount += node.ExpandRecursiveImpl(mode, structureChange) + 1;

			SetIsExpandedField(true, mode);

			if (structureChange)
			{
				FlatCount = flatCount;
				VisibleFlatCount = flatCount;
				VisibleFlatCountCache = flatCount;
			}

			return flatCount;
		}

		internal bool FindRecursive(object nodeData, out TNode result)
		{
			if (Hierarchy == null)
			{
				result = null;

				return false;
			}

			var isLoaded = CreateNodes();

			FlatCount = 0;
			VisibleFlatCount = 0;
			VisibleFlatCountCache = 0;

			result = default;

			var found = false;

			foreach (var node in Nodes)
			{
				if (isLoaded)
					node.SetIsExpandedField(Hierarchy.IsDataExpanded(node));

				if (Equals(node.Data, nodeData))
				{
					result = node;

					found = true;
				}

				if (found == false)
					found = node.FindRecursive(nodeData, out result);

				FlatCount += node.FlatCount + 1;

				if (node.IsExpanded)
					VisibleFlatCountCache += node.VisibleFlatCount + 1;
				else
					VisibleFlatCountCache += 1;
			}

			if (IsExpanded)
				VisibleFlatCount = VisibleFlatCountCache;

			return found;
		}

		private void HandleAdd(int newIndex, IList newItems)
		{
			if (newItems == null)
				return;

			Nodes.HandleAdd(newIndex, newItems);

			var flatCount = FlatCount;
			var visibleFlatCount = VisibleFlatCount;

			LoadNodesRange(newIndex, newItems.Count);

			var flatIndex = Hierarchy.FindIndex(Nodes[newIndex], true);

			HandleCount(flatIndex, FlatCount - flatCount, VisibleFlatCount - visibleFlatCount);
		}

		private void HandleCount(int flatCount, int visibleCount)
		{
			HandleCount(Hierarchy.FindIndex((TNode) this) + 1, flatCount, visibleCount);
		}

		private bool HandleAncestorsCount(int flatCount, int visibleCount)
		{
			var isExpanded = true;
			var current = ActualParent;

			while (current != null)
			{
				if (isExpanded)
					current.VisibleFlatCountCache += visibleCount;

				isExpanded &= current.IsExpanded;

				if (isExpanded)
					current.VisibleFlatCount += visibleCount;

				current.FlatCount += flatCount;

				current = current.ActualParent;
			}

			return isExpanded;
		}

		private void HandleCount(int flatIndex, int flatCount, int visibleCount)
		{
			var isExpanded = HandleAncestorsCount(flatCount, visibleCount);

			if (isExpanded && visibleCount != 0)
				Hierarchy.OnDescendantVisibleFlatCountChanged(flatIndex, visibleCount);

			if (flatCount != 0)
				Hierarchy.OnDescendantFlatCountChanged(flatCount);

			Hierarchy.ResetCursor();
		}

		private void HandleMove(int oldIndex, int newIndex)
		{
			Nodes.HandleMove(oldIndex, newIndex);

			Hierarchy.RaiseReset();
		}

		private void HandleRemove(int oldIndex, IList oldItems)
		{
			if (oldItems == null)
				return;

			var flatCount = FlatCount;
			var visibleFlatCount = VisibleFlatCount;
			var flatIndex = Hierarchy.FindIndex(Nodes[oldIndex], false);

			UnloadNodesRange(oldIndex, oldItems.Count);

			Nodes.HandleRemove(oldIndex, oldItems);

			HandleCount(flatIndex, FlatCount - flatCount, VisibleFlatCount - visibleFlatCount);
		}

		private void HandleReset()
		{
			var flatCount = FlatCount;
			var visibleFlatCount = VisibleFlatCount;
			
			Nodes.HandleReset();

			FlatCount = 0;
			VisibleFlatCountCache = 0;

			LoadNodesRange(0, Nodes.Count);

			var flatIndex = Hierarchy.FindIndex((TNode)this, true);

			//HandleAncestorsCount(flatCountChange, visibleFlatCountChange);
			//Hierarchy.ResetNode((TNode) this, flatCountChange, visibleFlatCountChange);

			HandleCount(flatIndex + 1, FlatCount - flatCount, VisibleFlatCount - visibleFlatCount);
		}

		public bool IsAncestorOf(TNode node)
		{
			if (Hierarchy == null)
				return false;

			var current = node;

			while (current != null)
			{
				current = current.Parent;

				if (ReferenceEquals(this, current))
					return true;
			}

			return false;
		}

		public bool IsDescendantOf(TNode node)
		{
			if (Hierarchy == null)
				return false;

			return node.IsAncestorOf((TNode) this);
		}

		public void LoadNodes()
		{
			if (Hierarchy == null)
				return;

			if (CreateNodes() == false)
				return;

			LoadNodesRange(0, Nodes.Count);
		}

		private void LoadNodesRange(int index, int count)
		{
			var isExpanded = IsExpanded;

			for (var i = index; i < index + count; i++)
			{
				var node = Nodes[i];

				FlatCount++;
				VisibleFlatCountCache++;

				if (Hierarchy.IsDataExpanded(node) == false)
					continue;

				node.SetIsExpandedField(true);
				node.LoadNodes();

				VisibleFlatCountCache += node.VisibleFlatCountCache;
				FlatCount += node.FlatCount;
			}

			if (isExpanded)
				VisibleFlatCount = VisibleFlatCountCache;
		}

		internal void LoadRecursive()
		{
			try
			{
				var isLoaded = CreateNodes();

				FlatCount = 0;
				VisibleFlatCount = 0;
				VisibleFlatCountCache = 0;

				foreach (var node in Nodes)
				{
					if (isLoaded)
						node.SetIsExpandedField(Hierarchy.IsDataExpanded(node));

					node.LoadRecursive();

					FlatCount += node.FlatCount + 1;

					if (node.IsExpanded)
						VisibleFlatCountCache += node.VisibleFlatCount + 1;
					else
						VisibleFlatCountCache += 1;
				}

				if (IsExpanded)
					VisibleFlatCount = VisibleFlatCountCache;
			}
			finally
			{
				UpdateActualIsExpanded();
			}
		}

		protected virtual void OnIsExpandedChanged()
		{
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

		internal void RebuildRecursive()
		{
			try
			{
				FlatCount = 0;
				VisibleFlatCount = 0;
				VisibleFlatCountCache = 0;

				if (Nodes == null)
					return;

				foreach (var node in Nodes)
				{
					node.RebuildRecursive();

					FlatCount += node.FlatCount + 1;

					if (node.IsExpanded)
						VisibleFlatCountCache += node.VisibleFlatCount + 1;
					else
						VisibleFlatCountCache += 1;
				}

				if (IsExpanded)
					VisibleFlatCount = VisibleFlatCountCache;
			}
			finally
			{
				UpdateActualIsExpanded();
			}
		}

		internal void SetIsExpanded(bool value, ExpandCollapseMode mode)
		{
			if (Hierarchy == null)
				return;

			if (mode == ExpandCollapseMode.Auto)
				IsExpanded = value;
			else
			{
				if (mode == ExpandCollapseMode.Default)
				{
					if (Hierarchy.IsFilteredInternal == false)
						IsExpanded = value;
					else
						IsExpandedField = value;
				}
				else
				{
					if (Hierarchy.IsFilteredInternal)
						IsExpanded = value;
					else
						IsFilterExpandedField = value;
				}
			}
		}

		private void SetIsExpandedField(bool value, ExpandCollapseMode mode = ExpandCollapseMode.Auto)
		{
			if (mode == ExpandCollapseMode.Auto)
			{
				if (Hierarchy.IsFilteredInternal)
					IsFilterExpandedField = value;
				else
					IsExpandedField = value;
			}
			else if (mode == ExpandCollapseMode.Default)
				IsExpandedField = value;
			else
				IsFilterExpandedField = value;

			UpdateActualIsExpanded();
		}

		private void UnloadNodesRange(int index, int count)
		{
			var isExpanded = IsExpanded;
			var expandDecrement = isExpanded ? 1 : 0;

			for (var i = index; i < index + count; i++)
			{
				var node = Nodes[i];

				FlatCount--;
				VisibleFlatCountCache -= expandDecrement;

				VisibleFlatCountCache -= node.VisibleFlatCountCache;
				FlatCount -= node.FlatCount;
			}

			if (isExpanded)
				VisibleFlatCount = VisibleFlatCountCache;
		}

		private void UpdateActualIsExpanded()
		{
			IsActualExpandedField = IsExpanded;
		}

		public virtual void Dispose()
		{
			Hierarchy = null;
			Parent = null;

			if (Nodes != null)
				Nodes.Source = null;
		}

		private static class PackedDefinition
		{
			public static readonly PackedBoolItemDefinition IsExpanded;
			public static readonly PackedBoolItemDefinition IsFilterExpandedField;
			public static readonly PackedBoolItemDefinition IsActualExpandedField;
			public static readonly PackedBoolItemDefinition PassedFilterField;
			public static readonly PackedBoolItemDefinition VisibleField;


			static PackedDefinition()
			{
				var allocator = new PackedValueAllocator();

				IsExpanded = allocator.AllocateBoolItem();
				IsFilterExpandedField = allocator.AllocateBoolItem();
				IsActualExpandedField = allocator.AllocateBoolItem();
				PassedFilterField = allocator.AllocateBoolItem();
				VisibleField = allocator.AllocateBoolItem();
			}
		}
	}

	internal enum ExpandCollapseMode
	{
		Auto,
		Default,
		Filtered
	}
}