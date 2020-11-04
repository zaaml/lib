// <copyright file="ScrollViewPresenterBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Media;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.Utils;
using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.ScrollView
{
	public abstract class ScrollViewPresenterBase<TScrollContentPanel> : FixedTemplateControl<TScrollContentPanel> where TScrollContentPanel : ScrollViewPanelBase
	{
		private readonly RectangleGeometry _clipGeometry = new RectangleGeometry();
		private FrameworkElement _child;
		private ScrollViewControlBase _scrollView;

		static ScrollViewPresenterBase()
		{
			ControlUtils.OverrideIsTabStop<ScrollViewPresenterBase<TScrollContentPanel>>(false);
		}

		protected ScrollViewPresenterBase()
		{
			Clip = _clipGeometry;
		}

		internal FrameworkElement Child
		{
			get => _child;
			set
			{
				if (ReferenceEquals(_child, value))
					return;

				DetachChild();

				_child = value;

				AttachChild();
			}
		}

		internal ScrollViewControlBase ScrollView
		{
			get => _scrollView;
			set
			{
				if (ReferenceEquals(_scrollView, value))
					return;

				if (_scrollView != null)
				{
					Child = null;

					OnScrollViewDetached();
				}

				_scrollView = value;

				if (_scrollView != null)
				{
					Child = _scrollView.Child;

					OnScrollViewAttached();
				}
			}
		}

		private TScrollContentPanel ScrollViewPanel => TemplateRoot;

		internal TScrollContentPanel ScrollViewPanelInternal => ScrollViewPanel;

		protected sealed override void ApplyTemplateOverride()
		{
			base.ApplyTemplateOverride();

			AttachChild();

			OnTemplateAttached();
		}

		protected sealed override Size ArrangeOverride(Size arrangeBounds)
		{
			var scrollViewPanel = ScrollViewPanel;

			if (scrollViewPanel == null)
				return arrangeBounds;

			if (scrollViewPanel.IsScrollClientInternal)
			{
				var scaleX = scrollViewPanel.ScaleXInternal;
				var scaleY = scrollViewPanel.ScaleYInternal;
				var padding = scrollViewPanel.PaddingInternal;
				var finalRect = new Rect(new Point(), scrollViewPanel.DesiredSize);

				if (finalRect.Width * scaleX < arrangeBounds.Width)
					finalRect.Width = arrangeBounds.Width / scaleX;

				if (finalRect.Height * scaleY < arrangeBounds.Height)
					finalRect.Height = arrangeBounds.Height / scaleY;

				finalRect.Width += padding.Width() / scaleX;
				finalRect.Height += padding.Height() / scaleY;

				scrollViewPanel.Arrange(finalRect);
			}
			else
				base.ArrangeOverride(arrangeBounds);

			_clipGeometry.Rect = new Rect(new Point(), arrangeBounds);

			return arrangeBounds;
		}

		private void AttachChild()
		{
			if (_child == null || TemplateRoot == null)
				return;

			if (ScrollView != null)
				ScrollView.LogicalChild = null;

			TemplateRoot.Child = _child;
		}

		private void DetachChild()
		{
			if (_child == null || TemplateRoot == null)
				return;

			TemplateRoot.Child = null;

			if (ScrollView != null)
				ScrollView.LogicalChild = _child;
		}

		protected sealed override Size MeasureOverride(Size availableSize)
		{
			var scrollViewPanel = ScrollViewPanel;

			if (scrollViewPanel == null)
				return new Size();

			scrollViewPanel.ActualViewport = availableSize;

			scrollViewPanel.Measure(XamlConstants.InfiniteSize);

			return scrollViewPanel.DesiredSize.Clamp(new Size(), availableSize);
		}

		protected virtual void OnScrollViewAttached()
		{
		}

		protected virtual void OnScrollViewDetached()
		{
		}

		protected virtual void OnTemplateAttached()
		{
		}

		protected virtual void OnTemplateDetached()
		{
		}

		protected sealed override void UndoTemplateOverride()
		{
			DetachChild();

			base.UndoTemplateOverride();

			OnTemplateDetached();
		}
	}
}