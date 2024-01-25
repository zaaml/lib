// <copyright file="DockControllerBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Zaaml.Core.Extensions;
using Zaaml.Core.Pools;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore.Extensions;

namespace Zaaml.UI.Controls.Docking
{
	internal abstract class DockControllerBase
	{
		private DockControl _dockControl;
		private DockItem _enqueueFocusItem;

		protected DockControllerBase(DockControlViewBase controlView)
		{
			ControlView = controlView;

			SelectionScope = new DockItemSelectionScope();
			Items = new DockItemCollection(OnItemAdded, OnItemRemoved);
			InternalItems = new DockItemCollection(AttachItem, DetachItem);
			DockItemGroupPool = new MultiObjectPool<DockItemGroupKind, DockItemGroup>(BuildItemGroup, OnDockItemGroupMounted, OnDockItemGroupReleased);

			SelectionScope.SelectedItemChanged += OnSelectionScopeSelectedItemChanged;

			foreach (var layout in DockItem.EnumerateDockStates().Select(GetLayout))
				layout.AttachController(this);
		}

		internal AutoHideLayout AutoHideLayout { get; } = new();

		public DockControlViewBase ControlView { get; }

		internal DockControl DockControl
		{
			get => _dockControl;
			set
			{
				if (ReferenceEquals(_dockControl, value))
					return;

				foreach (var dockItem in InternalItems.OfType<DockItemGroup>())
					dockItem.DetachDockControlInternal(_dockControl);

				_dockControl = value;

				foreach (var layout in DockItem.EnumerateDockStates().Select(GetLayout).SkipNull())
					layout.DockControl = _dockControl;

				foreach (var dockItem in InternalItems.OfType<DockItemGroup>())
					dockItem.AttachDockControlInternal(_dockControl);

				EnsureDocumentGroup();
			}
		}

		private MultiObjectPool<DockItemGroupKind, DockItemGroup> DockItemGroupPool { get; }

		internal DockLayout DockLayout { get; } = new();

		public DocumentDockItemGroup DocumentGroup { get; private set; }

		public DocumentLayout DocumentLayout { get; } = new();

		internal virtual bool DragOutWithActualSize => false;

		public DockItem EnqueueFocusItem
		{
			get => _enqueueFocusItem;
			set
			{
				if (ReferenceEquals(_enqueueFocusItem, value))
					return;

				_enqueueFocusItem = value;

				_enqueueFocusItem?.InvalidateArrange();
			}
		}

		internal FloatLayout FloatLayout { get; } = new();

		internal HiddenLayout HiddenLayout { get; } = new();

		protected DockItemCollection InternalItems { get; }

		protected abstract bool IsPreview { get; }

		public DockItemCollection Items { get; }

		internal DockItemSelectionScope SelectionScope { get; }
		internal event EventHandler<DragItemEventArgs> DragItemEvent;
		internal event EventHandler<DropItemEventArgs> DropItemEvent;

		internal void ApplyLayout(DockControlLayout layout)
		{
			foreach (var dockItem in InternalItems.ToList())
			{
				dockItem.ResetLayout();
				dockItem.DockState = DockItemState.Hidden;

				if (dockItem is not DockItemGroup groupItem)
					continue;

				groupItem.Items.Clear();

				ReleaseItemGroup(groupItem);
			}

			DockItem LoadLayout(DockItemLayout itemLayout)
			{
				var groupLayout = itemLayout as DockItemGroupLayout;
				var item = groupLayout != null ? MountItemGroup(groupLayout) : GetDockItem(itemLayout);

				if (item == null)
					return null;

				item.DockState = itemLayout.DockState;

				LayoutSettings.MergeSettings(itemLayout, item, FullLayout.LayoutProperties);

				if (groupLayout == null)
					return item;

				var groupItem = (DockItemGroup)item;

				foreach (var childItem in groupLayout.Items)
				{
					var dockItem = LoadLayout(childItem);

					if (dockItem != null)
						groupItem.Items.Add(dockItem);
				}

				return item;
			}

			foreach (var itemLayout in layout.Items)
				LoadLayout(itemLayout);

			EnsureDocumentGroup();
		}

		private void ArrangeDocumentGroup()
		{
			EnsureDocumentGroup();

			if (DocumentGroup.DockState != DockItemState.Document || DocumentGroup.Items.Count == 0)
				return;

			var splitDocumentGroup = InternalItems.OfType<SplitDockItemGroup>().FirstOrDefault(s => s.DockState == DockItemState.Document && s.ParentDockGroup == null);

			if (splitDocumentGroup != null && DocumentGroup.ParentDockGroup == null)
				splitDocumentGroup.Items.Insert(0, DocumentGroup);
		}

		private void AttachItem(DockItem dockItem)
		{
			dockItem.Controller = this;

			if (dockItem.IsSelected)
				SelectionScope.SelectedItem = dockItem;
		}

		private DockItemGroup BuildItemGroup(DockItemGroupKind dockItemGroupKind)
		{
			var group = CreateItemGroup(dockItemGroupKind);

			group.AttachDockControlInternal(DockControl);

			return group;
		}

		private DockItemGroup CopyGroup(DockItemGroup group, DockItemState dockState)
		{
			var clone = MountItemGroup(group.GroupKind);

			clone.DockState = dockState;

			CopyGroupContent(clone, group);

			return clone;
		}

		private void CopyGroupContent(DockItemGroup clone, DockItemGroup group)
		{
			foreach (var item in group.GetChildren())
			{
				if (item is DockItemGroup childGroup)
					clone.Items.Add(CopyGroup(childGroup, clone.DockState));
				else
					clone.Items.Add(item);
			}
		}

		private DockItemGroup CreateItemGroup(DockItemGroupKind dockItemGroupKind)
		{
			switch (dockItemGroupKind)
			{
				case DockItemGroupKind.Document:

					return new DocumentDockItemGroup { IsPreview = IsPreview };

				case DockItemGroupKind.Split:

					return new SplitDockItemGroup { IsPreview = IsPreview };

				case DockItemGroupKind.Tab:

					return new TabDockItemGroup { IsPreview = IsPreview };

				default:

					throw new ArgumentOutOfRangeException(nameof(dockItemGroupKind));
			}
		}

		private DockItemGroup CreateSplitDocumentGroup(DockItem child, DockItem parent, Dock dockSide)
		{
			if (parent.ParentDockGroup is not DocumentDockItemGroup parentDocumentGroup)
				return null;

			var documentGroup = MountItemGroup<DocumentDockItemGroup>();

			documentGroup.DockState = DockItemState.Document;

			foreach (var item in DestroyGroupRecursive(child).ToList())
			{
				documentGroup.Items.Add(item);

				item.DockState = DockItemState.Document;
			}

			var parentSplitDocumentGroup = parentDocumentGroup.ParentDockGroup as SplitDockItemGroup;

			if (parentSplitDocumentGroup != null && parentSplitDocumentGroup.Orientation == Util.GetOrientation(dockSide))
			{
				var idx = parentDocumentGroup.GetLayoutIndex(parentSplitDocumentGroup.Layout);

				if (dockSide is Dock.Right or Dock.Bottom)
					idx++;

				foreach (var item in parentSplitDocumentGroup.Items.Where(w => w.GetLayoutIndex(parentSplitDocumentGroup.Layout) >= idx).ToList())
					item.SetLayoutIndex(parentSplitDocumentGroup.Layout, item.GetLayoutIndex(parentSplitDocumentGroup.Layout) + 1);

				documentGroup.SetLayoutIndex(parentSplitDocumentGroup.Layout, idx);

				parentSplitDocumentGroup.Items.Add(documentGroup);

				return parentSplitDocumentGroup;
			}

			if (parentSplitDocumentGroup != null && DockControl.AllowSplitDocumentsInAllDirections)
			{
				var currentParentSplitDocumentGroup = parentSplitDocumentGroup;

				parentSplitDocumentGroup = MountItemGroup<SplitDockItemGroup>();
				parentSplitDocumentGroup.DockState = DockItemState.Document;

				parentDocumentGroup.CopyTargetLayoutIndex(parentSplitDocumentGroup);
				parentDocumentGroup.CopyTargetLayoutSettings(parentSplitDocumentGroup);
				currentParentSplitDocumentGroup.ReplaceItem(parentDocumentGroup, parentSplitDocumentGroup);

				parentSplitDocumentGroup.Items.Add(parentDocumentGroup);
			}
			else if (parentSplitDocumentGroup == null)
			{
				parentSplitDocumentGroup = MountItemGroup<SplitDockItemGroup>();
				parentSplitDocumentGroup.DockState = DockItemState.Document;

				parentSplitDocumentGroup.Items.Add(parentDocumentGroup);
			}

			parentSplitDocumentGroup.Orientation = Util.GetOrientation(dockSide);

			parentSplitDocumentGroup.Items.Add(documentGroup);

			var index = parentDocumentGroup.GetLayoutIndex(parentSplitDocumentGroup.Layout);

			if (dockSide is Dock.Right or Dock.Bottom)
				index++;

			foreach (var item in parentSplitDocumentGroup.Items.Where(w => w.GetLayoutIndex(parentSplitDocumentGroup.Layout) >= index).ToList())
				item.SetLayoutIndex(parentSplitDocumentGroup.Layout, item.GetLayoutIndex(parentSplitDocumentGroup.Layout) + 1);

			documentGroup.SetLayoutIndex(parentSplitDocumentGroup.Layout, index);

			return parentSplitDocumentGroup;
		}

		protected DockItemGroup CreateSplitGroup(DockItem child, DockItem parent, Dock dockSide)
		{
			var dockState = parent.DockState;
			var orientation = Util.GetOrientation(dockSide);

			var children = child is SplitDockItemGroup childSplitGroup && childSplitGroup.Orientation == orientation
				? DestroyGroup(childSplitGroup).OrderByDescending(i => i.GetLayoutIndex(childSplitGroup.Layout)).ToList()
				: new List<DockItem> { child };

			if (parent.ParentDockGroup is SplitDockItemGroup parentSplitContainer && parentSplitContainer.Orientation == orientation)
			{
				var index = parent.GetLayoutIndex(parentSplitContainer.Layout);

				if (dockSide is Dock.Right or Dock.Bottom)
					index++;

				foreach (var item in parentSplitContainer.Items.Where(w => w.GetLayoutIndex(parentSplitContainer.Layout) >= index).ToList())
					item.SetLayoutIndex(parentSplitContainer.Layout, item.GetLayoutIndex(parentSplitContainer.Layout) + children.Count);

				var i = index;

				foreach (var item in children)
				{
					item.SetLayoutIndex(parentSplitContainer.Layout, i++);

					item.DockState = dockState;

					parentSplitContainer.Items.Add(item);
				}

				return parentSplitContainer;
			}

			var itemGroup = MountItemGroup<SplitDockItemGroup>();

			itemGroup.DockState = dockState;

			var parentGroup = parent.ParentDockGroup;

			if (parentGroup is TabDockItemGroup)
			{
				parent = parentGroup;
				parentGroup = parent.ParentDockGroup;
			}

			parent.CopyTargetLayoutIndex(itemGroup);
			parent.CopyTargetLayoutSettings(itemGroup);
			parentGroup?.ReplaceItem(parent, itemGroup);

			itemGroup.Orientation = orientation;

			if (dockSide is Dock.Left or Dock.Top)
				children.Insert(0, parent);
			else
				children.Add(parent);

			foreach (var item in children.AsEnumerable().Reverse())
			{
				item.SetNewLayoutIndex(itemGroup.Layout);

				item.DockState = dockState;
			}

			itemGroup.AddItems(children);

			return itemGroup;
		}

		protected DockItemGroup CreateTabGroup(DockItem child, DockItem parent, Dock? dock)
		{
			var dockState = parent.DockState;

			if (ReferenceEquals(parent, DocumentGroup))
			{
				foreach (var childItem in DestroyGroupRecursive(child))
				{
					childItem.ClearLayoutIndex(DocumentGroup.Layout);

					childItem.DockState = DockItemState.Document;
				}

				return DocumentGroup;
			}

			var itemGroup = (DockItemGroup)(parent.ParentDockGroup as TabDockItemGroup) ?? parent.ParentDockGroup as DocumentDockItemGroup;

			if (itemGroup != null)
			{
				foreach (var childItem in DestroyGroupRecursive(child))
				{
					itemGroup.Items.Add(childItem);
					childItem.SetNewLayoutIndex(itemGroup.Layout);

					childItem.DockState = dockState;
				}
			}
			else
			{
				itemGroup = MountItemGroup<TabDockItemGroup>();
				itemGroup.DockState = dockState;

				var parentGroup = parent.ParentDockGroup;

				parent.CopyTargetLayoutIndex(itemGroup);
				parent.CopyTargetLayoutSettings(itemGroup);

				parentGroup?.ReplaceItem(parent, itemGroup);

				itemGroup.Items.Add(parent);

				foreach (var childItem in DestroyGroupRecursive(child))
				{
					childItem.SetNewLayoutIndex(itemGroup.Layout);
					childItem.DockState = dockState;
					itemGroup.Items.Add(childItem);
				}
			}

			return itemGroup;
		}

		protected IEnumerable<DockItem> DestroyGroup(DockItem item)
		{
			if (item is not DockItemGroup group)
				yield return item;
			else
			{
				var children = group.Items.ToList();

				group.Items.Clear();

				ReleaseItemGroup(group);

				foreach (var child in children)
					yield return child;
			}
		}

		protected IEnumerable<DockItem> DestroyGroupRecursive(DockItem rootDockItem)
		{
			foreach (var dockItem in DestroyGroup(rootDockItem))
			{
				if (dockItem.IsSimple())
					yield return dockItem;
				else
					foreach (var childDockItem in DestroyGroupRecursive(dockItem))
						yield return childDockItem;
			}
		}

		private void DetachItem(DockItem dockItem)
		{
			if (dockItem.IsSelected)
				SelectionScope.SelectedItem = null;

			dockItem.Controller = null;
		}

		internal void DropItem(DockItem source, DockItem target, DropGuideAction dropGuideAction)
		{
			DropItemEvent?.Invoke(this, new DropItemEventArgs(source, target, dropGuideAction));

			var selectedItem = source;

			if (source is DockItemGroup)
			{
				if (SelectionScope.SelectedItem != null)
					selectedItem = SelectionScope.SelectedItem;
			}

			var actionType = DropGuide.GetActionType(dropGuideAction);
			var dockSide = DropGuide.GetGuideSide(dropGuideAction);

			if (actionType == DropGuideActionType.Dock || actionType == DropGuideActionType.AutoHide || actionType == DropGuideActionType.Split && target != null && target.IsDocument())
			{
				Ungroup(source, DockItemState.Dock);

				DockLayout.SetDock(source, dockSide ?? Dock.Left);

				if (source.GetDependencyPropertyValueInfo(DockLayout.WidthProperty).IsDefaultValue)
					DockLayout.SetWidth(source, FloatLayout.GetWidth(source));

				if (source.GetDependencyPropertyValueInfo(DockLayout.HeightProperty).IsDefaultValue)
					DockLayout.SetHeight(source, FloatLayout.GetHeight(source));

				if (actionType == DropGuideActionType.Split)
				{
					foreach (var dockedItem in DockLayout.Items)
						dockedItem.SetLayoutIndex(DockLayout, dockedItem.GetLayoutIndex(DockLayout) + 1);

					source.SetLayoutIndex(DockLayout, 0);
				}
				else
					source.SetLayoutIndex(DockLayout, DockLayout.GetNewLayoutIndex());

				if (actionType == DropGuideActionType.AutoHide && source is DockItemGroup itemGroup)
					foreach (var childWindow in itemGroup.EnumerateDescendants())
						DockLayout.SetDock(childWindow, dockSide ?? Dock.Left);

				source.DockState = actionType == DropGuideActionType.AutoHide ? DockItemState.AutoHide : DockItemState.Dock;
			}
			else
			{
				var group = GroupItems(source, target, dropGuideAction);

				group.SelectedItem = selectedItem;
			}

			SelectionScope.SelectedItem = selectedItem;

			if (IsPreview)
				return;

			EnqueueFocusItem = source;
		}

		private void EnsureDockItemDocumentGroup(DockItem dockItem)
		{
			if (DocumentGroup == null)
				return;

			if (dockItem.DockState != DockItemState.Document || dockItem.IsSimple() == false)
				return;

			var itemGroup = dockItem.GetParentGroup(DockItemState.Document);

			if (itemGroup == null)
				DocumentGroup.Items.Add(dockItem);
		}

		private void EnsureDocumentGroup()
		{
			if (DocumentGroup != null)
				return;

			var documentGroup = InternalItems.OfType<DocumentDockItemGroup>().FirstOrDefault();

			if (documentGroup != null)
			{
				DocumentGroup = documentGroup;

				foreach (var dockItem in InternalItems)
					EnsureDockItemDocumentGroup(dockItem);

				return;
			}

			DocumentGroup = MountItemGroup<DocumentDockItemGroup>();
			DocumentGroup.DockState = DockItemState.Document;

			foreach (var dockItem in InternalItems)
				EnsureDockItemDocumentGroup(dockItem);
		}

		public static IEnumerable<Dock> EnumerateDockSides()
		{
			yield return Dock.Left;
			yield return Dock.Top;
			yield return Dock.Right;
			yield return Dock.Bottom;
		}

		private static int GetActualItemOrder(DockItem dockItem)
		{
			return dockItem.GetTargetLayout()?.GetDockItemOrderInternal(dockItem) ?? 0;
		}

		internal DockControlLayout GetActualLayout()
		{
			var dockControlLayout = new DockControlLayout();
			var itemLayouts = new Dictionary<DockItem, DockItemLayout>();

			DockItemLayout CreateItemLayout(DockItem dockItem)
			{
				var layout = dockItem.CreateItemLayout();

				LayoutSettings.CopySettings(dockItem, layout, FullLayout.LayoutProperties);

				if (dockItem is DockItemGroup group)
				{
					var groupLayout = (DockItemGroupLayout)layout;

					foreach (var child in group.Items.OrderBy(d => d.GetLayoutIndex(group.Layout)))
						groupLayout.Items.Add(itemLayouts.GetValueOrCreate(child, CreateItemLayout));
				}

				return layout;
			}

			foreach (var item in InternalItems.Where(d => d.IsRoot).OrderBy(d => d.DockState).ThenBy(d => d.GetLayoutIndex(d.GetTargetLayout())))
				dockControlLayout.Items.Add(itemLayouts.GetValueOrCreate(item, CreateItemLayout));

			dockControlLayout.Merge();

			return dockControlLayout;
		}

		public static IEnumerable<DependencyProperty> GetLayoutProperties(DockItemState dockState)
		{
			switch (dockState)
			{
				case DockItemState.Dock:

					return BaseLayout.GetLayoutProperties<DockLayout>();

				case DockItemState.Float:

					return BaseLayout.GetLayoutProperties<FloatLayout>();

				case DockItemState.AutoHide:

					return BaseLayout.GetLayoutProperties<AutoHideLayout>();

				case DockItemState.Document:

					return BaseLayout.GetLayoutProperties<DocumentLayout>();

				case DockItemState.Hidden:

					return BaseLayout.GetLayoutProperties<HiddenLayout>();
			}

			throw new ArgumentOutOfRangeException(nameof(dockState));
		}

		internal DockItem GetDockItem(string name)
		{
			return name != null && InternalItems.TryGetDockItem(name, out var item) ? item : null;
		}

		internal DockItem GetDockItem(DockItemLayout dockItemLayout)
		{
			return dockItemLayout?.ItemNameInternal != null && InternalItems.TryGetDockItem(dockItemLayout.ItemNameInternal, out var item) ? item : null;
		}

		private static DockItemGroupKind GetItemGroupKind<T>() where T : DockItemGroup
		{
			return GetItemGroupKind(typeof(T));
		}

		private static DockItemGroupKind GetItemGroupKind(Type containerType)
		{
			if (containerType == typeof(DocumentDockItemGroup))
				return DockItemGroupKind.Document;

			if (containerType == typeof(SplitDockItemGroup))
				return DockItemGroupKind.Split;

			if (containerType == typeof(TabDockItemGroup))
				return DockItemGroupKind.Tab;

			throw new Exception("Invalid group type");
		}

		internal DockItem GetItemIfDocumentLayout(UIElement uie)
		{
			var itemIfDocumentLayout = ReferenceEquals(uie, DockControl?.Controller?.DocumentLayout.View) || ReferenceEquals(uie, DockControl?.PreviewController?.DocumentLayout.View) ? DocumentGroup : uie as DockItem;

			if (DocumentGroup != null && ReferenceEquals(itemIfDocumentLayout, DocumentGroup) && DocumentGroup.GetChildren().Any())
				return null;

			return itemIfDocumentLayout;
		}

		public BaseLayout GetLayout(DockItemState dockState)
		{
			switch (dockState)
			{
				case DockItemState.Dock:

					return DockLayout;

				case DockItemState.Float:

					return FloatLayout;

				case DockItemState.AutoHide:

					return AutoHideLayout;

				case DockItemState.Document:

					return DocumentLayout;

				case DockItemState.Hidden:

					return HiddenLayout;
			}

			throw new ArgumentOutOfRangeException(nameof(dockState));
		}

		public virtual BaseLayout GetTargetLayout(DockItem item)
		{
			return GetLayout(item.DockState);
		}

		protected DockItemGroup GroupItems(DockItem child, DockItem parent, DropGuideAction dropGuideAction)
		{
			if (!parent.IsDock() && !parent.IsFloat() && !parent.IsDocument() && !ReferenceEquals(parent, DocumentGroup))
				return null;

			if (ReferenceEquals(parent, DocumentGroup))
			{
				foreach (var dockItem in child.EnumerateItems().Where(d => d.IsSimple()).ToList())
					dockItem.DockState = DockItemState.Document;

				return DocumentGroup;
			}

			var dockSide = DropGuide.GetGuideSide(dropGuideAction);
			var actionType = DropGuide.GetActionType(dropGuideAction);
			var dockState = parent.DockState;

			Ungroup(child, dockState);

			switch (actionType)
			{
				case DropGuideActionType.Split:

					return CreateSplitGroup(child, parent, dockSide ?? Dock.Left);

				case DropGuideActionType.SplitDocument:

					return CreateSplitDocumentGroup(child, parent, dockSide ?? Dock.Left);

				case DropGuideActionType.Tab:

					return CreateTabGroup(child, parent, dockSide);
			}

			return null;
		}

		private DockItemGroup MountItemGroup(DockItemGroupLayout groupLayout)
		{
			var group = MountItemGroup(groupLayout.GroupKind);

			groupLayout.InitGroup(group);

			return group;
		}

		public DockItemGroup MountItemGroup(DockItemGroupKind dockItemGroupKind)
		{
			return DockItemGroupPool.GetObject(dockItemGroupKind);
		}

		public T MountItemGroup<T>() where T : DockItemGroup
		{
			return (T)MountItemGroup(GetItemGroupKind<T>());
		}

		internal void OnBeginDragMoveInternal(DockItem dockItem)
		{
			DockControl?.OnBeginDragMove(dockItem);
		}

		private void OnDockItemGroupMounted(DockItemGroupKind kind, DockItemGroup dockItemGroup)
		{
			InternalItems.Add(dockItemGroup);
		}

		private void OnDockItemGroupReleased(DockItemGroupKind kind, DockItemGroup dockItemGroup)
		{
			dockItemGroup.DockState = DockItemState.Hidden;

			InternalItems.Remove(dockItemGroup);

			LayoutSettings.ClearSettings(dockItemGroup, FullLayout.LayoutProperties);
		}

		internal void OnDragMoveInternal(DockItem dockItem)
		{
			DockControl?.OnItemDragMove(dockItem);
		}

		internal void OnDragOutItemInternal(DockItem dockItem)
		{
			DragItemEvent?.Invoke(this, new DragItemEventArgs(dockItem));

			dockItem.EnqueueSyncDragPosition = true;

			var floatRect = new Rect(new Point(), DragOutWithActualSize ? dockItem.GetActualSize() : FloatLayout.GetSize(dockItem));
			var floatWindow = dockItem.DockState == DockItemState.Float ? dockItem.GetVisualAncestors().OfType<FloatingDockWindow>().FirstOrDefault() : null;

			floatRect = floatWindow != null ? new Rect(new Point(floatWindow.Left, floatWindow.Top), floatWindow.GetActualSize()) : floatRect.WithTopLeft(dockItem.GetScreenLogicalBox().TopLeft);

			Ungroup(dockItem, DockItemState.Float);

			var group = dockItem as DockItemGroup;

			DockItemGroup groupCopy = null;

			if (group != null)
			{
				var parentGroup = group.ParentDockGroup;

				groupCopy = MountItemGroup(group.GroupKind);
				groupCopy.DockState = dockItem.DockState;

				parentGroup?.ReplaceItem(dockItem, groupCopy);

				dockItem.CopyTargetLayoutIndex(groupCopy);
				dockItem.CopyTargetLayoutSettings(groupCopy);
			}

			//TODO Why Hidden?
			//dockItem.DockState = DockItemState.Hidden;

			FloatLayout.SetRect(dockItem, floatRect);

			dockItem.DockState = DockItemState.Float;
			dockItem.EnqueueDragMove();

			if (groupCopy != null)
				CopyGroupContent(groupCopy, group);
		}

		public void OnEndDragMoveInternal(DockItem dockItem)
		{
			DockControl?.OnEndDragMove(dockItem);

			dockItem.EnqueueSyncDragPosition = false;
		}

		private void OnItemAdded(DockItem dockItem)
		{
			InternalItems.Add(dockItem);

			EnsureDockItemDocumentGroup(dockItem);
		}

		internal virtual void OnItemArranged(DockItem dockItem)
		{
		}

		protected virtual void OnItemDockStateChanged(DockItem dockItem, DockItemState oldState, DockItemState newState)
		{
		}

		internal void OnItemDockStateChangedInternal(DockItem dockItem, DockItemState oldState, DockItemState newState)
		{
			EnsureDockItemDocumentGroup(dockItem);

			OnItemDockStateChanged(dockItem, oldState, newState);
		}

		internal virtual void OnItemMeasured(DockItem dockItem)
		{
		}

		private void OnItemRemoved(DockItem dockItem)
		{
			foreach (var dockState in DockItem.EnumerateDockStates())
			{
				var parentGroup = dockItem.GetParentGroup(dockState);

				parentGroup?.Items.Remove(dockItem);

				var layout = GetLayout(dockState);

				layout?.Items.Remove(dockItem);
			}

			InternalItems.Remove(dockItem);
		}

		private void OnSelectionScopeSelectedItemChanged(object sender, EventArgs e)
		{
			if (IsPreview || DockControl == null)
				return;

			DockControl.SelectedItem = SelectionScope.SelectedItem;
			DockControl.PreviewController.SelectionScope.SelectedItem = SelectionScope.SelectedItem?.PreviewItem;
		}

		private void ReleaseItemGroup(DockItemGroup dockItemGroup)
		{
			if (ReferenceEquals(dockItemGroup.Controller, this) == false)
				return;

			if (dockItemGroup.IsLocked)
			{
				dockItemGroup.IsLockedChanged += ReleaseOnUnlock;

				return;
			}

			if (ReferenceEquals(dockItemGroup, DocumentGroup))
				DocumentGroup = null;

			DockItemGroupPool.Release(dockItemGroup.GroupKind, dockItemGroup);
		}

		private void ReleaseOnUnlock(object sender, EventArgs e)
		{
			var dockItemGroup = (DockItemGroup)sender;

			dockItemGroup.IsLockedChanged -= ReleaseOnUnlock;

			ReleaseItemGroup(dockItemGroup);
		}

		protected void Ungroup(DockItem item, DockItemState dockState)
		{
			var itemGroup = item.GetParentGroup(dockState);

			if (itemGroup == null)
				return;

			var parentGroup = itemGroup.ParentDockGroup;

			if (itemGroup.Items.Count == 2 && itemGroup.AllowSingleItem == false)
			{
				var neighbor = itemGroup.Items[ReferenceEquals(item, itemGroup.Items[0]) ? 1 : 0];

				if (itemGroup.DockState == neighbor.DockState)
				{
					itemGroup.CopyTargetLayoutIndex(neighbor);
					itemGroup.CopyTargetLayoutSettings(neighbor);
				}

				parentGroup?.ReplaceItem(itemGroup, neighbor);

				itemGroup.ClearItems();
			}
			else
				itemGroup.Items.Remove(item);

			if (itemGroup.Items.Count > 0)
				return;

			Ungroup(itemGroup, dockState);

			ReleaseItemGroup(itemGroup);
		}
	}
}