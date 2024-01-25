// <copyright file="BaseLayoutView.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.Docking
{
	public abstract class BaseLayoutView : TemplateContractContentControl
	{
		public static readonly DependencyProperty SelectedItemProperty = DPM.Register<DockItem, BaseLayoutView>
			("SelectedItem", v => v.OnSelectedItemChangedPrivate);

		private bool _isArrangeValid;
		private BaseLayout _layoutInternal;

		protected BaseLayoutView()
		{
			Items = new DockItemCollection(OnItemAddedPrivate, OnItemRemovedPrivate);
		}

		internal DockControl DockControl => LayoutInternal?.DockControl;

		protected internal DockItemCollection Items { get; }

		internal BaseLayout LayoutInternal
		{
			get => _layoutInternal;
			set
			{
				if (ReferenceEquals(_layoutInternal, value))
					return;

				if (_layoutInternal != null)
					OnLayoutDetaching();

				_layoutInternal = value;

				if (_layoutInternal != null)
					OnLayoutAttached();
			}
		}

		protected internal IEnumerable<DockItem> OrderedItems => Items.OrderBy(GetDockItemOrder);

		protected internal IEnumerable<DockItem> ReverseOrderedItems => Items.OrderByDescending(GetDockItemOrder);

		public DockItem SelectedItem
		{
			get => (DockItem)GetValue(SelectedItemProperty);
			set => SetValue(SelectedItemProperty, value);
		}

		protected internal abstract void ArrangeItems();

		protected virtual int GetDockItemOrder(DockItem dockItem)
		{
			return LayoutInternal.GetDockItemOrderInternal(dockItem);
		}

		protected virtual void InvalidateItemsArrange()
		{
			if (_isArrangeValid == false)
				return;

			_isArrangeValid = false;

			InvalidateMeasure();
		}

		internal virtual bool IsItemVisible(DockItem item)
		{
			return true;
		}

		protected override Size MeasureOverride(Size availableSize)
		{
			if (_isArrangeValid)
				return base.MeasureOverride(availableSize);

			ArrangeItems();

			_isArrangeValid = true;

			return base.MeasureOverride(availableSize);
		}

		protected abstract void OnItemAdded(DockItem dockItem);

		private void OnItemAddedPrivate(DockItem dockItem)
		{
			OnItemAdded(dockItem);
			InvalidateItemsArrange();
		}

		protected abstract void OnItemRemoved(DockItem dockItem);

		private void OnItemRemovedPrivate(DockItem dockItem)
		{
			OnItemRemoved(dockItem);
			InvalidateItemsArrange();
		}

		protected virtual void OnLayoutAttached()
		{
			SetBinding(SelectedItemProperty, new Binding { Path = new PropertyPath(BaseLayout.SelectedItemProperty), Mode = BindingMode.TwoWay, Source = LayoutInternal });
		}

		protected virtual void OnLayoutDetaching()
		{
			ClearValue(SelectedItemProperty);
		}

		internal virtual void OnSelectedItemChangedInternal(DockItem oldItem, DockItem newItem)
		{
		}

		private void OnSelectedItemChangedPrivate(DockItem oldItem, DockItem newItem)
		{
		}

		protected override void OnTemplateContractAttached()
		{
			base.OnTemplateContractAttached();

			InvalidateItemsArrange();
		}

		protected override void OnTemplateContractDetaching()
		{
			InvalidateItemsArrange();

			base.OnTemplateContractDetaching();
		}
	}

	public abstract class BaseLayoutView<T> : BaseLayoutView where T : BaseLayout
	{
		protected T Layout => (T)LayoutInternal;
	}
}