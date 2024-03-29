﻿// <copyright file="TrackBarControl.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using Zaaml.Core;
using Zaaml.Core.Packed;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.PresentationCore.Utils;

namespace Zaaml.UI.Controls.Primitives.TrackBar
{
	[ContentProperty(nameof(ItemCollection))]
	[TemplateContractType(typeof(TrackBarTemplateContract))]
	public class TrackBarControl : RangeControlBase
	{
		private static readonly DependencyPropertyKey ItemCollectionPropertyKey = DPM.RegisterReadOnly<TrackBarItemCollection, TrackBarControl>
			("ItemCollectionPrivate");

		public static readonly DependencyProperty ItemCollectionProperty = ItemCollectionPropertyKey.DependencyProperty;

		public static readonly DependencyProperty OrientationProperty = DPM.Register<Orientation, TrackBarControl>
			("Orientation", Orientation.Horizontal);

		private TrackBarItem _dragItem;
		private double _originPixelRatio;
		private Range<double> _originRange;

		private double _pixelDelta;
		private Point _trackBarOrigin;
		private Point _trackBarPosition;
		private double _valueDelta;

		public event EventHandler<TrackBarControlDragEventArgs> DragStarted;
		public event EventHandler<TrackBarControlDragEventArgs> DragEnded;

		static TrackBarControl()
		{
			ControlUtils.OverrideIsTabStop<TrackBarControl>(false);
			DefaultStyleKeyHelper.OverrideStyleKey<TrackBarControl>();
		}

		public TrackBarControl()
		{
			this.OverrideStyleKey<TrackBarControl>();
		}

		private bool CornersDirty
		{
			get => PackedDefinition.CornersDirty.GetValue(PackedValue);
			set => PackedDefinition.CornersDirty.SetValue(ref PackedValue, value);
		}

		private bool IsDragSyncValue
		{
			get => PackedDefinition.IsDragSyncValue.GetValue(PackedValue);
			set => PackedDefinition.IsDragSyncValue.SetValue(ref PackedValue, value);
		}

		public TrackBarItemCollection ItemCollection => this.GetValueOrCreate(ItemCollectionPropertyKey, () => new TrackBarItemCollection(this));

		public Orientation Orientation
		{
			get => (Orientation)GetValue(OrientationProperty);
			set => SetValue(OrientationProperty, value);
		}

		private double PixelRatio => TrackBarPanel?.PixelRatio ?? 0.0;

		private TrackBarTemplateContract TemplateContract => (TrackBarTemplateContract)TemplateContractCore;

		private TrackBarPanel TrackBarPanel => TemplateContract.TrackBarPanel;

		private void AssignIndices()
		{
			for (var index = 0; index < ItemCollection.Count; index++)
				ItemCollection[index].Index = index;
		}

		private void Clamp()
		{
			if (Initializing || Updating)
				return;

			TrackBarValueItem prev = null;

			foreach (var item in ItemCollection)
			{
				item.Clamp();

				if (item is TrackBarValueItem valueItem)
				{
					ClampRange(prev, valueItem);
					prev = valueItem;
				}
			}

			ClampRange(prev, null);
		}

		private void ClampRange(TrackBarValueItem first, TrackBarValueItem second)
		{
			if (ItemCollection.Count == 0)
				return;

			if (ItemCollection.Count == 1 && ItemCollection[0] is TrackBarValueItem)
				return;

			var minimum = first?.Value ?? Minimum;
			var maximum = second?.Value ?? Maximum;
			var range = maximum - minimum;
			var startIndex = first != null ? first.Index + 1 : 0;
			var endIndex = second != null ? second.Index - 1 : ItemCollection.Count - 1;

			while (startIndex < endIndex && ItemCollection[startIndex] is TrackBarValueItem)
				startIndex++;

			while (endIndex > startIndex && ItemCollection[endIndex] is TrackBarValueItem)
				endIndex--;

			var rangeItemCount = endIndex - startIndex + 1;

			if (rangeItemCount == 0)
				return;

			var rangeValue = range / rangeItemCount;

			for (var i = startIndex; i <= endIndex; i++)
			{
				var rangeItem = (TrackBarRangeItem)ItemCollection[i];

				rangeItem.Range = rangeValue;
			}
		}

		private void DragSyncValue(TrackBarValueItem valueItem, double value)
		{
			try
			{
				IsDragSyncValue = true;

				valueItem.SetValueInternal(value);
			}
			finally
			{
				IsDragSyncValue = false;
			}
		}

		protected override void EndInitCore()
		{
			AssignIndices();
			Clamp();
		}

		protected override void EndUpdateCore()
		{
			Clamp();

			InvalidatePanel();

			base.EndUpdateCore();
		}

		private void FinishDrag()
		{
			if (_dragItem == null)
				return;

			var dragItem = _dragItem;

			_dragItem = null;

			ReleaseMouseCapture();

			OnDragEndedPrivate(dragItem);
		}

		private void InvalidateCornerRadius()
		{
			CornersDirty = true;

			InvalidateMeasure();
		}

		private void InvalidatePanel()
		{
			TrackBarPanel?.InvalidateAncestorsMeasure(this, true);
		}

		protected override Size MeasureOverride(Size availableSize)
		{
			if (CornersDirty)
			{
				CornersDirty = false;

				UpdateCornerRadius();
			}

			return base.MeasureOverride(availableSize);
		}


		protected override void OnCornerRadiusChanged(CornerRadius oldValue, CornerRadius newValue)
		{
			base.OnCornerRadiusChanged(oldValue, newValue);

			InvalidateCornerRadius();
		}

		protected virtual void OnDragEnded(TrackBarItem item)
		{
		}

		private void OnDragEndedPrivate(TrackBarItem item)
		{
			OnDragEnded(item);

			DragEnded?.Invoke(this, new TrackBarControlDragEventArgs(item));

			item.OnDragEndedInternal();
		}

		protected virtual void OnDragStarted(TrackBarItem item)
		{
		}

		private void OnDragStartedPrivate(TrackBarItem item)
		{
			OnDragStarted(item);

			DragStarted?.Invoke(this, new TrackBarControlDragEventArgs(item));

			item.OnDragStartedInternal();
		}

		internal void OnItemAddedInternal(TrackBarItem item)
		{
			FinishDrag();

			item.TrackBarControl = this;

			if (Initializing == false)
			{
				AssignIndices();
				item.Clamp();
				Clamp();
			}

			TrackBarPanel?.Children.Add(item);

			InvalidateCornerRadius();
		}

		internal void OnItemMouseLeftButtonDown(TrackBarItem item, MouseButtonEventArgs e)
		{
			if (e.Handled || item.CanDrag == false)
				return;

			_dragItem = CaptureMouse() ? item : null;

			if (_dragItem == null)
				return;

			_trackBarOrigin = e.GetPosition(this);
			_originPixelRatio = PixelRatio;
			_trackBarPosition = _trackBarOrigin;
			_pixelDelta = 0;
			_valueDelta = 0;

			if (item is TrackBarValueItem valueItem)
			{
				_originRange = new Range<double>(valueItem.Value, valueItem.Value);

				OnDragStartedPrivate(_dragItem);

				return;
			}

			var rangeItem = (TrackBarRangeItem)item;

			_originRange = new Range<double>(rangeItem.PrevValueItem?.Value ?? Minimum, rangeItem.NextValueItem?.Value ?? Maximum);

			OnDragStartedPrivate(_dragItem);
		}

		internal void OnItemRemovedInternal(TrackBarItem item)
		{
			FinishDrag();

			TrackBarPanel?.Children.Remove(item);
			item.TrackBarControl = null;

			if (Initializing == false)
			{
				AssignIndices();
				Clamp();
			}

			InvalidateCornerRadius();
		}

		protected override void OnLostMouseCapture(MouseEventArgs e)
		{
			base.OnLostMouseCapture(e);

			FinishDrag();
		}

		protected override void OnMaximumChanged(double oldValue, double newValue)
		{
			base.OnMaximumChanged(oldValue, newValue);

			Clamp();

			InvalidatePanel();
		}

		protected override void OnMinimumChanged(double oldValue, double newValue)
		{
			base.OnMinimumChanged(oldValue, newValue);

			Clamp();

			InvalidatePanel();
		}

		protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
		{
			base.OnMouseLeftButtonUp(e);

			if (_dragItem == null)
				return;

			UpdateValueOnMouseEvent(e);

			FinishDrag();
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);

			if (_dragItem == null)
				return;

			UpdateValueOnMouseEvent(e);
		}

		protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
		{
			if (Focusable)
				Focus();

			base.OnPreviewMouseLeftButtonDown(e);
		}

		protected override void OnTemplateContractAttached()
		{
			base.OnTemplateContractAttached();

			TrackBarPanel.TrackBar = this;

			TrackBarPanel.Children.AddRange(ItemCollection);
		}

		protected override void OnTemplateContractDetaching()
		{
			TrackBarPanel.TrackBar = null;

			TrackBarPanel.Children.Clear();

			base.OnTemplateContractDetaching();
		}

		internal void OnTrackBarItemValueChanged(TrackBarValueItem valueItem)
		{
			if (Initializing || Updating)
				return;

			valueItem.Clamp();

			ClampRange(valueItem.PrevValueItem, valueItem);
			ClampRange(valueItem, valueItem.NextValueItem);

			InvalidatePanel();
		}

		internal void SyncDragItem()
		{
			if (IsDragSyncValue)
				return;

			if (_dragItem is not TrackBarValueItem valueItem)
				return;

			_trackBarOrigin = _trackBarPosition;
			_pixelDelta = 0;
			_valueDelta = 0;
			_originPixelRatio = PixelRatio;
			_originRange = new Range<double>(valueItem.Value, valueItem.Value);
		}

		private void UpdateCornerRadius()
		{
			var enabledCorners = CornerRadius;
			var disabledCorners = new CornerRadius(0);

			TrackBarRangeItem first = null;
			TrackBarRangeItem last = null;

			foreach (var trackBarRangeItem in ItemCollection.OfType<TrackBarRangeItem>())
			{
				first ??= trackBarRangeItem;
				last = trackBarRangeItem;
			}

			if (first != null)
				first.ActualCornerRadius = CornerRadiusUtils.Compose(enabledCorners, disabledCorners, false, MaskCornerRadiusFlags.TopLeft | MaskCornerRadiusFlags.BottomLeft);

			if (last != null && ReferenceEquals(first, last) == false)
				last.ActualCornerRadius = CornerRadiusUtils.Compose(enabledCorners, disabledCorners, false, MaskCornerRadiusFlags.TopRight | MaskCornerRadiusFlags.BottomRight);
		}

		private void UpdateValueOnMouseEvent(MouseEventArgs e)
		{
			if (_dragItem == null)
				return;

			var pixelRatio = PixelRatio;

			if (pixelRatio.IsZero())
				return;

			_trackBarPosition = e.GetPosition(this);

			var orientation = Orientation;
			var currentPosition = _trackBarPosition.AsOriented(orientation).Direct;
			var originPosition = _trackBarOrigin.AsOriented(orientation).Direct;

			_pixelDelta = currentPosition - originPosition;
			_valueDelta = _pixelDelta / pixelRatio;

			if (_dragItem is TrackBarValueItem valueItem)
			{
				DragSyncValue(valueItem, _originRange.Minimum + _valueDelta);

				return;
			}

			var rangeItem = (TrackBarRangeItem)_dragItem;
			var prevValueItem = rangeItem.PrevValueItem;
			var nextValueItem = rangeItem.NextValueItem;

			if (prevValueItem == null || nextValueItem == null)
				return;

			if (_valueDelta < 0)
			{
				var minimumValue = prevValueItem.PrevValueItem?.Value ?? Minimum;
				var newValue = _originRange.Minimum + _valueDelta;

				if (newValue < minimumValue)
					_valueDelta = minimumValue - _originRange.Minimum;

				prevValueItem.SetValueInternal(_originRange.Minimum + _valueDelta);
				nextValueItem.SetValueInternal(_originRange.Maximum + _valueDelta);
			}
			else if (_valueDelta > 0)
			{
				var maximumValue = nextValueItem.NextValueItem?.Value ?? Maximum;
				var newValue = _originRange.Maximum + _valueDelta;

				if (newValue > maximumValue)
					_valueDelta = maximumValue - _originRange.Maximum;

				prevValueItem.SetValueInternal(_originRange.Minimum + _valueDelta);
				nextValueItem.SetValueInternal(_originRange.Maximum + _valueDelta);
			}
		}

		private static class PackedDefinition
		{
			public static readonly PackedBoolItemDefinition IsDragSyncValue;
			public static readonly PackedBoolItemDefinition CornersDirty;

			static PackedDefinition()
			{
				var allocator = GetAllocator<TrackBarControl>();

				IsDragSyncValue = allocator.AllocateBoolItem();
				CornersDirty = allocator.AllocateBoolItem();
			}
		}
	}

	public class TrackBarTemplateContract : TemplateContract
	{
		[TemplateContractPart(Required = true)]
		public TrackBarPanel TrackBarPanel { get; [UsedImplicitly] private set; }
	}
}