// <copyright file="DockItemGroup.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.UI.Controls.Docking
{
	public abstract class DockItemGroup : DockItem
	{
		public static readonly DependencyProperty SelectedItemProperty = DPM.Register<DockItem, DockItemGroup>
			("SelectedItem", g => g.OnSelectedItemChangedPrivate);

		public event EventHandler SelectedItemChanged;

		protected DockItemGroup(BaseLayout layout)
		{
			AttachToView = false;
			Items = new DockItemCollection(OnItemAdded, OnItemRemoved, true);

			Layout = layout;
			Layout.SetBinding(BaseLayout.SelectedItemProperty, new Binding { Path = new PropertyPath(SelectedItemProperty), Mode = BindingMode.TwoWay, Source = this });
		}

		internal virtual bool AllowSingleItem => false;

		protected override void AttachDockControlCore(DockControl dockControl)
		{
			base.AttachDockControlCore(dockControl);

			Layout.DockControl = dockControl;
		}

		protected override void DetachDockControlCore(DockControl dockControl)
		{
			base.DetachDockControlCore(dockControl);

			Layout.DockControl = null;
		}

		public abstract DockItemGroupKind GroupKind { get; }

		internal override bool IsActualSelected => Layout.SelectedItem?.IsSelected == true;

		public DockItemCollection Items { get; }

		internal BaseLayout Layout { get; }

		public DockItem SelectedItem
		{
			get => (DockItem)GetValue(SelectedItemProperty);
			set => SetValue(SelectedItemProperty, value);
		}

		internal void AddItems(IEnumerable<DockItem> items)
		{
			foreach (var item in items)
				Items.Add(item);
		}

		internal override void AttachController(DockControllerBase controller)
		{
			base.AttachController(controller);

			Layout.AttachController(controller);
		}

		internal void ClearItems()
		{
			Items.Clear();
		}

		internal override void DetachController(DockControllerBase controller)
		{
			Layout.DetachController(controller);

			base.DetachController(controller);
		}

		internal IEnumerable<DockItem> GetChildren()
		{
			return Items.GetByDockState(DockState);
		}

		protected override void OnDockStateChanged(DockItemState oldState, DockItemState newState)
		{
			foreach (var item in Items.GetByDockState(oldState).ToList())
			{
				item.DetachGroup(item.DockState);
				
				item.DockState = DockState;
				
				item.AttachGroup(item.DockState, this);
			}

			base.OnDockStateChanged(oldState, newState);
		}

		protected virtual void OnItemAdded(DockItem item)
		{
			item.AttachGroup(DockState, this);

			item.DockStateChanged += OnItemDockStateChanged;
			item.IsSelectedChanged += OnItemIsSelectedChanged;

			UpdateStructurePrivate();
		}

		private void OnItemDockStateChanged(object sender, DockItemStateChangedEventArgs args)
		{
			OnItemDockStateChanged((DockItem)sender, args);
			UpdateStructurePrivate();
		}

		private protected virtual void OnItemDockStateChanged(DockItem item, DockItemStateChangedEventArgs args)
		{
			UpdateVisualStatePrivate();
		}

		private void OnItemIsSelectedChanged(object sender, EventArgs e)
		{
			UpdateVisualStatePrivate();
		}

		protected virtual void OnItemRemoved(DockItem item)
		{
			item.DetachGroup(this);

			item.DockStateChanged -= OnItemDockStateChanged;
			item.IsSelectedChanged -= OnItemIsSelectedChanged;

			UpdateStructurePrivate();
		}

		protected virtual void OnSelectedItemChanged(DockItem oldItem, DockItem newItem)
		{
		}

		internal virtual void OnSelectedItemChangedInternal(DockItem oldItem, DockItem newItem)
		{
			OnSelectedItemChanged(oldItem, newItem);

			SelectedItemChanged?.Invoke(this, EventArgs.Empty);
		}

		private void OnSelectedItemChangedPrivate(DockItem oldItem, DockItem newItem)
		{
			OnSelectedItemChangedInternal(oldItem, newItem);
		}

		internal void ReplaceItem(DockItem oldItem, DockItem newItem)
		{
			var index = oldItem.GetLayoutIndex(Layout);

			oldItem.SetLayoutIndex(Layout, null);
			newItem.SetLayoutIndex(Layout, index);

			Items.Replace(oldItem, newItem);
		}

		private void UpdateStructurePrivate()
		{
			AttachToView = GetChildren().Any(c => c is not DockItemGroup or DockItemGroup { AttachToView: true });
		}

		private protected override void OnAttachToViewChanged()
		{
			ParentDockGroup?.UpdateStructurePrivate();

			base.OnAttachToViewChanged();
		}

		private void UpdateVisualStatePrivate()
		{
			UpdateVisualState(true);
		}
	}

	[ContentProperty(nameof(Items))]
	[DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
	public abstract class DockItemGroup<T> : DockItemGroup where T : BaseLayout, new()
	{
		protected DockItemGroup() : base(new T())
		{
		}

		protected abstract BaseLayoutView<T> LayoutView { get; }

		protected override void OnTemplateContractAttached()
		{
			base.OnTemplateContractAttached();

			Layout.View = LayoutView;
		}

		protected override void OnTemplateContractDetaching()
		{
			Layout.View = null;

			base.OnTemplateContractDetaching();
		}
	}
}