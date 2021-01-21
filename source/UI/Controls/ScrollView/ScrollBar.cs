// <copyright file="ScrollBar.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using System.Windows.Controls;
using Zaaml.Core;
using Zaaml.Core.Extensions;
using Zaaml.Core.Packed;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.CommandCore;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Controls.Primitives;
using Zaaml.UI.Controls.Primitives.TrackBar;

namespace Zaaml.UI.Controls.ScrollView
{
	[TemplateContractType(typeof(ScrollBarTemplateContract))]
	public class ScrollBar : ValueRangeControlBase
	{
		public static readonly DependencyProperty OrientationProperty = DPM.Register<Orientation, ScrollBar>
			("Orientation");

		public static readonly DependencyProperty ViewportSizeProperty = DPM.Register<double, ScrollBar>
			("ViewportSize", s => s.OnViewportSizeChanged);

		public static readonly DependencyProperty SmallChangeProperty = DPM.Register<double, ScrollBar>
			("SmallChange", 0.1);

		public static readonly DependencyProperty LargeChangeProperty = DPM.Register<double, ScrollBar>
			("LargeChange", 1.0);

		private byte _packedValue;

		static ScrollBar()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<ScrollBar>();
		}

		public ScrollBar()
		{
			this.OverrideStyleKey<ScrollBar>();

			CoerceValue(IsEnabledProperty);
		}

		internal bool CanScroll
		{
			get => PackedDefinition.CanScroll.GetValue(_packedValue);
			set
			{
				if (CanScroll == value)
					return;

				PackedDefinition.CanScroll.SetValue(ref _packedValue, value);

				CoerceValue(IsEnabledProperty);
			}
		}

		protected override bool IsEnabledCore => base.IsEnabledCore && CanScroll;

		internal bool IsInScrollViewer
		{
			get => PackedDefinition.IsInScrollViewer.GetValue(_packedValue);
			set => PackedDefinition.IsInScrollViewer.SetValue(ref _packedValue, value);
		}

		public double LargeChange
		{
			get => (double) GetValue(LargeChangeProperty);
			set => SetValue(LargeChangeProperty, value);
		}

		private RepeatButton LargeDecrease => TemplateContract.LargeDecrease;

		private RepeatButton LargeIncrease => TemplateContract.LargeIncrease;

		public Orientation Orientation
		{
			get => (Orientation) GetValue(OrientationProperty);
			set => SetValue(OrientationProperty, value);
		}

		private ScrollViewControlBase ScrollViewControl => IsInScrollViewer ? this.GetTemplatedParent<ScrollViewControlBase>() : null;

		public double SmallChange
		{
			get => (double) GetValue(SmallChangeProperty);
			set => SetValue(SmallChangeProperty, value);
		}

		private RepeatButton SmallDecrease => TemplateContract.SmallDecrease;

		private RepeatButton SmallIncrease => TemplateContract.SmallIncrease;

		internal ScrollBarTemplateContract TemplateContract => (ScrollBarTemplateContract) TemplateContractInternal;

		private ScrollBarThumb Thumb => TemplateContract.Thumb;

		private TrackBarControl TrackBar => TemplateContract.TrackBar;

		public double ViewportSize
		{
			get => (double) GetValue(ViewportSizeProperty);
			set => SetValue(ViewportSizeProperty, value);
		}

		private void ExecuteScrollViewerCommandHorizontal(ScrollViewControlBase scrollViewControl, ChangeCommandKind changeKind)
		{
			switch (changeKind)
			{
				case ChangeCommandKind.SmallDecrease:
					scrollViewControl.LineLeft();
					break;
				case ChangeCommandKind.LargeDecrease:
					scrollViewControl.PageLeft();
					break;
				case ChangeCommandKind.LargeIncrease:
					scrollViewControl.PageRight();
					break;
				case ChangeCommandKind.SmallIncrease:
					scrollViewControl.LineRight();
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(changeKind));
			}
		}

		private void ExecuteScrollViewerCommandVertical(ScrollViewControlBase scrollViewControl, ChangeCommandKind changeKind)
		{
			switch (changeKind)
			{
				case ChangeCommandKind.SmallDecrease:
					scrollViewControl.LineUp();
					break;
				case ChangeCommandKind.LargeDecrease:
					scrollViewControl.PageUp();
					break;
				case ChangeCommandKind.LargeIncrease:
					scrollViewControl.PageDown();
					break;
				case ChangeCommandKind.SmallIncrease:
					scrollViewControl.LineDown();
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(changeKind));
			}
		}

		private void ExecuteSelfChangeCommand(ChangeCommandKind changeKind)
		{
			var value = Value;

			switch (changeKind)
			{
				case ChangeCommandKind.SmallDecrease:
					value -= SmallChange;
					break;
				case ChangeCommandKind.LargeDecrease:
					value -= LargeChange;
					break;
				case ChangeCommandKind.LargeIncrease:
					value += LargeChange;
					break;
				case ChangeCommandKind.SmallIncrease:
					value += SmallChange;
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(changeKind));
			}

			value = value.Clamp(Minimum, Maximum);
			Value = value;
		}

		private bool OnCanExecuteChangeCommand(ChangeCommandKind changeKind)
		{
			//switch (changeKind)
			//{
			//  case ChangeCommandKind.SmallDecrease:
			//  case ChangeCommandKind.LargeDecrease:
			//    return Value > Minimum;
			//  case ChangeCommandKind.LargeIncrease:
			//  case ChangeCommandKind.SmallIncrease:
			//    return Value < Maximum;
			//  default:
			//    throw new ArgumentOutOfRangeException(nameof(changeKind));
			//}

			return true;
		}

		private void OnChangeCommand(ChangeCommandKind changeKind)
		{
			var scrollViewer = ScrollViewControl;

			if (scrollViewer == null)
				ExecuteSelfChangeCommand(changeKind);
			else
			{
				switch (Orientation)
				{
					case Orientation.Vertical:
						ExecuteScrollViewerCommandVertical(scrollViewer, changeKind);
						break;
					case Orientation.Horizontal:
						ExecuteScrollViewerCommandHorizontal(scrollViewer, changeKind);
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
		}

		protected override void OnMaximumChanged(double oldValue, double newValue)
		{
			base.OnMaximumChanged(oldValue, newValue);

			UpdateCanScroll();

			UpdateThumbSize();

			SyncThumb();
		}

		protected override void OnMinimumChanged(double oldValue, double newValue)
		{
			base.OnMinimumChanged(oldValue, newValue);

			UpdateCanScroll();

			UpdateThumbSize();

			SyncThumb();
		}

		protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
		{
			base.OnRenderSizeChanged(sizeInfo);

			UpdateThumbSize();
		}

		protected override void OnTemplateContractAttached()
		{
			base.OnTemplateContractAttached();

			TrackBar.SizeChanged += TrackBarOnSizeChanged;
			TrackBar.DragStarted += TrackBarOnDragStarted;
			TrackBar.DragEnded += TrackBarOnDragEnded;

			var changeCommand = new RelayCommand<ChangeCommandKind>(OnChangeCommand, OnCanExecuteChangeCommand);

			SmallIncrease.CommandParameter = ChangeCommandKind.SmallIncrease;
			SmallIncrease.Command = changeCommand;

			LargeIncrease.CommandParameter = ChangeCommandKind.LargeIncrease;
			LargeIncrease.Command = changeCommand;

			LargeDecrease.CommandParameter = ChangeCommandKind.LargeDecrease;
			LargeDecrease.Command = changeCommand;

			SmallDecrease.CommandParameter = ChangeCommandKind.SmallDecrease;
			SmallDecrease.Command = changeCommand;

			UpdateThumbSize();
		}

		protected override void OnTemplateContractDetaching()
		{
			TrackBar.DragEnded -= TrackBarOnDragEnded;
			TrackBar.DragStarted -= TrackBarOnDragStarted;
			TrackBar.SizeChanged -= TrackBarOnSizeChanged;

			SmallIncrease.Command = null;
			SmallIncrease.CommandParameter = null;

			LargeIncrease.Command = null;
			LargeIncrease.CommandParameter = null;

			LargeDecrease.Command = null;
			LargeDecrease.CommandParameter = null;

			SmallDecrease.Command = null;
			SmallDecrease.CommandParameter = null;

			base.OnTemplateContractDetaching();
		}

		protected override void OnValueChanged(double oldValue, double newValue)
		{
			base.OnValueChanged(oldValue, newValue);

			SyncThumb();
		}

		private void OnViewportSizeChanged()
		{
			UpdateThumbSize();
		}

		private void SyncThumb()
		{
			TrackBar?.SyncDragItem();
		}

		private void TrackBarOnDragEnded(object sender, TrackBarControlDragEventArgs e)
		{
			Thumb.IsPressed = false;

			ScrollViewControl?.OnScrollBarDragCompletedInternal(this);
		}

		private void TrackBarOnDragStarted(object sender, TrackBarControlDragEventArgs e)
		{
			Thumb.IsPressed = true;
		}

		private void TrackBarOnSizeChanged(object sender, SizeChangedEventArgs e)
		{
			UpdateThumbSize();
		}

		private void UpdateCanScroll()
		{
			CanScroll = (Maximum - Minimum).IsZero() == false;
		}

		private void UpdateThumbSize()
		{
			if (Thumb == null || TrackBar == null)
				return;

			var range = Maximum - Minimum;

			if (range.IsZero())
			{
				Thumb.ThumbSize = new Size();

				return;
			}

			var orientation = Orientation;
			var trackBarSize = TrackBar.GetActualSize().AsOriented(orientation).Direct;
			//var actualViewport = this.IsDefaultValue(ViewportSizeProperty) ? trackBarSize : ViewportSize;
			var actualViewport = ViewportSize;
			var coefficient = actualViewport > 0 ? actualViewport / (actualViewport + range) : 0;
			var thumbDirect = trackBarSize * coefficient;

			if (thumbDirect.IsNaN() || thumbDirect < 0)
				thumbDirect = 0;

			Thumb.ThumbSize = new OrientedSize(orientation, XamlConstants.ZeroSize).ChangeDirect(Math.Round(thumbDirect)).Size;

			Thumb.InvalidateAncestorsMeasure(TrackBar, true);
		}

		private enum ChangeCommandKind
		{
			SmallDecrease,
			LargeDecrease,
			LargeIncrease,
			SmallIncrease
		}

		private static class PackedDefinition
		{
			public static readonly PackedBoolItemDefinition IsInScrollViewer;
			public static readonly PackedBoolItemDefinition CanScroll;

			static PackedDefinition()
			{
				var allocator = new PackedValueAllocator();

				IsInScrollViewer = allocator.AllocateBoolItem();
				CanScroll = allocator.AllocateBoolItem();
			}
		}
	}

	public class ScrollBarTemplateContract : TemplateContract
	{
		[TemplateContractPart(Required = true)]
		public RepeatButton LargeDecrease { get; [UsedImplicitly] private set; }

		[TemplateContractPart(Required = true)]
		public RepeatButton LargeIncrease { get; [UsedImplicitly] private set; }

		[TemplateContractPart(Required = true)]
		public RepeatButton SmallDecrease { get; [UsedImplicitly] private set; }

		[TemplateContractPart(Required = true)]
		public RepeatButton SmallIncrease { get; [UsedImplicitly] private set; }

		[TemplateContractPart(Required = true)]
		public ScrollBarThumb Thumb { get; [UsedImplicitly] private set; }

		[TemplateContractPart(Required = true)]
		public TrackBarControl TrackBar { get; [UsedImplicitly] private set; }
	}
}