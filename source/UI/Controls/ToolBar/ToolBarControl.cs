// <copyright file="ToolBarControl.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Controls;
using Zaaml.Core;
using Zaaml.Core.Extensions;
using Zaaml.Core.Packed;
using Zaaml.Core.Runtime;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.Behaviors.Draggable;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.Runtime;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.PresentationCore.Utils;

namespace Zaaml.UI.Controls.ToolBar
{
	[TemplateContractType(typeof(ToolBarControlTemplateContract))]
	public class ToolBarControl : ToolBarControlBase<ToolBarControl, ToolBarItem, ToolBarItemCollection, ToolBarItemsPresenter, ToolBarItemsPanel>
	{
		public static readonly DependencyProperty BandProperty = DPM.Register<int, ToolBarControl>
			("Band", t => t.OnBandChanged);

		public static readonly DependencyProperty BandIndexProperty = DPM.Register<int, ToolBarControl>
			("BandIndex", t => t.OnBandIndexChanged);

		private static readonly DependencyPropertyKey ActualOrientationPropertyKey = DPM.RegisterReadOnly<Orientation, ToolBarControl>
			("ActualOrientation", Orientation.Horizontal, t => t.OnActualOrientationChanged);

		private static readonly DependencyPropertyKey TrayPropertyKey = DPM.RegisterReadOnly<ToolBarTray, ToolBarControl>
			("Tray", t => t.OnTrayChanged);

		public static readonly DependencyProperty LengthProperty = DPM.Register<double, ToolBarControl>
			("Length", double.PositiveInfinity, t => t.OnLengthChanged);

		public static readonly DependencyProperty DragHandleVisibilityProperty = DPM.Register<ElementVisibility, ToolBarControl>
			("DragHandleVisibility", ElementVisibility.Auto, t => t.OnDragHandleVisibilityChanged);

		private static readonly DependencyPropertyKey ActualDragHandleVisibilityPropertyKey = DPM.RegisterReadOnly<Visibility, ToolBarControl>
			("ActualDragHandleVisibility", Visibility.Collapsed);

		public static readonly DependencyProperty ActualDragHandleVisibilityProperty = ActualDragHandleVisibilityPropertyKey.DependencyProperty;
		public static readonly DependencyProperty TrayProperty = TrayPropertyKey.DependencyProperty;
		public static readonly DependencyProperty ActualOrientationProperty = ActualOrientationPropertyKey.DependencyProperty;

		private byte _packedValue;

		static ToolBarControl()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<ToolBarControl>();
		}

		public ToolBarControl()
		{
			this.OverrideStyleKey<ToolBarControl>();
		}

		public Visibility ActualDragHandleVisibility
		{
			get => (Visibility) GetValue(ActualDragHandleVisibilityProperty);
			private set => this.SetReadOnlyValue(ActualDragHandleVisibilityPropertyKey, value);
		}

		public Orientation ActualOrientation
		{
			get => (Orientation) GetValue(ActualOrientationProperty);
			private set => this.SetReadOnlyValue(ActualOrientationPropertyKey, value);
		}

		public int Band
		{
			get => (int) GetValue(BandProperty);
			set => SetValue(BandProperty, value);
		}

		public int BandIndex
		{
			get => (int) GetValue(BandIndexProperty);
			set => SetValue(BandIndexProperty, value);
		}

		internal DraggableBehavior DraggableBehavior { get; } = new DraggableBehavior {Advisor = DummyDraggableAdvisor.Instance};

		internal FrameworkElement DragHandle => TemplateContract.DragHandle;

		public ElementVisibility DragHandleVisibility
		{
			get => (ElementVisibility) GetValue(DragHandleVisibilityProperty);
			set => SetValue(DragHandleVisibilityProperty, value.Box());
		}

		internal bool IsMeasureToMaxLength
		{
			get => PackedDefinition.IsMeasureToMaxLength.GetValue(_packedValue);
			private set => PackedDefinition.IsMeasureToMaxLength.SetValue(ref _packedValue, value);
		}

		internal bool IsMeasureToMinLength
		{
			get => PackedDefinition.IsMeasureToMinLength.GetValue(_packedValue);
			private set => PackedDefinition.IsMeasureToMinLength.SetValue(ref _packedValue, value);
		}

		public bool IsMenuOpen
		{
			get => (bool) GetValue(IsMenuOpenProperty);
			set => SetValue(IsMenuOpenProperty, value.Box());
		}

		private ToolBarItemsPanel ItemsHost => ItemsPresenter?.ItemsHostInternal;

		public double Length
		{
			get => (double) GetValue(LengthProperty);
			set => SetValue(LengthProperty, value);
		}

		protected ToolBarOverflowItemsPresenter OverflowItemsPresenter => TemplateContract.OverflowItemsPresenter;

		private ToolBarControlTemplateContract TemplateContract => (ToolBarControlTemplateContract) TemplateContractCore;

		public ToolBarTray Tray
		{
			get => (ToolBarTray) GetValue(TrayProperty);
			internal set => this.SetReadOnlyValue(TrayPropertyKey, value);
		}

		private Size ApplyLength(Size size)
		{
			var orientedSize = size.AsOriented(ActualOrientation);

			orientedSize.Direct = orientedSize.Direct.Clamp(0, Length);

			return orientedSize.Size;
		}

		internal void CloseOverflow()
		{
			IsMenuOpen = false;
		}

		protected override ToolBarItemCollection CreateItemCollection()
		{
			return new ToolBarItemCollection(this);
		}

		private Size GetFinalSize(Size measureSize, Size availableSize)
		{
			var actualOrientation = ActualOrientation;

			if (actualOrientation == Orientation.Horizontal)
			{
				if (availableSize.Width.IsPositiveInfinity() == false && measureSize.Width < availableSize.Width)
					measureSize.Width = availableSize.Width;
			}
			else
			{
				if (availableSize.Height.IsPositiveInfinity() == false && measureSize.Height < availableSize.Height)
					measureSize.Height = availableSize.Height;
			}

			return measureSize;
		}

		private void InvalidateMeasureInt()
		{
			InvalidateMeasureUntilSelf();

			if (Tray != null)
				this.InvalidateAncestorsMeasure(Tray);
		}

		private void InvalidateMeasureUntilSelf()
		{
			InvalidateMeasure();

			var element = (FrameworkElement) ItemsPresenter?.ItemsHostBaseInternal ?? ItemsPresenter;

			element?.InvalidateAncestorsMeasure(this);
		}

		internal void MeasureMinMaxLength(OrientedSize childConstraint, out double minLength, out double maxLength)
		{
			IsMeasureToMinLength = true;

			InvalidateMeasureUntilSelf();

			Measure(childConstraint.Size);

			minLength = DesiredSize.AsOriented(childConstraint.Orientation).Direct;

			InvalidateMeasureUntilSelf();

			IsMeasureToMinLength = false;

			IsMeasureToMaxLength = true;

			Measure(childConstraint.Size);

			IsMeasureToMaxLength = false;

			maxLength = DesiredSize.AsOriented(childConstraint.Orientation).Direct;
		}

		protected override Size MeasureOverride(Size availableSize)
		{
			if (IsMeasureToMinLength)
			{
				if (ItemCollection.ActualCountInternal > 1)
				{
					HasOverflowItems = true;

					return base.MeasureOverride(availableSize);
				}
			}

			if (IsMeasureToMaxLength)
			{
				HasOverflowItems = false;

				return base.MeasureOverride(availableSize);
			}

			var hasOverflowedItems = HasOverflowItems;

			HasOverflowItems = false;

			availableSize = ApplyLength(availableSize);

			var measureOverride = base.MeasureOverride(availableSize);

			HasOverflowItems = ItemsHost?.HasOverflowChildren ?? false;

			if (HasOverflowItems == false && hasOverflowedItems == false)
				return ApplyLength(measureOverride);

			if (ItemsHost != null)
				PanelUtils.InvalidateAncestorsMeasure(ItemsHost, this);

			return GetFinalSize(ApplyLength(base.MeasureOverride(availableSize)), availableSize);
		}

		private void OnActualOrientationChanged()
		{
			InvalidateMeasureInt();
		}

		private void OnBandChanged()
		{
			Tray?.InvalidateBands();
		}

		private void OnBandIndexChanged()
		{
			Tray?.InvalidateBands();
		}

		private void OnDragHandleVisibilityChanged()
		{
			UpdateActualDragHandleVisibility();
		}

		internal override void OnItemAttachedInternal(ToolBarItem item)
		{
			item.ToolBar = this;

			base.OnItemAttachedInternal(item);
		}

		internal override void OnItemDetachedInternal(ToolBarItem item)
		{
			base.OnItemDetachedInternal(item);

			item.ToolBar = null;
		}

		private void OnLengthChanged()
		{
			InvalidateMeasureInt();
		}

		protected override void OnTemplateContractAttached()
		{
			ItemsPresenter.ToolBar = this;
			OverflowItemsPresenter.ToolBar = this;
			OverflowItemsPresenter.OverflowItems.SourceCollectionInternal = ItemCollection;

			DraggableBehavior.Handle = new DraggableElementHandle { Element = DragHandle };

			this.AddBehavior(DraggableBehavior);

			base.OnTemplateContractAttached();
		}

		protected override void OnTemplateContractDetaching()
		{
			base.OnTemplateContractDetaching();

			this.RemoveBehavior(DraggableBehavior);

			DraggableBehavior.Handle = null;

			OverflowItemsPresenter.OverflowItems.SourceCollectionInternal = null;
			OverflowItemsPresenter.ToolBar = null;
			ItemsPresenter.ToolBar = null;
		}

		private void OnTrayChanged()
		{
			UpdateActualOrientation();
			UpdateActualDragHandleVisibility();
		}

		internal void UpdateActualDragHandleVisibility()
		{
			ActualDragHandleVisibility = VisibilityUtils.EvaluateElementVisibility(DragHandleVisibility, Tray?.IsLocked == false ? Visibility.Visible : Visibility.Collapsed);
		}

		internal void UpdateActualOrientation()
		{
			ActualOrientation = Tray?.Orientation ?? Orientation.Horizontal;
		}

		private static class PackedDefinition
		{
			public static readonly PackedBoolItemDefinition IsMeasureToMinLength;
			public static readonly PackedBoolItemDefinition IsMeasureToMaxLength;

			static PackedDefinition()
			{
				var allocator = new PackedValueAllocator();

				IsMeasureToMinLength = allocator.AllocateBoolItem();
				IsMeasureToMaxLength = allocator.AllocateBoolItem();
			}
		}
	}

	public class ToolBarControlTemplateContract : ToolBarControlBaseTemplateContract<ToolBarControl, ToolBarItem, ToolBarItemCollection, ToolBarItemsPresenter, ToolBarItemsPanel>
	{
		[TemplateContractPart(Required = false)]
		public FrameworkElement DragHandle { get; [UsedImplicitly] private set; }

		[TemplateContractPart(Required = false)]
		public ToolBarOverflowItemsPresenter OverflowItemsPresenter { get; [UsedImplicitly] private set; }
	}
}