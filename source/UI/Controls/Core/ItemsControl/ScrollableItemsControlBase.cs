// <copyright file="ScrollableItemsControlBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using Zaaml.Core;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.UI.Controls.ScrollView;
using Zaaml.UI.Panels.Core;
using Zaaml.UI.Utils;

namespace Zaaml.UI.Controls.Core
{
	public abstract class ScrollableItemsControlBase<TControl, TItem, TCollection, TPresenter, TPanel>
		: FocusableItemsControlBase<TControl, TItem, TCollection, TPresenter, TPanel>, IIndexedScrollableFocusNavigatorAdvisor<TItem>, IScrollableFocusNavigatorAdvisor<TItem>
		where TItem : System.Windows.Controls.Control
		where TCollection : ItemCollectionBase<TControl, TItem>
		where TPresenter : ScrollableItemsPresenterBase<TControl, TItem, TCollection, TPanel>
		where TPanel : ItemsPanel<TItem>
		where TControl : System.Windows.Controls.Control
	{
		protected virtual bool CanScrollIntoView => IsItemsHostVisible;

		internal BringIntoViewMode DefaultBringIntoViewMode
		{
			get => Items.DefaultBringIntoViewMode;
			set => Items.DefaultBringIntoViewMode = value;
		}

		protected ScrollViewControl ScrollView => TemplateContract.ScrollView;

		private ScrollableItemsControlBaseTemplateContract<TPresenter> TemplateContract => (ScrollableItemsControlBaseTemplateContract<TPresenter>)TemplateContractInternal;

		private protected void BringItemIntoView(TItem item, bool updateLayout)
		{
			if (item == null)
			{
				ScrollView?.ExecuteScrollCommand(ScrollCommandKind.ScrollToHome);

				return;
			}

			if (!(ItemsPresenter?.ItemsHostInternal is IItemsHost<TItem> host))
				return;

			var bringIntoViewRequest = new BringIntoViewRequest<TItem>(item, DefaultBringIntoViewMode, 0);

			if (updateLayout)
			{
				host.EnqueueBringIntoView(bringIntoViewRequest);

				UpdateLayout();

				host.BringIntoView(bringIntoViewRequest);
			}
			else
				host.BringIntoView(bringIntoViewRequest);
		}

		internal bool IsOnCurrentPage(int index)
		{
			return IsOnCurrentPage(index, out var itemsHostRect, out var listBoxItemRect);
		}

		internal bool IsOnCurrentPage(int index, out Rect itemsHostRect, out Rect itemRect)
		{
			itemsHostRect = Rect.Empty;
			itemRect = Rect.Empty;

			try
			{
				var frameworkElement = (FrameworkElement)ScrollView?.ScrollViewPresenterInternal ?? ScrollView;

				if (frameworkElement == null)
					return true;

				itemsHostRect = new Rect(0.0, 0.0, frameworkElement.ActualWidth, frameworkElement.ActualHeight);

				var freItem = GetItemFromIndex(index);

				if (freItem == null || freItem.IsVisualDescendantOf(frameworkElement) == false)
				{
					itemRect = Rect.Empty;

					return false;
				}

				var transform = freItem.TransformToVisual(frameworkElement);

				itemRect = new Rect(transform.Transform(new Point()),
					transform.Transform(new Point(freItem.ActualWidth, freItem.ActualHeight)));

				if (HasLogicalOrientation == false)
					return itemsHostRect.Contains(itemRect);

				var orientation = LogicalOrientation;

				return itemsHostRect.GetMinPart(orientation) <= itemRect.GetMinPart(orientation) &&
							 itemRect.GetMaxPart(orientation) <= itemsHostRect.GetMaxPart(orientation);
			}
			catch (Exception ex)
			{
				LogService.LogError(ex);
			}

			return false;
		}


		protected override TemplateContract CreateTemplateContract()
		{
			var templateContract = base.CreateTemplateContract();

			if (templateContract is ScrollableItemsControlBaseTemplateContract<TPresenter> == false)
				throw new InvalidOperationException("Invalid template contract. Must be derived from ItemsControlBaseTemplateContract<>");

			return templateContract;
		}

		protected override void OnTemplateContractAttached()
		{
			base.OnTemplateContractAttached();

			if (ScrollView != null)
			{
				ScrollView.IsTabStop = false;
				ItemsPresenter.ScrollView = ScrollView;
			}
		}

		protected override void OnTemplateContractDetaching()
		{
			ItemsPresenter.ScrollView = null;

			base.OnTemplateContractDetaching();
		}

		internal void ScrollIntoView(int index)
		{
			if (CanScrollIntoView == false)
				return;

			if (index == -1 || ScrollView == null || IsItemsHostVisible == false)
				return;

			if (IsOnCurrentPage(index, out var itemsHostRect, out var itemRect))
				return;

			if (IsVirtualizing)
			{
				ItemsOverride.BringIntoView(index);

				ScrollView?.UpdateLayout();
			}
			else
			{
				var orientation = LogicalOrientation;
				var orientedViewer = new OrientedScrollViewerWrapper(ScrollView, orientation);
				var offset = orientedViewer.Offset;

				var delta = 0.0;

				var hostMin = itemsHostRect.GetMinPart(orientation);
				var hostMax = itemsHostRect.GetMaxPart(orientation);

				var itemMin = itemRect.GetMinPart(orientation);
				var itemMax = itemRect.GetMaxPart(orientation);

				if (hostMax < itemMax)
				{
					delta = itemMax - hostMax;
					offset += delta;
				}

				if (itemMin - delta < hostMin)
					offset -= hostMin - (itemMin - delta);

				orientedViewer.ScrollToOffset(offset);
			}
		}

		bool IIndexedScrollableFocusNavigatorAdvisor<TItem>.IsOnCurrentPage(int index)
		{
			return IsOnCurrentPage(index);
		}

		void IIndexedScrollableFocusNavigatorAdvisor<TItem>.ScrollIntoView(int index)
		{
			ScrollIntoView(index);
		}

		ScrollViewControl IScrollableFocusNavigatorAdvisor<TItem>.ScrollView => ScrollView;
	}

	public class ScrollableItemsControlBaseTemplateContract<TPresenter> : ItemsControlBaseTemplateContract<TPresenter>
		where TPresenter : ItemsPresenterBase
	{
		[TemplateContractPart(Required = false)]
		public ScrollViewControl ScrollView { get; [UsedImplicitly] private set; }
	}
}