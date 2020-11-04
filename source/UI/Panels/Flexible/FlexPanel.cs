// <copyright file="FlexPanel.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using System.Windows.Controls;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.UI.Panels.Flexible;
using Zaaml.UI.Panels.Interfaces;
using Panel = Zaaml.UI.Panels.Core.Panel;

// ReSharper disable once CheckNamespace

namespace Zaaml.UI.Panels
{
	public class FlexPanel : Panel, IFlexPanel, IFlexPanelEx
	{
		public static readonly DependencyProperty DistributorProperty = DPM.Register<IFlexDistributor, FlexPanel>
			("Distributor", FlexDistributor.Equalizer, p => p.InvalidateMeasure);

		public static readonly DependencyProperty OrientationProperty = DPM.Register<Orientation, FlexPanel>
			("Orientation", Orientation.Vertical, p => p.OnOrientationChanged);

		public static readonly DependencyProperty SpacingProperty = DPM.Register<double, FlexPanel>
			("Spacing", 0.0, p => p.InvalidateMeasure);

		public static readonly DependencyProperty StretchProperty = DPM.Register<FlexStretch, FlexPanel>
			("Stretch", FlexStretch.None, p => p.OnStretchChanged);

		private static readonly DependencyPropertyKey HasHiddenChildrenPropertyKey = DPM.RegisterReadOnly<bool, FlexPanel>
			("HasHiddenChildren", false, f => f.OnHasHiddenChildrenChanged);

		public static readonly DependencyProperty HasHiddenChildrenProperty = HasHiddenChildrenPropertyKey.DependencyProperty;

		private static readonly DependencyPropertyKey IsHiddenPropertyKey = DPM.RegisterAttachedReadOnly<bool, FlexPanel>
			("IsHidden", false, OnIsHiddenChanged);

		public static readonly DependencyProperty GenericDefinitionProperty = DPM.Register<FlexDefinition, FlexPanel>
			("GenericDefinition", p => p.OnGenericDefinitionChanged);

		public static readonly DependencyProperty DefinitionSelectorProperty = DPM.Register<FlexDefinitionSelector, FlexPanel>
			("DefinitionSelector", p => p.OnDefinitionSelectorChanged);

		public static readonly DependencyProperty IsHiddenProperty = IsHiddenPropertyKey.DependencyProperty;

		private int _arrangePassVersion;
		private bool _isInArrange;
		private FlexPanelLayout _layout;
		private int _measurePassVersion;
		private bool _observeLayout;
		public event EventHandler HasHiddenChildrenChanged;

		static FlexPanel()
		{
#if !SILVERLIGHT
			DefaultStyleKeyProperty.OverrideMetadata(typeof(FlexPanel), new FrameworkPropertyMetadata(typeof(FlexPanel)));
#endif
		}

		public FlexDefinitionSelector DefinitionSelector
		{
			get => (FlexDefinitionSelector) GetValue(DefinitionSelectorProperty);
			set => SetValue(DefinitionSelectorProperty, value);
		}

		public FlexDefinition GenericDefinition
		{
			get => (FlexDefinition) GetValue(GenericDefinitionProperty);
			set => SetValue(GenericDefinitionProperty, value);
		}

		public bool HasHiddenChildren
		{
			get => (bool) GetValue(HasHiddenChildrenProperty);
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
			try
			{
				_isInArrange = true;

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
			finally
			{
				_isInArrange = false;
			}
		}

		private FlexPanelLayout CreateLayout()
		{
			return new FlexPanelLayout(this);
		}

		protected virtual FlexElement GetFlexElement(UIElement child)
		{
			return child.GetFlexElement(this, DefinitionSelector?.Select(this, child) ?? GenericDefinition);
		}

		public static bool GetIsHidden(DependencyObject element)
		{
			return (bool) element.GetValue(IsHiddenPropertyKey.DependencyProperty);
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

		protected override void OnChildDesiredSizeChanged(UIElement child)
		{
			if (_isInArrange)
				return;

			base.OnChildDesiredSizeChanged(child);
		}

		private void OnDefinitionChanged(object sender, EventArgs eventArgs)
		{
			InvalidateMeasure();
		}

		private void OnDefinitionSelectorChanged(FlexDefinitionSelector oldSelector, FlexDefinitionSelector newSelector)
		{
			if (oldSelector != null)
				oldSelector.SelectorChanged -= OnSelectorChanged;

			if (newSelector != null)
				newSelector.SelectorChanged += OnSelectorChanged;

			InvalidateMeasure();
		}

		private void OnGenericDefinitionChanged(FlexDefinition oldDefinition, FlexDefinition newDefinition)
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
			get => (IFlexDistributor) GetValue(DistributorProperty);
			set => SetValue(DistributorProperty, value);
		}

		public FlexStretch Stretch
		{
			get => (FlexStretch) GetValue(StretchProperty);
			set => SetValue(StretchProperty, value);
		}

		public double Spacing
		{
			get => (double) GetValue(SpacingProperty);
			set => SetValue(SpacingProperty, value);
		}

		public Orientation Orientation
		{
			get => (Orientation) GetValue(OrientationProperty);
			set => SetValue(OrientationProperty, value);
		}

		bool IFlexPanelEx.AllowMeasureInArrange => true;
	}
}