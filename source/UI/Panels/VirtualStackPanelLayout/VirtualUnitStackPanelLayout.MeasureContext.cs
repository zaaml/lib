// <copyright file="VirtualUnitStackPanelLayout.MeasureContext.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using Zaaml.Core.Extensions;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.Extensions;
using Zaaml.UI.Controls.ScrollView;

namespace Zaaml.UI.Panels.VirtualStackPanelLayout
{
	internal partial class VirtualUnitStackPanelLayout
	{
		#region  Nested Types

		private struct VirtualMeasureContext
		{
			private VirtualUnitStackPanelLayout Layout { get; }

			private int OffsetElementIndex { get; }

			private OrientedSize OrientedResult { get; set; }

			private OrientedSize OrientedAvailable { get; }

			private OrientedSize OrientedConstraint { get; }

			private int FirstIndex { get; set; }

			private int LastIndex { get; set; }

			private int FirstVisibleIndex { get; set; }

			private int LastVisibleIndex { get; set; }

			private int LeadingCacheCount { get; set; }

			private int TrailingCacheCount { get; set; }

			private int SourceCount { get; }

			private int VisibleCount => FirstVisibleIndex == -1 ? 0 : LastVisibleIndex - FirstVisibleIndex + 1;

			private Orientation Orientation { get; }

			private OrientedSize OrientedLeadingSize { get; set; }

			private OrientedSize OrientedTrailingSize { get; set; }

			public double PreCacheDelta => OrientedLeadingSize.Direct;

			public VirtualMeasureContext(VirtualUnitStackPanelLayout layout, Size availableSize)
			{
				Layout = layout;

				Orientation = layout.Orientation;

				SourceCount = Layout.ItemsCount;

				var canScrollDirect = Orientation == Orientation.Vertical ? Layout.CanVerticallyScroll : Layout.CanHorizontallyScroll;
				var canScrollIndirect = Orientation == Orientation.Vertical ? Layout.CanHorizontallyScroll : Layout.CanVerticallyScroll;

				OrientedResult = new OrientedSize(Orientation);
				OrientedLeadingSize = new OrientedSize(Orientation);
				OrientedTrailingSize = new OrientedSize(Orientation);
				OrientedAvailable = availableSize.AsOriented(Orientation);
				OffsetElementIndex = layout.CalcFirstVisibleIndex(layout.Offset);
				OrientedConstraint = new OrientedSize(Orientation)
				                     .ChangeDirect(canScrollDirect ? double.PositiveInfinity : OrientedAvailable.Direct)
				                     .ChangeIndirect(canScrollIndirect ? double.PositiveInfinity : OrientedAvailable.Indirect);

				LastIndex = -1;
				FirstIndex = -1;
				FirstVisibleIndex = -1;
				LastVisibleIndex = -1;
				LeadingCacheCount = 0;
				TrailingCacheCount = 0;
			}

			public void Measure(int focusedIndex)
			{
				if (SourceCount == 0)
					return;
				
				MeasureLeading();
				MeasureVisible();
				MeasureTrailing();
				MeasureFocusedItem(focusedIndex);
			}

			private void MeasureLeading()
			{
				FirstIndex = (OffsetElementIndex - Layout.Panel.LeadingTrailingLimit).Clamp(0, SourceCount);
				FirstVisibleIndex = OffsetElementIndex.Clamp(0, SourceCount);

				var constraint = OrientedConstraint.Size;
				var index = FirstIndex;

				while (index < FirstVisibleIndex && index < SourceCount)
				{
					var element = EnsureElement(Layout.Realize(index));
					
					if (element == null)
					{
						index++;
						
						continue;
					}

					AddItem(element);
					Layout.MeasureChild(element, constraint);

					OrientedLeadingSize = OrientedLeadingSize.StackSize(element.DesiredSize);
					LeadingCacheCount++;

					index++;
				}
			}

			private void MeasureFocusedItem(int focusedIndex)
      {
        if (focusedIndex < 0 || focusedIndex >= SourceCount)
          return;

        if (focusedIndex >= FirstIndex && focusedIndex <= LastIndex)
          return;

        var focusedItem = EnsureElement(Layout.Realize(focusedIndex));

        if (focusedItem == null)
          return;
				
				if (focusedIndex < FirstIndex)
				{
					InsertItem(0, focusedItem);
					Layout.MeasureChild(focusedItem, OrientedConstraint.Size);

					OrientedLeadingSize = OrientedLeadingSize.StackSize(focusedItem.DesiredSize);
				}
				else
				{
					AddItem(focusedItem);
					Layout.MeasureChild(focusedItem, OrientedConstraint.Size);

					OrientedTrailingSize = OrientedTrailingSize.StackSize(focusedItem.DesiredSize);
				}
			}

      private void InsertItem(int index, UIElement item)
      {
	      Layout.UIElementInserter.Insert(index, item);
      }

      private void AddItem(UIElement item)
      {
	      Layout.UIElementInserter.Add(item);
      }

      private UIElement EnsureElement(UIElement element)
      {
	      return element;
      }
      
      private void MeasureVisible()
			{
				var constraint = OrientedConstraint.Size;
				var index = FirstVisibleIndex;
				var realizedIndex = index;

				while (OrientedAvailable.Direct > OrientedResult.Direct && index < SourceCount)
				{
					var element = EnsureElement(Layout.Realize(index));

					if (element == null)
					{
						index++;
						
						continue;
					}
					
					realizedIndex = index;

					AddItem(element);
					Layout.MeasureChild(element, constraint);

					OrientedResult = OrientedResult.StackSize(element.DesiredSize);

					index++;
				}

				LastVisibleIndex = realizedIndex;
				LastIndex = realizedIndex;

				// Offset error. Move leading cache
				while (OrientedAvailable.Direct > OrientedResult.Direct && FirstVisibleIndex > FirstIndex && FirstVisibleIndex > 0)
				{
					var element = Layout.UIElementInserter[FirstVisibleIndex - FirstIndex - 1];
					var orientedSize = element.DesiredSize.AsOriented(Orientation);
					var direct = OrientedResult.Direct + orientedSize.Direct;

					if (direct > OrientedAvailable.Direct)
						return;

					OrientedResult = OrientedResult.ChangeDirect(direct);
					OrientedLeadingSize = OrientedLeadingSize.ChangeDirect(OrientedLeadingSize.Direct - orientedSize.Direct);

					FirstVisibleIndex--;
					LeadingCacheCount--;
				}

				// Offset error. Measure beyond offset
				if (OrientedAvailable.Direct > OrientedResult.Direct && FirstVisibleIndex > 0)
				{
					Debug.Assert(LeadingCacheCount == 0);

					while (OrientedAvailable.Direct > OrientedResult.Direct && FirstVisibleIndex > 0)
					{
						var element = EnsureElement(Layout.Realize(FirstVisibleIndex - 1));
						
						if (element == null)
						{
							FirstVisibleIndex--;
							
							continue;
						}

						InsertItem(0, element);
						Layout.MeasureChild(element, constraint);

						OrientedResult = OrientedResult.StackSize(element.DesiredSize);

						FirstVisibleIndex--;
					}

					FirstIndex = FirstVisibleIndex;

					// Remeasure leading cache
					while (LeadingCacheCount < Layout.Panel.LeadingTrailingLimit && FirstIndex > 0)
					{
						var element = EnsureElement(Layout.Realize(FirstIndex - 1));
						
						if (element == null)
						{
							FirstIndex--;
							
							continue;
						}

						InsertItem(0, element);
						Layout.MeasureChild(element, constraint);

						OrientedLeadingSize = OrientedLeadingSize.StackSize(element.DesiredSize);

						FirstIndex--;
						LeadingCacheCount++;
					}
				}
			}

      private void MeasureTrailing()
			{
				var constraint = OrientedConstraint.Size;
				var index = LastVisibleIndex + 1;
				var realizedIndex = index;

				while (index < SourceCount && TrailingCacheCount <= Layout.Panel.LeadingTrailingLimit)
				{
					var element = EnsureElement(Layout.Realize(index));
					
					if (element == null)
					{
						index++;
						
						continue;
					}

					realizedIndex = index;

					AddItem(element);
					Layout.MeasureChild(element, constraint);

					OrientedTrailingSize = OrientedTrailingSize.StackSize(element.DesiredSize);

					index++;
					TrailingCacheCount++;
				}

				LastIndex = realizedIndex;
			}

			public Vector CalcOffset()
			{
				if (OffsetElementIndex == FirstVisibleIndex)
					return Layout.Offset;

				var orientedVector = Layout.Offset.AsOriented(Orientation);

				orientedVector.Direct = FirstVisibleIndex;

				return orientedVector.Vector;
			}

			public Size CalcFinalSize()
			{
				var finalOrientedResult = OrientedResult;

				if (finalOrientedResult.Direct > OrientedAvailable.Direct)
					finalOrientedResult.Direct = OrientedAvailable.Direct;

				return finalOrientedResult.Size;
			}

			public ScrollInfo CalcScrollInfo()
			{
				var extentCount = SourceCount;
				var visibleCount = VisibleCount;

				if (OrientedResult.Direct > OrientedAvailable.Direct && visibleCount > 1)
					visibleCount--;

				//if (LastIndex + 1 == SourceCount && OrientedAvailable.Direct >= OrientedResult.Direct)
				//	extentCount = visibleCount;

				return new ScrollInfo
				{
					Viewport = new OrientedSize(Orientation).ChangeDirect(visibleCount).ChangeIndirect(OrientedAvailable.Indirect).Size,
					Extent = new OrientedSize(Orientation).ChangeDirect(extentCount).ChangeIndirect(Math.Max(OrientedResult.Indirect, OrientedAvailable.Indirect)).Size
				};
			}
		}

		#endregion
	}
}