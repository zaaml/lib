// <copyright file="SharedSizeContentPanel.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.Utils;

namespace Zaaml.UI.Controls.Primitives.SharedSizePrimitives
{
	public sealed class SharedSizeContentPanel : Panel
	{
		private SharedSizeEntry _actualSharedSizeEntry;
		private FrameworkElement _child;
		private SharedSizeContentControl _sharedSizeContentControl;

		private SharedSizeEntry ActualSharedSizeEntry
		{
			get { return ActualSharedSizeEntry = EnsureActualSharedSize(); }
			set
			{
				if (ReferenceEquals(_actualSharedSizeEntry, value))
					return;

				_actualSharedSizeEntry?.UnregisterSharedPanel(this);

				_actualSharedSizeEntry = value;

				_actualSharedSizeEntry?.RegisterSharedPanel(this);
			}
		}

		private FrameworkElement Child
		{
			set
			{
				if (ReferenceEquals(_child, value))
					return;

				if (_child != null)
				{
					if (value == null)
						Children.Clear();
				}

				_child = value;

				if (_child != null)
				{
					if (Children.Count == 0)
						Children.Add(_child);
					else
						Children[0] = _child;
				}

				InvalidateMeasure();
			}
		}

		internal SharedSizeContentControl SharedSizeContentControl
		{
			get => _sharedSizeContentControl;
			set
			{
				if (ReferenceEquals(_sharedSizeContentControl, value))
					return;

				_sharedSizeContentControl = value;

				UpdateChild();
				InvalidateMeasure();
			}
		}

		protected override Size ArrangeOverride(Size finalSize)
		{
			_child?.Arrange(finalSize.Rect());

			return finalSize;
		}

		private static void CalcSize(ref Size actualShareSize, ref Size measure, bool shareWidth, bool shareHeight)
		{
			if (shareWidth)
				actualShareSize.Width = measure.Width = Math.Max(measure.Width, actualShareSize.Width);

			if (shareHeight)
				actualShareSize.Height = measure.Height = Math.Max(measure.Height, actualShareSize.Height);
		}

		private ContentPresenter CreateContentPresenter(object content)
		{
			var contentPresenter = new ContentPresenter {Content = content};

			if (SharedSizeContentControl != null)
			{
				contentPresenter.SetBinding(VerticalAlignmentProperty, new Binding {Path = new PropertyPath(Control.VerticalContentAlignmentProperty), Source = SharedSizeContentControl});
				contentPresenter.SetBinding(HorizontalAlignmentProperty, new Binding {Path = new PropertyPath(Control.HorizontalContentAlignmentProperty), Source = SharedSizeContentControl});
			}

			return contentPresenter;
		}

		private SharedSizeEntry EnsureActualSharedSize()
		{
			var key = SharedSizeContentControl?.SharedSizeKey;

			if (key == null)
				return null;

			// ReSharper disable once LoopCanBeConvertedToQuery
			foreach (var ancestor in this.GetAncestors(VisualTreeEnumerationStrategy.Instance).OfType<SharedSizeGroupControl>().Where(s => s.IsSharingEnabled))
			{
				var entry = ancestor.GetSharedSize(key);
				if (entry != null)
					return entry;
			}

			return null;
		}

		private Size MeasureChild(Size availableSize)
		{
			if (_child == null)
				return XamlConstants.ZeroSize;

			_child.Measure(availableSize);

			return _child.DesiredSize;
		}

		protected override Size MeasureOverride(Size availableSize)
		{
			if (SharedSizeContentControl == null)
				return MeasureChild(availableSize);

			var measure = MeasureChild(availableSize);
			var sharedSize = ActualSharedSizeEntry;

			if (sharedSize == null)
				return measure;

			var shareHeight = SharedSizeContentControl.ShareHeight;
			var shareWidth = SharedSizeContentControl.ShareWidth;

			var size = sharedSize.ActualSize;

			CalcSize(ref size, ref measure, shareWidth, shareHeight);

			if (sharedSize.IsInMeasurePass)
				sharedSize.ActualSize = size;
			else
				sharedSize.InvalidateGroup();

			return measure;
		}

		internal void OnContentChanged()
		{
			UpdateChild();
		}

		private void UpdateChild()
		{
			Child = _sharedSizeContentControl != null ? WrapContent(_sharedSizeContentControl.Content) : null;
		}

		private FrameworkElement WrapContent(object content)
		{
			if (content == null)
				return null;

			return content as FrameworkElement ?? CreateContentPresenter(content);
		}
	}
}