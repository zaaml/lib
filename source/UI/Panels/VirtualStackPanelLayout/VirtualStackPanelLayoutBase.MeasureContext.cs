// <copyright file="VirtualStackPanelLayoutBase.MeasureContext.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Zaaml.Core.Extensions;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.Extensions;
using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Panels.VirtualStackPanelLayout
{
	internal partial class VirtualStackPanelLayoutBase
	{
		protected struct VirtualMeasureContext
		{
			public VirtualStackPanelLayoutBase Layout { get; }

			public int PreviewFirstVisibleIndex { get; private set; }

			public double PreviewFirstVisibleOffset { get; private set; }

			public OrientedSize OrientedResult { get; private set; }

			public OrientedSize OrientedAvailable { get; }

			public OrientedSize OrientedConstraint { get; }

			public int FirstIndex { get; private set; }

			public int LastIndex { get; private set; }

			public int FirstVisibleIndex { get; private set; }

			public int LastVisibleIndex { get; private set; }

			public int LeadingCacheCount { get; private set; }

			public int TrailingCacheCount { get; private set; }

			public bool BringIntoViewResult { get; private set; }

			public int SourceCount { get; }

			public int VisibleCount => FirstVisibleIndex == -1 ? 0 : LastVisibleIndex - FirstVisibleIndex + 1;

			public Orientation Orientation { get; }

			public OrientedSize OrientedLeadingSize { get; private set; }

			public OrientedSize OrientedTrailingSize { get; private set; }

			public ICollection<UIElement> LeadingElements { get; }

			public ICollection<UIElement> TrailingElements { get; }

			public double PreCacheDelta => OrientedLeadingSize.Direct;

			public BringIntoViewRequest BringIntoViewRequest { get; }

			public VirtualMeasureContext(VirtualStackPanelLayoutBase layout, Size availableSize)
			{
				Layout = layout;

				Orientation = layout.Orientation;
				BringIntoViewRequest = layout.BringIntoViewRequest;

				var canScrollDirect = Orientation == Orientation.Vertical ? layout.CanVerticallyScroll : layout.CanHorizontallyScroll;
				var canScrollIndirect = Orientation == Orientation.Vertical ? layout.CanHorizontallyScroll : layout.CanVerticallyScroll;

				LeadingElements = layout.Panel.LeadingElements;
				TrailingElements = layout.Panel.TrailingElements;

				SourceCount = layout.ItemsCount + (LeadingElements?.Count ?? 0) + (TrailingElements?.Count ?? 0);

				OrientedResult = new OrientedSize(Orientation);
				OrientedLeadingSize = new OrientedSize(Orientation);
				OrientedTrailingSize = new OrientedSize(Orientation);
				OrientedAvailable = availableSize.AsOriented(Orientation);

				OrientedConstraint = new OrientedSize(Orientation)
					.ChangeDirect(canScrollDirect ? double.PositiveInfinity : OrientedAvailable.Direct)
					.ChangeIndirect(canScrollIndirect ? double.PositiveInfinity : OrientedAvailable.Indirect);

				BringIntoViewResult = false;
				PreviewFirstVisibleIndex = -1;
				PreviewFirstVisibleOffset = 0;
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

				var scrollInfo = Layout.ScrollInfo;
				
				BringIntoViewResult = Layout.TryHandleBringIntoView(BringIntoViewRequest, ref scrollInfo);
				PreviewFirstVisibleIndex = Layout.CalcFirstVisibleIndex(scrollInfo.Offset, out var localFirstVisibleOffset);
				PreviewFirstVisibleOffset = localFirstVisibleOffset;

				MeasureLeading();
				MeasureVisible();
				MeasureTrailing();
				MeasureFocusedItem(focusedIndex);
			}

			private void MeasureLeading()
			{
				FirstIndex = (PreviewFirstVisibleIndex - Layout.Panel.LeadingTrailingLimit).Clamp(0, SourceCount);
				FirstVisibleIndex = PreviewFirstVisibleIndex.Clamp(0, SourceCount);

				var constraint = OrientedConstraint.Size;
				var index = FirstIndex;

				while (index < FirstVisibleIndex && index < SourceCount)
				{
					var childIndex = index;
					var element = Realize(childIndex);

					if (element == null)
					{
						index++;

						continue;
					}

					AddItem(element);
					Layout.MeasureChild(childIndex, element, constraint, ref this);

					OrientedLeadingSize = OrientedLeadingSize.StackSize(element.DesiredSize);
					LeadingCacheCount++;

					index++;
				}
			}


			private void MeasureVisible()
			{
				var constraint = OrientedConstraint.Size;
				var index = FirstVisibleIndex;
				var realizedIndex = index;

				while (OrientedAvailable.Direct > OrientedResult.Direct && index < SourceCount)
				{
					var childIndex = index;
					var element = Realize(childIndex);

					if (element == null)
					{
						index++;

						continue;
					}

					realizedIndex = index;

					AddItem(element);
					Layout.MeasureChild(childIndex, element, constraint, ref this);

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
						var childIndex = FirstVisibleIndex - 1;
						var element = Realize(childIndex);

						if (element == null)
						{
							FirstVisibleIndex--;

							continue;
						}

						InsertItem(0, element);
						Layout.MeasureChild(childIndex, element, constraint, ref this);

						OrientedResult = OrientedResult.StackSize(element.DesiredSize);

						FirstVisibleIndex--;
					}

					FirstIndex = FirstVisibleIndex;

					// Remeasure leading cache
					while (LeadingCacheCount < Layout.Panel.LeadingTrailingLimit && FirstIndex > 0)
					{
						var childIndex = FirstIndex - 1;
						var element = Realize(childIndex);

						if (element == null)
						{
							FirstIndex--;

							continue;
						}

						InsertItem(0, element);
						Layout.MeasureChild(childIndex, element, constraint, ref this);

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
					var childIndex = index;
					var element = Realize(childIndex);

					if (element == null)
					{
						index++;

						continue;
					}

					realizedIndex = index;

					AddItem(element);
					Layout.MeasureChild(childIndex, element, constraint, ref this);

					OrientedTrailingSize = OrientedTrailingSize.StackSize(element.DesiredSize);

					index++;
					TrailingCacheCount++;
				}

				LastIndex = realizedIndex;
			}

			private void MeasureFocusedItem(int focusedIndex)
			{
				if (focusedIndex < 0 || focusedIndex >= SourceCount)
					return;

				if (focusedIndex >= FirstIndex && focusedIndex <= LastIndex)
					return;

				var focusedItem = Realize(focusedIndex);

				if (focusedItem == null)
					return;

				if (focusedIndex < FirstIndex)
				{
					InsertItem(0, focusedItem);
					Layout.MeasureChild(focusedIndex, focusedItem, OrientedConstraint.Size, ref this);

					OrientedLeadingSize = OrientedLeadingSize.StackSize(focusedItem.DesiredSize);
				}
				else
				{
					AddItem(focusedItem);
					Layout.MeasureChild(focusedIndex, focusedItem, OrientedConstraint.Size, ref this);

					OrientedTrailingSize = OrientedTrailingSize.StackSize(focusedItem.DesiredSize);
				}
			}

			private UIElement Realize(int index)
			{
				var leadingDelta = 0;

				if (LeadingElements != null)
				{
					leadingDelta += LeadingElements.Count;

					if (index >= 0 && index < LeadingElements.Count && LeadingElements.Count > 0)
					{
						var element = LeadingElements.ElementAt(index);

						return EnsureElement(element);
					}
				}

				if (TrailingElements != null)
				{
					if (index - leadingDelta >= Layout.Source.Count)
					{
						if (index >= 0 && index < TrailingElements.Count && TrailingElements.Count > 0)
						{
							var element = TrailingElements.ElementAt(index - leadingDelta - Layout.Source.Count);

							return EnsureElement(element);
						}

						throw new IndexOutOfRangeException(nameof(index));
					}
				}

				return EnsureElement(Layout.Realize(index - leadingDelta));
			}

			private void InsertItem(int index, UIElement item)
			{
				Layout.UIElementInserter.Insert(index, item);

				OnItemAttached(item);
			}

			private void AddItem(UIElement item)
			{
				Layout.UIElementInserter.Add(item);

				OnItemAttached(item);
			}

			private void OnItemAttached(UIElement item)
			{
			}

			private UIElement EnsureElement(UIElement element)
			{
				return element;
			}

			public Size ViewportSize
			{
				get
				{
					var finalOrientedResult = OrientedResult;

					if (finalOrientedResult.Direct > OrientedAvailable.Direct)
						finalOrientedResult.Direct = OrientedAvailable.Direct;

					return finalOrientedResult.Size;
				}
			}
		}
	}
}