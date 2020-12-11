// <copyright file="ItemsControlBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.TemplateCore;
using Panel = Zaaml.UI.Panels.Core.Panel;

namespace Zaaml.UI.Controls.Core
{
	[TemplateContractType(typeof(ItemsControlBaseTemplateContract))]
	public abstract class ItemsControlBase : TemplateContractControl, IItemsControl, ILogicalOwner
	{
		private static readonly DependencyPropertyKey HasItemsPropertyKey = DPM.RegisterReadOnly<bool, ItemsControlBase>
			("HasItems", i => i.OnHasItemsChangedPrivate);

		public static readonly DependencyProperty HasItemsProperty = HasItemsPropertyKey.DependencyProperty;

		private Size _minDesiredSize;

		internal ItemsControlBase()
		{
		}

		protected virtual bool HasLogicalOrientation => ItemsHost?.HasLogicalOrientationInt ?? false;

		protected bool IsVirtualizing => ItemsPresenter?.IsVirtualizing ?? false;

		private Panel ItemsHost => ItemsPresenter?.ItemsHostBaseInternal;

		private ItemsPresenterBase ItemsPresenter => TemplateContract.ItemsPresenterInternal;

		internal abstract Type ItemType { get; }

		protected virtual Orientation LogicalOrientation => ItemsHost?.LogicalOrientationInt ?? Orientation.Vertical;

		internal bool PreserveMinSize { get; set; }

		private ItemsControlBaseTemplateContract TemplateContract => (ItemsControlBaseTemplateContract) TemplateContractInternal;

		protected override Size ArrangeOverride(Size arrangeBounds)
		{
			_minDesiredSize.Width = Math.Max(_minDesiredSize.Width, arrangeBounds.Width);
			_minDesiredSize.Height = Math.Max(_minDesiredSize.Height, arrangeBounds.Height);

			return base.ArrangeOverride(arrangeBounds);
		}

		protected override Size MeasureOverride(Size availableSize)
		{
			var result = base.MeasureOverride(availableSize);

			if (PreserveMinSize)
			{
				var minDesiredSize = _minDesiredSize.Clamp(new Size(), availableSize);

				result = result.Clamp(minDesiredSize, XamlConstants.InfiniteSize);
			}

			return result;
		}

		internal virtual void OnCollectionChangedInternal(object sender, NotifyCollectionChangedEventArgs args)
		{
		}

		protected virtual void OnHasItemsChanged()
		{
		}

		private void OnHasItemsChangedPrivate()
		{
			OnHasItemsChanged();
		}

		internal virtual void OnItemsSourceChangedInt(IEnumerable oldSource, IEnumerable newSource)
		{
		}

		internal virtual void OnSourceChangedInternal()
		{
		}

		protected override void OnUnloaded()
		{
			_minDesiredSize = new Size();
		}

		public bool HasItems
		{
			get => (bool) GetValue(HasItemsProperty);
			internal set => this.SetReadOnlyValue(HasItemsPropertyKey, value);
		}

		void IItemsControl.OnSourceChanged()
		{
			OnSourceChangedInternal();
		}

		void IItemsControl.OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
		{
			OnCollectionChangedInternal(sender, args);
		}

		void ILogicalOwner.AddLogicalChild(object child)
		{
			AddLogicalChild(child);
		}

		void ILogicalOwner.RemoveLogicalChild(object child)
		{
			RemoveLogicalChild(child);
		}
	}

	public abstract class ItemsControlBaseTemplateContract : TemplateContract
	{
		protected abstract ItemsPresenterBase ItemsPresenterBase { get; }

		internal ItemsPresenterBase ItemsPresenterInternal => ItemsPresenterBase;
	}
}