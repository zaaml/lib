// <copyright file="FlexItemsPanelBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using System.Windows.Controls;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.UI.Panels.Core;
using Zaaml.UI.Panels.Interfaces;
using Panel = Zaaml.UI.Panels.Core.Panel;

namespace Zaaml.UI.Panels.Flexible
{
	public class FlexItemsPanelBase<TItem> : ItemsPanel<TItem>, IFlexPanel
		where TItem : Control
	{
		public static readonly DependencyProperty DistributorProperty = DPM.Register<IFlexDistributor, FlexItemsPanelBase<TItem>>
			("Distributor", FlexDistributor.Equalizer, p => p.InvalidateMeasure);

		public static readonly DependencyProperty OrientationProperty = DPM.Register<Orientation, FlexItemsPanelBase<TItem>>
			("Orientation", Orientation.Vertical, p => p.OnOrientationChanged);

		public static readonly DependencyProperty SpacingProperty = DPM.Register<double, FlexItemsPanelBase<TItem>>
			("Spacing", 0.0, p => p.InvalidateMeasure);

		public static readonly DependencyProperty StretchProperty = DPM.Register<FlexStretch, FlexItemsPanelBase<TItem>>
			("Stretch", FlexStretch.None, p => p.OnStretchChanged);

		private static readonly DependencyPropertyKey HasHiddenChildrenPropertyKey = DPM.RegisterReadOnly<bool, FlexItemsPanelBase<TItem>>
			("HasHiddenChildren", false, f => f.OnHasHiddenChildrenChanged);

		public static readonly DependencyProperty HasHiddenChildrenProperty = HasHiddenChildrenPropertyKey.DependencyProperty;

		private static readonly DependencyPropertyKey IsHiddenPropertyKey = DPM.RegisterAttachedReadOnly<bool, FlexItemsPanelBase<TItem>>
			("IsHidden", false, OnIsHiddenChanged);

		public static readonly DependencyProperty FlexDefinitionProperty = DPM.Register<FlexDefinition, FlexItemsPanelBase<TItem>>
			("FlexDefinition", p => p.OnFlexDefinitionChanged);

		public static readonly DependencyProperty DefinitionSelectorProperty = DPM.Register<FlexDefinitionSelector, FlexItemsPanelBase<TItem>>
			("DefinitionSelector", p => p.OnDefinitionSelectorChanged);

		public static readonly DependencyProperty IsHiddenProperty = IsHiddenPropertyKey.DependencyProperty;

		private int _arrangePassVersion;
		private FlexPanelLayout _layout;
		private int _measurePassVersion;
		private bool _observeLayout;
		public event EventHandler HasHiddenChildrenChanged;

		static FlexItemsPanelBase()
		{
#if !SILVERLIGHT
			DefaultStyleKeyProperty.OverrideMetadata(typeof(FlexItemsPanelBase<TItem>), new FrameworkPropertyMetadata(typeof(FlexItemsPanelBase<TItem>)));
#endif
		}

		public FlexDefinitionSelector DefinitionSelector
		{
			get => (FlexDefinitionSelector)GetValue(DefinitionSelectorProperty);
			set => SetValue(DefinitionSelectorProperty, value);
		}

		public FlexDefinition FlexDefinition
		{
			get => (FlexDefinition)GetValue(FlexDefinitionProperty);
			set => SetValue(FlexDefinitionProperty, value);
		}

		public bool HasHiddenChildren
		{
			get => (bool)GetValue(HasHiddenChildrenProperty);
			private set => this.SetReadOnlyValue(HasHiddenChildrenPropertyKey, value);
		}

		protected override bool HasLogicalOrientation => true;

		internal FlexPanelLayout Layout => _layout ??= CreateLayout();

		protected override Orientation LogicalOrientation => Orientation;

		public bool ObserveLayoutUpdate
		{
			get => _observeLayout;
			set
			{
				if (_observeLayout == value)
					return;

				_observeLayout = value;

				if (_observeLayout)
					LayoutUpdated += OnLayoutUpdated;
				else
					LayoutUpdated -= OnLayoutUpdated;

				Layout.OnLayoutUpdated();
			}
		}

		protected override Size ArrangeOverrideCore(Size finalSize)
		{
			var arrangePass = ParentLayoutPass.CreateArrangePass(finalSize);

			arrangePass.LayoutPassVersion = _arrangePassVersion++;

			for (var arrangePassIndex = 0; arrangePassIndex < 8; arrangePassIndex++)
			{
				foreach (var child in Children)
				{
					var parentLayoutListener = child as IParentLayoutListener;

					parentLayoutListener?.EnterParentArrangePass(arrangePass);
					arrangePass.ChildIndex++;
				}

				arrangePass.ActualSize = Layout.Arrange(finalSize);
				arrangePass.ChildIndex = 0;

				foreach (var child in Children)
				{
					var parentLayoutListener = child as IParentLayoutListener;

					parentLayoutListener?.LeaveParentArrangePass(ref arrangePass);
					arrangePass.ChildIndex++;
				}

				if (arrangePass.IsArrangePassDirty)
					continue;

				return arrangePass.ActualSize;
			}

			return arrangePass.ActualSize;
		}

		private protected virtual FlexPanelLayout CreateLayout()
		{
			return new FlexPanelLayout(this);
		}

		protected virtual FlexElement GetFlexElement(UIElement child)
		{
			return child.GetFlexElement(this, Orientation, DefinitionSelector?.Select(this, child) ?? FlexDefinition);
		}

		public static bool GetIsHidden(DependencyObject element)
		{
			return (bool)element.GetValue(IsHiddenPropertyKey.DependencyProperty);
		}

		protected override Size MeasureOverrideCore(Size availableSize)
		{
			var measurePass = ParentLayoutPass.CreateMeasurePass(availableSize);

			measurePass.LayoutPassVersion = _measurePassVersion++;

			for (var measurePassIndex = 0; measurePassIndex < 8; measurePassIndex++)
			{
				foreach (var child in Children)
				{
					var parentLayoutListener = child as IParentLayoutListener;

					parentLayoutListener?.EnterParentMeasurePass(measurePass);
					measurePass.ChildIndex++;
				}

				measurePass.DesiredSize = Layout.Measure(availableSize);
				measurePass.ChildIndex = 0;

				foreach (var child in Children)
				{
					var parentLayoutListener = child as IParentLayoutListener;

					parentLayoutListener?.LeaveParentMeasurePass(ref measurePass);
					measurePass.ChildIndex++;
				}

				if (measurePass.IsMeasurePassDirty)
					continue;

				return measurePass.DesiredSize;
			}

			return measurePass.DesiredSize;
		}

		private void OnDefinitionChanged(object sender, EventArgs eventArgs)
		{
			InvalidateMeasure();

			//FlexPanelLayout.InvalidateFlexMeasure(this);
		}

		private void OnDefinitionSelectorChanged(FlexDefinitionSelector oldSelector, FlexDefinitionSelector newSelector)
		{
			if (oldSelector != null)
				oldSelector.SelectorChanged -= OnSelectorChanged;

			if (newSelector != null)
				newSelector.SelectorChanged += OnSelectorChanged;

			InvalidateMeasure();
		}

		private void OnFlexDefinitionChanged(FlexDefinition oldDefinition, FlexDefinition newDefinition)
		{
			if (oldDefinition != null)
				oldDefinition.DefinitionChanged -= OnDefinitionChanged;

			if (newDefinition != null)
				newDefinition.DefinitionChanged += OnDefinitionChanged;

			InvalidateMeasure();
		}

		private void OnHasHiddenChildrenChanged()
		{
			HasHiddenChildrenChanged?.Invoke(this, EventArgs.Empty);
		}

		private static void OnIsHiddenChanged(DependencyObject obj)
		{
		}

		private void OnLayoutUpdated(object sender, EventArgs eventArgs)
		{
			if (IsLoaded == false)
				return;

			Layout.OnLayoutUpdated();
		}

		private void OnOrientationChanged()
		{
			InvalidateMeasure();
			InvalidateArrange();
		}

		private void OnSelectorChanged(object sender, EventArgs eventArgs)
		{
			InvalidateMeasure();
		}

		private void OnStretchChanged()
		{
			InvalidateMeasure();
		}

		internal static void SetIsHidden(DependencyObject element, bool value)
		{
			element.SetValue(IsHiddenPropertyKey, value);
		}

		void IFlexPanel.SetIsHidden(UIElement child, bool value)
		{
			SetIsHidden(child, value);
		}

		bool IFlexPanel.HasHiddenChildren
		{
			get => HasHiddenChildren;
			set => HasHiddenChildren = value;
		}

		FlexElement IFlexPanel.GetFlexElement(UIElement child)
		{
			return GetFlexElement(child);
		}

		bool IFlexPanel.GetIsHidden(UIElement child)
		{
			return GetIsHidden(child);
		}

		public IFlexDistributor Distributor
		{
			get => (IFlexDistributor)GetValue(DistributorProperty);
			set => SetValue(DistributorProperty, value);
		}

		public FlexStretch Stretch
		{
			get => (FlexStretch)GetValue(StretchProperty);
			set => SetValue(StretchProperty, value);
		}

		public double Spacing
		{
			get => (double)GetValue(SpacingProperty);
			set => SetValue(SpacingProperty, value);
		}

		public Orientation Orientation
		{
			get => (Orientation)GetValue(OrientationProperty);
			set => SetValue(OrientationProperty, value);
		}
	}
}