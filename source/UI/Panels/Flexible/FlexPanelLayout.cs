// <copyright file="FlexPanelLayout.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Zaaml.Core.Extensions;
using Zaaml.Core.Utils;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.Extensions;
using Zaaml.UI.Panels.Core;
using Zaaml.UI.Panels.Interfaces;

namespace Zaaml.UI.Panels.Flexible
{
	internal class FlexPanelLayout : PanelLayoutBase<IFlexPanel>
	{
		private static readonly ArrayPool<Visibility> VisibilityArrayPool = new();

		internal readonly FlexElementCollection FlexElements = new();
		private bool _measureInfinite;

		public FlexPanelLayout(IFlexPanel panel) : base(panel)
		{
		}

		private protected virtual bool AllowMeasureInArrange => false;

		protected virtual PanelMeasureMode MeasureMode => PanelMeasureMode.Desired;

		private protected virtual bool? ShouldFillIndirect => null;

		protected override Size ArrangeCore(Size finalSize)
		{
			var arrangeResult = ArrangeCoreImpl(finalSize);

			return arrangeResult;
		}

		private Size ArrangeCoreImpl(Size finalSize)
		{
			var flexPanel = Panel;
			var allowMeasure = AllowMeasureInArrange;
			var orientation = flexPanel.Orientation;
			var useLayoutRounding = flexPanel.UseLayoutRounding;
			var spacing = GetRoundSpacing(flexPanel.Spacing, useLayoutRounding);

			for (var index = 0; index < flexPanel.Elements.Count; index++)
				FlexElements[index] = FlexElements[index].WithUIElement(flexPanel.Elements[index], orientation);

			var currentFlexElements = FlexElementCollection.Mount(FlexElements.Capacity);

			try
			{
				FlexElements.CopyTo(currentFlexElements);

				while (true)
				{
					var nextArrangePass = false;
					var size = new OrientedSize(orientation);
					var spacingOffset = 0.0;
					var finalOriented = finalSize.AsOriented(orientation);
					var finalIndirect = finalOriented.Indirect;
					var currentPoint = new OrientedPoint(orientation);
					var childFinalOriented = new OrientedSize(orientation);
					// Stretch

					Stretch(currentFlexElements, CalcSpacingDelta(GetVisibleCount(Panel, true), spacing), finalOriented.Direct);

					for (var index = 0; index < flexPanel.Elements.Count; index++)
					{
						var child = flexPanel.Elements[index];
						var flexElement = currentFlexElements[index];

						if (child.Visibility == Visibility.Collapsed)
							continue;

						if (flexPanel.GetIsHidden(child))
						{
							child.Arrange(XamlConstants.ZeroRect);
							flexElement.ActualLength = 0.0;
							currentFlexElements[index] = flexElement;

							continue;
						}

						var desiredOriented = child.DesiredSize.AsOriented(orientation);

						childFinalOriented.Direct = flexElement.ActualLength;
						childFinalOriented.Indirect = Math.Max(finalIndirect, desiredOriented.Indirect);

						// Arrange Child
						var rect = new Rect(XamlConstants.ZeroPoint, childFinalOriented.Size).WithOffset(currentPoint);

						if (useLayoutRounding)
							rect = rect.LayoutRound(RoundingMode.MidPointFromZero);

						if (_measureInfinite && allowMeasure && desiredOriented.Direct.IsGreaterThan(childFinalOriented.Direct))
						{
							var remeasureOriented = desiredOriented;

							remeasureOriented.ChangeDirect(childFinalOriented.Direct);

							child.Measure(remeasureOriented.Size);
						}

						child.Arrange(rect);

						var arrangeSize = GetActualArrangeSize(child);

						if (arrangeSize.IsEmpty == false)
						{
							rect.Width = arrangeSize.Width;
							rect.Height = arrangeSize.Height;
						}

						var finalChildDirect = rect.Size().AsOriented(orientation).Direct;

						if (IsArrangeFixed(flexElement) == false && finalChildDirect.IsLessThan(childFinalOriented.Direct))
						{
							var length = finalChildDirect;

							flexElement.SetLengths(length, length, length, length);
							currentFlexElements[index] = flexElement;
							nextArrangePass = true;

							break;
						}

						if (useLayoutRounding)
						{
							var rectSize = rect.Size().AsOriented(orientation);

							flexElement.ActualLength = rectSize.Direct;
							currentPoint.Direct = Math.Max(0, (currentPoint.Direct + rectSize.Direct + spacing).LayoutRound(orientation, RoundingMode.MidPointFromZero));
						}
						else
						{
							var rectSize = rect.Size().AsOriented(orientation);

							flexElement.ActualLength = rectSize.Direct;
							currentPoint.Direct = Math.Max(0, currentPoint.Direct + rectSize.Direct + spacing);
						}

						currentFlexElements[index] = flexElement;

						spacingOffset += spacing;

						size = size.StackSize(childFinalOriented);
					}

					if (nextArrangePass)
						continue;

					if (spacingOffset.Equals(0.0) == false)
						size.Direct = Math.Max(0, size.Direct + spacingOffset - spacing);

					var result = finalSize;

					if (orientation == Orientation.Horizontal)
					{
						var shouldFill = ShouldFillIndirect ?? flexPanel.ShouldFill(Orientation.Horizontal);

						result.Width = shouldFill ? finalSize.Width : Math.Min(finalSize.Width, size.Width);
					}
					else
					{
						var shouldFill = ShouldFillIndirect ?? flexPanel.ShouldFill(Orientation.Vertical);

						result.Height = shouldFill ? finalSize.Height : Math.Min(finalSize.Height, size.Height);
					}

					return result;
				}
			}
			finally
			{
				FlexElementCollection.Release(currentFlexElements);
			}
		}

		private static double CalcSpacingDelta(int visibleCount, double spacing)
		{
			if (spacing.Equals(0.0))
				return 0.0;

			if (visibleCount < 2)
				return 0.0;

			return (visibleCount - 1) * spacing;
		}

		private bool CanHide(FlexElement flexElement)
		{
			return (flexElement.OverflowBehavior & FlexOverflowBehavior.Hide) != 0;
		}

		private bool CanPin(FlexElement flexElement)
		{
			return (flexElement.OverflowBehavior & FlexOverflowBehavior.Pin) != 0;
		}

		private bool CanPinStretch(FlexElement flexElement)
		{
			return (flexElement.OverflowBehavior & FlexOverflowBehavior.Pin) != 0 && (flexElement.OverflowBehavior & FlexOverflowBehavior.Stretch) != 0;
		}

		private OrientedSize FinalMeasureItems(OrientedSize availableOriented, double spacing, bool skipHiddenSpacing)
		{
			var children = Panel.Elements;
			var childrenCount = children.Count;
			var orientation = Panel.Orientation;
			var desiredOriented = new OrientedSize(orientation);

			for (var index = 0; index < childrenCount; index++)
			{
				var child = children[index];
				var flexItem = FlexElements[index];
				var childFinalDirect = flexItem.ActualLength;
				var childDesiredOriented = child.DesiredSize.AsOriented(orientation);

				if (flexItem.ActualLength.IsLessThan(childDesiredOriented.Direct) == false)
				{
					childDesiredOriented.Direct = flexItem.ActualLength;
					desiredOriented = desiredOriented.StackSize(childDesiredOriented);

					continue;
				}

				childDesiredOriented = MeasureChild(child, OrientedSize.Create(orientation, childFinalDirect, availableOriented.Indirect));
				flexItem = flexItem.WithUIElement(child, orientation);
				childDesiredOriented.Direct = flexItem.ActualLength;
				desiredOriented = desiredOriented.StackSize(childDesiredOriented);
				FlexElements[index] = flexItem;
			}

			desiredOriented.Direct = Math.Max(0, desiredOriented.Direct + CalcSpacingDelta(GetVisibleCount(Panel, skipHiddenSpacing), spacing));

			return desiredOriented;
		}

		private Size GetActualArrangeSize(UIElement uie)
		{
			var fre = uie as FrameworkElement;

			return fre?.GetActualSize().Rect().GetInflated(fre.Margin).Size() ?? Size.Empty;
		}

		internal FlexElement GetActualElement(IFlexPanel panel, UIElement child)
		{
			return FlexElements.ElementAtOrDefault(panel.Elements.IndexOf(child));
		}

		private static OrientedSize GetChildConstraint(FlexElement flexElement, OrientedSize autoConstraint, OrientedSize starConstraint)
		{
			if (starConstraint.Direct.IsNaN())
				starConstraint.Direct = double.PositiveInfinity;

			if (autoConstraint.Direct.IsNaN())
				autoConstraint.Direct = double.PositiveInfinity;

			var flexLength = flexElement.Length;

			if (flexLength.IsStar)
			{
				if (starConstraint.Direct.IsPositiveInfinity())
					return starConstraint;

				starConstraint.Direct *= flexLength.Value;

				return starConstraint;
			}

			if (flexLength.IsAbsolute)
			{
				autoConstraint.Direct = flexLength.Value;

				return autoConstraint;
			}

			return autoConstraint;
		}

		private static OrientedSize GetFinalMeasureSize(OrientedSize availableSize, OrientedSize desiredSize, OrientedSize visibleSize)
		{
			var finalSize = availableSize;

			// Indirect
			finalSize.Indirect = finalSize.Indirect.IsInfinity() ? desiredSize.Indirect : visibleSize.Indirect;

			// Direct
			finalSize.Direct = finalSize.Direct.IsInfinity() ? desiredSize.Direct : visibleSize.Direct;

			return finalSize.ConstraintSize(availableSize);
		}

		private static double GetRoundSpacing(double spacing, bool useLayoutRounding)
		{
			if (spacing.Equals(0.0))
				return 0.0;

			if (useLayoutRounding == false)
				return spacing;

			spacing = spacing.Truncate();

			if (Math.Abs(spacing) < 1)
				return Math.Sign(spacing) * 1.0;

			return spacing;
		}

		private static int GetVisibleCount(IFlexPanel flexPanel, bool skipHiddenSpacing)
		{
			var count = 0;

			foreach (var c in flexPanel.Elements)
			{
				if (c.IsVisible() && (skipHiddenSpacing == false || flexPanel.GetIsHidden(c) == false))
					count++;
			}

			return count;
		}

		internal static void InvalidateFlexMeasure(IFlexPanel flexPanel)
		{
			flexPanel.InvalidateMeasure();

			var dependencyObject = flexPanel as DependencyObject;

			while (dependencyObject != null)
			{
				if (dependencyObject is IFlexPanel parentPanel)
					parentPanel.InvalidateMeasure();

				dependencyObject = dependencyObject.GetVisualParent();
			}
		}

		private bool IsArrangeFixed(FlexElement element)
		{
			return Equals(element.MinLength, element.MaxLength) && Equals(element.MinLength, element.ActualLength);
		}

		private static OrientedSize MeasureChild(UIElement child, OrientedSize childConstraint)
		{
			child.Measure(childConstraint.Size);

			return child.DesiredSize.AsOriented(childConstraint.Orientation);
		}

		protected override Size MeasureCore(Size availableSize)
		{
			return MeasureCoreImpl(availableSize);
		}

		private Size MeasureCoreImpl(Size availableSize)
		{
			var flexPanel = Panel;
			var orientation = flexPanel.Orientation;
			var availableOriented = availableSize.AsOriented(orientation);
			var children = flexPanel.Elements;
			var visibility = VisibilityArrayPool.GetArray(children.Count);

			_measureInfinite = availableOriented.Direct.IsPositiveInfinity();

			// First measure pass
			try
			{
				for (var index = 0; index < children.Count; index++)
				{
					var child = children[index];

					Panel.SetIsHidden(child, false);
					visibility[index] = child.Visibility;
				}

				Panel.HasHiddenChildren = false;

				var result = MeasureImpl(availableOriented.Size, out var isOverflowedChanged);
				var childVisibilityChanged = false;

				// Check children visibility changes
				if (isOverflowedChanged == false)
				{
					var count = Math.Min(children.Count, visibility.Length);

					for (var index = 0; index < count; index++)
					{
						childVisibilityChanged = visibility[index] != children[index].Visibility;

						if (childVisibilityChanged)
							break;
					}
				}

				// Second measure pass
				if (isOverflowedChanged || childVisibilityChanged)
					result = MeasureImpl(availableOriented.Size, out _);

				return result.AsOriented(orientation).Size;
			}
			finally
			{
				VisibilityArrayPool.ReleaseArray(visibility);
			}
		}

		private Size MeasureImpl(Size availableSize, out bool overflowChanged)
		{
			overflowChanged = false;

			var children = Panel.Elements;
			var orientation = Panel.Orientation;
			var childrenCount = children.Count;

			if (childrenCount == 0)
				return XamlConstants.ZeroSize;

			var useLayoutRounding = Panel.UseLayoutRounding;

			FlexElements.UseLayoutRounding = useLayoutRounding;

			if (useLayoutRounding)
				availableSize = availableSize.LayoutRound(RoundingMode.MidPointFromZero);

			var spacing = GetRoundSpacing(Panel.Spacing, useLayoutRounding);
			var spacingDelta = CalcSpacingDelta(GetVisibleCount(Panel, false), spacing);
			var availableOriented = availableSize.AsOriented(orientation);

			// First measure
			var desiredOriented = MeasureItems(availableSize, out var desiredFixed, out _);

			if (MeasureMode == PanelMeasureMode.Desired)
			{
				if (desiredFixed.Direct + spacing < availableOriented.Direct)
				{
					desiredFixed.ChangeDirect(desiredOriented.Direct + spacingDelta);

					return desiredFixed.Size;
				}
			}

			// Stretch
			desiredOriented.Direct = Stretch(FlexElements, spacingDelta, availableOriented.Direct);

			// Final measure
			desiredOriented = FinalMeasureItems(availableOriented, spacing, false);

			// Overflow
			var visibleSize = ProcessSpacingAndOverflow(spacing, desiredOriented, availableOriented, out overflowChanged);

			// Return
			return GetFinalMeasureSize(availableOriented.Clamp(desiredFixed, XamlConstants.InfiniteSize.AsOriented(orientation)), desiredOriented, visibleSize).Size;
		}

		private OrientedSize MeasureItems(Size availableSize, out OrientedSize fixedSize, out OrientedSize flexibleSize)
		{
			var stretch = Panel.Stretch;
			var orientation = Panel.Orientation;
			var children = Panel.Elements;
			var childrenCount = children.Count;
			var oriented = availableSize.AsOriented(orientation);
			var fixedChildConstraint = oriented.Clone.ChangeDirect(double.PositiveInfinity);
			var starChildConstraint = oriented.Clone.ChangeDirect(0);
			var fixedResult = new OrientedSize(orientation);
			var starValue = 0.0;

			FlexElements.EnsureCount(childrenCount);

			for (var index = 0; index < childrenCount; index++)
			{
				var uiElement = children[index];
				var flexElement = Panel.GetFlexElement(uiElement).WithOrientation(orientation);

				if (uiElement.Visibility == Visibility.Collapsed)
					flexElement.StretchDirection = FlexStretchDirection.None;

				if (flexElement.IsStar)
					starValue += flexElement.Length.Value;

				FlexElements[index] = flexElement;
			}

			// None Stretch
			if (stretch == FlexStretch.None)
			{
				for (var index = 0; index < childrenCount; index++)
				{
					var flexElement = FlexElements[index];
					var child = children[index];
					var childConstraint = GetChildConstraint(flexElement, fixedChildConstraint, starChildConstraint);

					// Stack child size
					var size = MeasureChild(child, childConstraint);

					flexElement = flexElement.WithUIElement(child, orientation);
					size.Direct = flexElement.DesiredLength;
					fixedResult = fixedResult.StackSize(size);
					FlexElements[index] = flexElement;
				}

				fixedSize = fixedResult;
				flexibleSize = new OrientedSize(orientation);

				return fixedResult;
			}

			// Fixed size children
			for (var index = 0; index < childrenCount; index++)
			{
				var flexElement = FlexElements[index];

				if (flexElement.IsStar)
					continue;

				var child = children[index];
				var childConstraint = GetChildConstraint(flexElement, fixedChildConstraint, starChildConstraint);

				// Stack child size
				var size = MeasureChild(child, childConstraint);

				flexElement = flexElement.WithUIElement(child, orientation);
				size.Direct = flexElement.DesiredLength;
				fixedResult = fixedResult.StackSize(size);
				FlexElements[index] = flexElement;
			}

			fixedSize = fixedResult;
			starChildConstraint.ChangeDirect(FlexUtils.CalcStarValue(oriented.Direct, fixedResult.Direct, starValue));

			// Star size children
			var flexibleResult = new OrientedSize(orientation);

			for (var index = 0; index < childrenCount; index++)
			{
				var flexElement = FlexElements[index];

				if (flexElement.IsFixed)
					continue;

				var child = children[index];
				var childConstraint = GetChildConstraint(flexElement, fixedChildConstraint, starChildConstraint);

				// Stack child size
				var size = MeasureChild(child, childConstraint);

				flexElement = flexElement.WithUIElement(child, orientation);
				size.Direct = flexElement.DesiredLength;
				flexibleResult = flexibleResult.StackSize(size);
				FlexElements[index] = flexElement.WithUIElement(child, orientation);
			}

			flexibleSize = flexibleResult;

			return fixedResult.StackSize(flexibleResult);
		}

		public override void OnLayoutUpdated()
		{
			var orientation = Panel.Orientation;
			var useLayoutRounding = Panel.UseLayoutRounding;

			for (var index = 0; index < Panel.Elements.Count; index++)
			{
				var fre = Panel.Elements[index] as FrameworkElement;

				if (fre == null)
					continue;

				var prevFlexItem = FlexElements[index];
				var nextFlexItem = prevFlexItem.WithUIElement(fre, orientation).WithRounding(useLayoutRounding);

				if (prevFlexItem.MinLength.IsCloseTo(nextFlexItem.MinLength) && prevFlexItem.MaxLength.IsCloseTo(nextFlexItem.MaxLength))
					continue;

				Panel.InvalidateMeasure();

				return;
			}
		}

		private OrientedSize ProcessSpacingAndOverflow(double spacing, OrientedSize desiredOriented, OrientedSize availableOriented, out bool isHiddenChanged)
		{
			var childrenCount = Panel.Elements.Count;
			var orientation = Panel.Orientation;

			var target = availableOriented.Direct;

			isHiddenChanged = false;

			// Current length is greater than available and we have no possibility to stretch down -> mark elements as hidden
			var current = 0.0;
			var visible = 0.0;

			var hasHiddenChildren = false;
			var visibleChildrenCount = 0;

			if (desiredOriented.Direct.IsGreaterThan(availableOriented.Direct))
			{
				var stretchOverflow = FlexElementCollection.Mount(FlexElements.Capacity);

				try
				{
					// Process Pinned Flexible
					for (var index = 0; index < childrenCount; index++)
					{
						var flexElement = FlexElements[index];

						if (CanPinStretch(flexElement) == false)
							continue;

						flexElement.StretchDirection = FlexStretchDirection.Shrink;

						stretchOverflow.Add(flexElement);

						Panel.SetIsHidden(Panel.Elements[index], false);
					}

					// Process Pinned
					for (var index = 0; index < childrenCount; index++)
					{
						var child = Panel.Elements[index];

						if (child.Visibility == Visibility.Collapsed)
							continue;

						var flexElement = FlexElements[index];

						if (CanPin(flexElement) == false)
							continue;

						current += flexElement.ActualLength;
						current += spacing;

						visible = current;

						Panel.SetIsHidden(Panel.Elements[index], false);
					}

					// Process Hide
					for (var index = 0; index < childrenCount; index++)
					{
						var child = Panel.Elements[index];

						if (child.Visibility == Visibility.Collapsed)
							continue;

						visibleChildrenCount++;

						var flexElement = FlexElements[index];

						if (CanPin(flexElement))
							continue;

						current += flexElement.ActualLength;

						if (CanHide(flexElement) == false)
						{
							isHiddenChanged |= Panel.GetIsHidden(child);
							Panel.SetIsHidden(child, false);

							current += spacing;

							visible = current;

							continue;
						}

						var isOverflowed = current.IsGreaterThan(target, XamlConstants.LayoutComparisonPrecision);

						current += spacing;

						if (isOverflowed == false)
							visible = current;

						isHiddenChanged |= Panel.GetIsHidden(child) != isOverflowed;
						hasHiddenChildren |= isOverflowed;

						Panel.SetIsHidden(child, isOverflowed);
					}

					if (visibleChildrenCount > 0)
						visible -= spacing;

					Panel.HasHiddenChildren = hasHiddenChildren;

					// Stretch Pinned
					if (visible.IsGreaterThan(availableOriented.Direct) && stretchOverflow.Count > 0)
					{
						var currentPinStretch = stretchOverflow.Actual;
						var pinStretchTarget = (currentPinStretch - (visible - availableOriented.Direct)).Clamp(0, availableOriented.Direct);
						var pinStretchDesired = stretchOverflow.Stretch(FlexStretch.Fill, pinStretchTarget, FlexDistributor.Equalizer);

						if (pinStretchDesired < currentPinStretch)
						{
							var pinStretchIndex = 0;

							for (var index = 0; index < childrenCount; index++)
							{
								var flexElement = FlexElements[index];

								if (CanPinStretch(flexElement))
									FlexElements[index] = stretchOverflow[pinStretchIndex++].WithStretchDirection(FlexStretchDirection.Shrink).WithMaxLength(flexElement.ActualLength);
								else
									FlexElements[index] = FlexElements[index].WithStretchDirection(FlexStretchDirection.Shrink).WithShrinkPriority(short.MaxValue);
							}

							FinalMeasureItems(availableOriented, spacing, true);

							return OrientedSize.Create(orientation, availableOriented.Direct, desiredOriented.Indirect);
						}
					}
				}
				finally
				{
					FlexElementCollection.Release(stretchOverflow);
				}

				return OrientedSize.Create(orientation, visible.Clamp(0, availableOriented.Direct), desiredOriented.Indirect);
			}

			for (var index = 0; index < childrenCount; index++)
			{
				var flexElement = FlexElements[index];
				var child = Panel.Elements[index];

				if (child.Visibility == Visibility.Collapsed)
					continue;

				visibleChildrenCount++;

				current += flexElement.ActualLength;
				current += spacing;

				Panel.SetIsHidden(Panel.Elements[index], false);
			}

			Panel.HasHiddenChildren = false;
			visible = Math.Max(0, current);

			if (visibleChildrenCount > 0)
				visible = visible - spacing;

			return OrientedSize.Create(orientation, visible.Clamp(0, availableOriented.Direct), desiredOriented.Indirect);
		}

		private double Stretch(FlexElementCollection flexElements, double spacingDelta, double availableDirect)
		{
			if (flexElements.Count == 0)
				return 0.0;

			var stretch = Panel.Stretch;
			var target = availableDirect;
			
			if (spacingDelta.Equals(0.0) == false)
				target = Math.Max(0, target - spacingDelta);

			return flexElements.Stretch(stretch, target, Panel.Distributor) + spacingDelta;
		}
	}

	internal class ArrayPool<T>
	{
		private readonly List<T[]> _pool = new();

		public T[] GetArray(int capacity)
		{
			if (_pool.Count == 0)
				return new T[capacity];

			var array = _pool[_pool.Count - 1];

			_pool.RemoveAt(_pool.Count - 1);

			ArrayUtils.EnsureArrayLength(ref array, capacity);

			return array;
		}

		public void ReleaseArray(T[] array)
		{
			_pool.Add(array);
		}
	}
}