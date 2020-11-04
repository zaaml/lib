// <copyright file="DockControllerBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Zaaml.Core;
using Zaaml.Core.Collections;
using Zaaml.Core.Extensions;
using Zaaml.Core.Pools;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore.Extensions;
using static Zaaml.UI.Controls.Docking.BaseLayout;

namespace Zaaml.UI.Controls.Docking
{
	internal abstract class DockControllerBase
	{
		private DockControl _dockControl;
		private DockItem _enqueueFocusItem;
		internal event EventHandler<DragItemEventArgs> DragItemEvent;
		internal event EventHandler<DropItemEventArgs> DropItemEvent;

		protected DockControllerBase(DockControlViewBase controlView)
		{
			ControlView = controlView;

			SelectionScope = new DockItemSelectionScope();
			Items = new DockItemCollection(OnItemAdded, OnItemRemoved);
			InternalItems = new DockItemCollection(AttachItem, DetachItem);
			DockItemGroupPool = new MultiObjectPool<DockItemGroupKind, DockItemGroup>(BuildItemGroup, OnDockItemGroupMounted, OnDockItemGroupReleased);
			SuspendState = new DelegateObservableSuspendState(OnLayoutSuspended, OnLayoutResumed);

			SelectionScope.SelectedItemChanged += OnSelectionScopeSelectedItemChanged;
		}

		internal AutoHideLayout AutoHideLayout { get; } = new AutoHideLayout();

		public DockControlViewBase ControlView { get; }

		internal DockControl DockControl
		{
			get => _dockControl;
			set
			{
				if (ReferenceEquals(_dockControl, value))
					return;

				_dockControl = value;

				foreach (var layout in DockItem.EnumerateDockStates().Select(GetLayout).SkipNull())
					layout.DockControl = _dockControl;

				foreach (var dockItem in InternalItems.OfType<DockItemGroup>())
					dockItem.DockControl = _dockControl;
			}
		}

		private MultiObjectPool<DockItemGroupKind, DockItemGroup> DockItemGroupPool { get; }

		internal DockLayout DockLayout { get; } = new DockLayout();

		public DocumentDockItemGroup DocumentGroup { get; private set; }

		public DocumentLayout DocumentLayout { get; } = new DocumentLayout();

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

		internal FloatLayout FloatLayout { get; } = new FloatLayout();

		internal HiddenLayout HiddenLayout { get; } = new HiddenLayout();

		protected DockItemCollection InternalItems { get; }

		private bool IsInArrangeItems { get; set; }

		internal bool IsItemLayoutValid { get; private set; }

		internal bool IsLayoutSuspended => SuspendState.IsSuspended;

		protected abstract bool IsPreview { get; }

		public DockItemCollection Items { get; }

		internal DockItemSelectionScope SelectionScope { get; }

		private DelegateObservableSuspendState SuspendState { get; }

		public void AfterMeasure()
		{
		}

		internal void ApplyLayout(DockControlLayout layout, bool arrange)
		{
			using (EnterLayoutSuspendState())
			{
				// Detach layouts
				foreach (var dockItem in InternalItems.ToList())
				{
					dockItem.RemoveFromLayout();
					dockItem.InvalidateItemArrange();

					dockItem.DockState = DockItemState.Hidden;
				}

				var normalizedLayout = layout.GetNormalized();

				// Destroy groups
				var itemGroups = InternalItems.OfType<DockItemGroup>().ToList();

				foreach (var group in itemGroups)
					group.Items.Clear();

				foreach (var group in itemGroups)
					ReleaseItemGroup(group);

				var layouts = normalizedLayout.Items.ToList();

				// Create groups
				foreach (var groupLayout in layouts.OfType<DockItemGroupLayout>())
				{
					var group = MountItemGroup(groupLayout.GroupKind);

					group.Name = groupLayout.ItemName;

					groupLayout.InitGroup(group);
				}

				// Set dock state
				foreach (var itemLayout in layouts)
				{
					var dockItem = GetDockItem(itemLayout);

					if (dockItem == null)
						continue;

					dockItem.DockState = itemLayout.DockState;

					LayoutSettings.CopySettings(itemLayout, dockItem, FullLayout.LayoutProperties);
				}

				// Load groups children
				var groups = new MultiMap<DockItemGroup, DockItem>();

				foreach (var groupItemLayout in layouts.OfType<DockItemGroupLayout>())
				{
					var groupItem = (DockItemGroup) GetDockItem(groupItemLayout);

					if (groupItem == null)
						continue;

					foreach (var dockItem in groupItemLayout.Items.Select(GetDockItem).SkipNull())
						groups.AddValue(groupItem, dockItem);
				}

				foreach (var group in groups)
					group.Key.AddItems(group.Value);

				EnsureDocumentGroup();

				// Restore indices
				foreach (var itemLayout in layouts)
				{
					var dockItem = GetDockItem(itemLayout.ItemName);

					var targetLayout = dockItem?.GetTargetLayout(false);

					if (targetLayout != null)
						SetDockItemIndex(dockItem, targetLayout.GetType(), DockItemLayout.GetIndex(itemLayout));
				}
			}

			if (arrange)
				ArrangeItemsImpl(true);
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

		protected internal void ArrangeItems()
		{
			ArrangeItemsImpl(false);
		}

		private void ArrangeItemsImpl(bool force)
		{
			if (IsInArrangeItems)
				return;

			try
			{
				IsInArrangeItems = true;

				if (IsItemLayoutValid && force == false)
					return;

				ArrangeDocumentGroup();

				// Arrange windows
				foreach (var item in InternalItems)
					item.PreApplyLayout();

				foreach (var item in InternalItems)
					item.ApplyLayout();

				foreach (var item in InternalItems)
					item.PostApplyLayout();
			}
			finally
			{
				IsItemLayoutValid = true;
				IsInArrangeItems = false;
			}
		}

		public void BeforeMeasure()
		{
		}

		private DockItemGroup BuildItemGroup(DockItemGroupKind dockItemGroupKind)
		{
			var group = CreateItemGroup(dockItemGroupKind);

			group.DockControl = DockControl;
			group.Name = GenerateItemName();

			return group;
		}

		private DockItemGroup CreateItemGroup(DockItemGroupKind dockItemGroupKind)
		{
			switch (dockItemGroupKind)
			{
				case DockItemGroupKind.Document:

					return new DocumentDockItemGroup {IsPreview = IsPreview};

				case DockItemGroupKind.Split:

					return new SplitDockItemGroup {IsPreview = IsPreview};

				case DockItemGroupKind.Tab:

					return new TabDockItemGroup {IsPreview = IsPreview};

				default:

					throw new ArgumentOutOfRangeException(nameof(dockItemGroupKind));
			}
		}

		private DockItemGroup CreateSplitDocumentGroup(DockItem child, DockItem parent, Dock dockSide)
		{
			var parentDocumentGroup = parent.ParentDockGroup as DocumentDockItemGroup;

			if (parentDocumentGroup == null)
				return null;

			var documentGroup = MountItemGroup<DocumentDockItemGroup>();

			documentGroup.DockState = DockItemState.Document;

			foreach (var item in DestroyGroupRecursive(child).ToList())
			{
				item.DockState = DockItemState.Document;

				documentGroup.Items.Add(item);
			}

			documentGroup.EnsureItemsIndices();

			var parentSplitDocumentGroup = parentDocumentGroup.ParentDockGroup as SplitDockItemGroup;

			if (parentSplitDocumentGroup != null && DockControl.AllowSplitDocumentsInAllDirections)
			{
				var currentParentSplitDocumentGroup = parentSplitDocumentGroup;

				parentSplitDocumentGroup = MountItemGroup<SplitDockItemGroup>();
				parentSplitDocumentGroup.DockState = DockItemState.Document;

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

			parentSplitDocumentGroup.EnsureItemsIndices();

			var index = GetActualDockItemIndex<SplitLayout>(parentDocumentGroup);

			if (dockSide == Dock.Right || dockSide == Dock.Bottom)
				index++;

			foreach (var item in parentSplitDocumentGroup.Items.Where(w => GetActualDockItemIndex<SplitLayout>(w) >= index).ToList())
				SetDockItemIndex<SplitLayout>(item, GetActualDockItemIndex<SplitLayout>(item) + 1);

			SetDockItemIndex<SplitLayout>(documentGroup, index);

			return parentSplitDocumentGroup;
		}

		protected DockItemGroup CreateSplitGroup(DockItem child, DockItem parent, Dock dockSide)
		{
			var dockState = parent.DockState;
			var orientation = Util.GetOrientation(dockSide);

			var children = child is SplitDockItemGroup childSplitGroup && childSplitGroup.Orientation == orientation ? DestroyGroup(childSplitGroup).OrderByDescending(GetActualDockItemIndex<SplitLayout>).ToList() : new List<DockItem> {child};

			if (parent.ParentDockGroup is SplitDockItemGroup parentSplitContainer && ReferenceEquals(parentSplitContainer.ActualItem, parentSplitContainer) && parentSplitContainer.Orientation == orientation)
			{
				parentSplitContainer.EnsureItemsIndices();

				var index = GetActualDockItemIndex<SplitLayout>(parent);

				if (dockSide == Dock.Right || dockSide == Dock.Bottom)
					index++;

				foreach (var item in parentSplitContainer.Items.Where(w => GetActualDockItemIndex<SplitLayout>(w) >= index).ToList())
					SetDockItemIndex<SplitLayout>(item, GetActualDockItemIndex<SplitLayout>(item) + children.Count);

				var i = index;

				foreach (var item in children)
				{
					SetDockItemIndex<SplitLayout>(item, i++);
					item.DockState = dockState;

					parentSplitContainer.Items.Add(item);
				}

				return parentSplitContainer;
			}

			var itemGroup = MountItemGroup<SplitDockItemGroup>();

			itemGroup.DockState = dockState;
			itemGroup.InvalidateItemArrange();

			var parentGroup = parent.ParentDockGroup;

			if (parentGroup is TabDockItemGroup)
			{
				parent = parentGroup;
				parentGroup = parent.ParentDockGroup;
			}

			parent.CopyTargetLayoutSettings(itemGroup);
			parentGroup?.ReplaceItem(parent, itemGroup);

			itemGroup.Orientation = orientation;

			if (dockSide == Dock.Left || dockSide == Dock.Top)
				children.Insert(0, parent);
			else
				children.Add(parent);

			foreach (var item in children.AsEnumerable().Reverse())
			{
				SetDockItemIndex<SplitLayout>(item, itemGroup.Layout.IndexProvider.NewIndex);

				item.DockState = dockState;
			}

			itemGroup.AddItems(children.AsEnumerable().Reverse());

			return itemGroup;
		}

		protected DockItemGroup CreateTabGroup(DockItem child, DockItem parent, Dock? dockSide)
		{
			var dockState = parent.DockState;

			if (ReferenceEquals(parent, DocumentGroup))
			{
				foreach (var childWindow in DestroyGroupRecursive(child))
				{
					SetDockItemIndex<TabLayout>(childWindow, null);
					childWindow.DockState = DockItemState.Document;
				}

				return DocumentGroup;
			}

			var itemGroup = (DockItemGroup) (parent.ParentDockGroup as TabDockItemGroup) ?? (parent.ParentDockGroup as DocumentDockItemGroup);

			if (itemGroup != null)
			{
				foreach (var childItem in DestroyGroupRecursive(child))
				{
					itemGroup.Items.Add(childItem);

					SetDockItemIndex<TabLayout>(childItem, itemGroup.Layout.IndexProvider.NewIndex);

					childItem.DockState = dockState;
				}
			}
			else
			{
				itemGroup = MountItemGroup<TabDockItemGroup>();
				itemGroup.DockState = dockState;

				var parentGroup = parent.ParentDockGroup;

				parent.CopyTargetLayoutSettings(itemGroup);

				SetDockItemIndex<TabLayout>(parent, itemGroup.Layout.IndexProvider.NewIndex);

				// Replace must be first.
				parentGroup?.ReplaceItem(parent, itemGroup);

				itemGroup.Items.Add(parent);

				foreach (var childItem in DestroyGroupRecursive(child))
				{
					SetDockItemIndex<TabLayout>(childItem, itemGroup.Layout.IndexProvider.NewIndex);

					childItem.DockState = dockState;
					itemGroup.Items.Add(childItem);
				}
			}

			return itemGroup;
		}

		protected IEnumerable<DockItem> DestroyGroup(DockItem item)
		{
			var group = item as DockItemGroup;

			if (group == null)
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

		internal void DropItem(DockItem source, DockItem target, DropGuideAction dropGuideAction)
		{
			DropItemEvent?.Invoke(this, new DropItemEventArgs(source, target, dropGuideAction));

			var selectedItem = source;

			if (source is DockItemGroup)
			{
				if (SelectionScope.SelectedItem != null)
					selectedItem = SelectionScope.SelectedItem;
			}

			using (EnterLayoutSuspendState())
			{
				var actionType = DropGuide.GetActionType(dropGuideAction);
				var dockSide = DropGuide.GetGuideSide(dropGuideAction);

				if (actionType == DropGuideActionType.Dock || actionType == DropGuideActionType.AutoHide || actionType == DropGuideActionType.Split && target != null && target.IsDocument())
				{
					Ungroup(source, DockItemState.Dock);

					DockLayout.SetDockSide(source, dockSide ?? Dock.Left);

					if (source.GetDependencyPropertyValueInfo(DockLayout.DockWidthProperty).IsDefaultValue)
						DockLayout.SetDockWidth(source, FloatLayout.GetFloatWidth(source));

					if (source.GetDependencyPropertyValueInfo(DockLayout.DockHeightProperty).IsDefaultValue)
						DockLayout.SetDockHeight(source, FloatLayout.GetFloatHeight(source));

					if (actionType == DropGuideActionType.Split)
					{
						foreach (var dockedItem in DockLayout.Items)
							SetDockItemIndex<DockLayout>(dockedItem, GetActualDockItemIndex<DockLayout>(dockedItem.ActualItem) + 1);

						SetDockItemIndex<DockLayout>(source, 0);
					}
					else
						SetDockItemIndex<DockLayout>(source, DockLayout.IndexProvider.NewIndex);

					if (actionType == DropGuideActionType.AutoHide && source is DockItemGroup itemGroup)
						foreach (var childWindow in itemGroup.EnumerateDescendants())
							DockLayout.SetDockSide(childWindow, dockSide ?? Dock.Left);

					source.DockState = actionType == DropGuideActionType.AutoHide ? DockItemState.AutoHide : DockItemState.Dock;
				}
				else
				{
					var group = GroupItems(source, target, dropGuideAction);

					group.SelectedItem = selectedItem;
				}
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

			var documentGroup = InternalItems.OfType<DocumentDockItemGroup>().FirstOrDefault(d => d.Name == "DocumentGroup");

			if (documentGroup != null)
			{
				DocumentGroup = documentGroup;

				foreach (var dockItem in InternalItems)
					EnsureDockItemDocumentGroup(dockItem);

				return;
			}

			DocumentGroup = MountItemGroup<DocumentDockItemGroup>();
			DocumentGroup.Name = "DocumentGroup";
			DocumentGroup.DockState = DockItemState.Document;

			foreach (var dockItem in InternalItems)
				EnsureDockItemDocumentGroup(dockItem);
		}

		protected internal IDisposable EnterLayoutSuspendState()
		{
			return SuspendState.EnterSuspendState();
		}

		public static IEnumerable<Dock> EnumerateDockSides()
		{
			yield return Dock.Left;
			yield return Dock.Top;
			yield return Dock.Right;
			yield return Dock.Bottom;
		}

		private static string GenerateItemName()
		{
			return "_" + Guid.NewGuid().ToString().Replace("-", "_");
		}

		private static int GetActualItemLayoutIndex(DockItem dockItem)
		{
			return dockItem.ActualLayout?.Items.OrderBy(i => dockItem.ActualLayout.GetDockItemIndex(i) ?? 0).IndexOf(dockItem) ?? 0;
		}

		internal DockControlLayout GetActualLayout()
		{
			ArrangeItemsImpl(true);

			var dockManagerLayout = new DockControlLayout();
			var groupLayouts = new Dictionary<DockItem, DockItemLayout>();
			var writtenProperties = new HashSet<LayoutKind>();
			var documentGroupsCount = InternalItems.OfType<DocumentDockItemGroup>().Count();
			var groups = new MultiMap<LayoutKind, DockItem>();
			var nameDictionary = new Dictionary<DockItem, string>();

			// Group items by actual layout kinds
			foreach (var item in InternalItems)
			{
				nameDictionary[item] = item.Name ?? GenerateItemName();

				// Skip document container in case documents structure is not modified
				if (ReferenceEquals(DocumentGroup, item) && documentGroupsCount < 2)
					continue;

				// Skip groups without simple items
				if (item.EnumerateDescendants(true).Count(DockItemExtensions.IsSimple) == 0)
					continue;

				groups.AddValue(item.ActualLayoutKind, item);
			}

			// Layout factory method caching container layouts
			DockItemLayout GetItemLayout(DockItem i)
			{
				var dockItemLayout = i.IsSimple() ? i.CreateItemLayout() : groupLayouts.GetValueOrCreate(i, i.CreateItemLayout);

				dockItemLayout.ItemName = nameDictionary[i];

				return dockItemLayout;
			}

			foreach (var kv in groups)
			{
				foreach (var dockItem in kv.Value)
				{
					var layouts = new List<DockItemLayout>();
					var actualItem = dockItem.ActualItem ?? dockItem;

					foreach (var dockItemGroup in dockItem.ItemGroups.Values)
					{
						// Skip document container in case documents structure is not modified
						if (ReferenceEquals(DocumentGroup, dockItemGroup) && documentGroupsCount < 2)
							continue;

						var layout = GetItemLayout(dockItem);

						DockItemLayout.SetIndex(layout, GetActualItemLayoutIndex(actualItem));

						layouts.Add(layout);

						var groupLayout = (DockItemGroupLayout) groupLayouts.GetValueOrCreate(dockItemGroup, dockItemGroup.CreateItemLayout);

						groupLayout.Items.Add(layout);

						// Copy container layout properties
						LayoutSettings.CopySettings(actualItem, layout, dockItemGroup.Layout.LayoutProperties);

						writtenProperties.Add(dockItemGroup.Layout.LayoutKind);
					}

					if (layouts.Count == 0)
					{
						// Root layout
						var layout = GetItemLayout(dockItem);

						DockItemLayout.SetIndex(layout, GetActualItemLayoutIndex(actualItem));

						LayoutSettings.CopySettings(actualItem, layout, FullLayout.LayoutProperties);

						// Do not serialize generated Id when it is not needed to bind container with layout
						if (layout.IsSimple() == false)
							layout.SkipSerializeId = true;

						dockManagerLayout.Items.Add(layout);
					}
					else if (layouts.Count > 1 || dockItem.ParentDockGroup == null)
					{
						// Shared layout, create root
						var layout = GetItemLayout(dockItem);

						DockItemLayout.SetIndex(layout, GetActualItemLayoutIndex(actualItem));

						// Copy root layout properties
						foreach (var layoutKind in FullLayout.EnumerateLayoutKinds().WhereNot(writtenProperties.Contains))
							LayoutSettings.CopySettings(actualItem, layout, FullLayout.GetLayoutProperties(layoutKind));

						dockManagerLayout.Items.Add(layout);
					}
					else
					{
						// Nested layout
						var layout = layouts[0];

						// Do not serialize generated Id when it is not needed to bind container with layout
						if (layout.IsSimple() == false)
							layout.SkipSerializeId = true;

						// Copy rest layout properties
						foreach (var layoutKind in FullLayout.EnumerateLayoutKinds().WhereNot(writtenProperties.Contains))
							LayoutSettings.CopySettings(actualItem, layout, FullLayout.GetLayoutProperties(layoutKind));
					}

					writtenProperties.Clear();
				}
			}

			dockManagerLayout.Sort();

			return dockManagerLayout;
		}

		internal DockItem GetDockItem(string name)
		{
			return name != null && InternalItems.TryGetDockItem(name, out var item) ? item : null;
		}

		internal DockItem GetDockItem(DockItemLayout dockItemLayout)
		{
			return dockItemLayout?.ItemName != null && InternalItems.TryGetDockItem(dockItemLayout.ItemName, out var item) ? item : null;
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
			return ReferenceEquals(uie, DockControl?.Controller?.DocumentLayout.View) || ReferenceEquals(uie, DockControl?.PreviewController?.DocumentLayout.View) ? DocumentGroup : uie as DockItem;
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

		public virtual BaseLayout GetTargetLayout(DockItem item, bool arrange)
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

		public void InvalidateItemArrange()
		{
			IsItemLayoutValid = false;
			ControlView?.InvalidateMeasure();
		}

		public DockItemGroup MountItemGroup(DockItemGroupKind dockItemGroupKind)
		{
			return DockItemGroupPool.GetObject(dockItemGroupKind);
		}

		public T MountItemGroup<T>() where T : DockItemGroup
		{
			return (T) MountItemGroup(GetItemGroupKind<T>());
		}

		internal void OnBeginDragMoveInternal(DockItem dockItem)
		{
			DockControl?.OnBeginDragMove(dockItem);
		}

		private void OnDockItemGroupMounted(DockItemGroupKind kind, DockItemGroup dockItemGroup)
		{
			dockItemGroup.Name = GenerateItemName();

			InternalItems.Add(dockItemGroup);
		}

		private void OnDockItemGroupReleased(DockItemGroupKind kind, DockItemGroup dockItemGroup)
		{
			InternalItems.Remove(dockItemGroup);

			dockItemGroup.Name = null;
		}

		public void OnDockStateChanged(DockItem dockItem, DockItemState oldState)
		{
			EnsureDockItemDocumentGroup(dockItem);
		}

		public void OnDockStateChanging(DockItem dockItem, DockItemState newState)
		{
		}

		internal void OnDragMoveInternal(DockItem dockItem)
		{
			DockControl?.OnItemDragMove(dockItem);
		}

		internal void OnDragOutItemInternal(DockItem dockItem)
		{
			DragItemEvent?.Invoke(this, new DragItemEventArgs(dockItem));

			using (EnterLayoutSuspendState())
			{
				dockItem.EnqueueSyncDragPosition = true;

				var actualSize = dockItem.GetActualSize();
				var floatRect = new Rect(new Point(), actualSize);
				var floatWindow = dockItem.DockState == DockItemState.Float ? dockItem.GetVisualAncestors().OfType<FloatingDockWindow>().FirstOrDefault() : null;

				floatRect = floatWindow != null ? new Rect(new Point(floatWindow.Left, floatWindow.Top), floatWindow.GetActualSize()) : floatRect.WithTopLeft(dockItem.GetScreenBox().TopLeft);

				Ungroup(dockItem, DockItemState.Float);

				dockItem.DockState = DockItemState.Hidden;

				FloatLayout.SetFloatRect(dockItem, floatRect);

				ArrangeItems();

				dockItem.DockState = DockItemState.Float;
			}
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

			InvalidateItemArrange();
		}

		private void AttachItem(DockItem dockItem)
		{
			dockItem.Controller = this;

			if (dockItem.IsSelected)
				SelectionScope.SelectedItem = dockItem;
		}

		internal virtual void OnItemArranged(DockItem dockItem)
		{
		}

		internal virtual void OnItemMeasured(DockItem dockItem)
		{
		}

		private void OnItemRemoved(DockItem dockItem)
		{
			InternalItems.Remove(dockItem);

			InvalidateItemArrange();
		}

		private void DetachItem(DockItem dockItem)
		{
			if (dockItem.IsSelected)
				SelectionScope.SelectedItem = null;

			dockItem.RemoveFromLayout();

			dockItem.Controller = null;
		}

		private void OnLayoutResumed()
		{
		}

		private void OnLayoutSuspended()
		{
			InvalidateItemArrange();
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
			var dockItemGroup = (DockItemGroup) sender;

			dockItemGroup.IsLockedChanged -= ReleaseOnUnlock;

			ReleaseItemGroup(dockItemGroup);
		}

		protected void Ungroup(DockItem item, DockItemState dockState)
		{
			var itemGroup = item.GetParentGroup(dockState);

			if (itemGroup == null)
				return;

			using (EnterLayoutSuspendState())
			{
				if (itemGroup.Items.Count == 2 && itemGroup.AllowSingleItem == false)
				{
					var neighbor = itemGroup.Items[ReferenceEquals(item, itemGroup.Items[0]) ? 1 : 0];

					if (itemGroup.DockState == neighbor.DockState)
						itemGroup.PushCurrentLayout(neighbor);

					var parentGroup = itemGroup.ParentDockGroup;

					parentGroup?.ReplaceItem(itemGroup, neighbor);

					itemGroup.ClearItems();
				}
				else
					itemGroup.Items.Remove(item);

				if (itemGroup.Items.Count > 0)
					return;

				itemGroup.ParentDockGroup?.Items.Remove(itemGroup);

				ReleaseItemGroup(itemGroup);
			}
		}

		protected void Ungroup(DockItem item)
		{
			Ungroup(item, item.DockState);
		}

		internal void UngroupInternal(DockItem item, DockItemState dockState)
		{
			Ungroup(item, dockState);
		}
	}
}