// <copyright file="VirtualPixelStackPanelLayout.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Specialized;
using System.Windows;
using Zaaml.Core.Collections.Specialized;
using Zaaml.Core.Extensions;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.Extensions;
using Zaaml.UI.Controls.Core;
using Zaaml.UI.Controls.ScrollView;
using Zaaml.UI.Panels.Interfaces;

namespace Zaaml.UI.Panels.VirtualStackPanelLayout
{
	internal class VirtualPixelStackPanelLayout : VirtualStackPanelLayoutBase
	{
		public VirtualPixelStackPanelLayout(IVirtualStackPanel panel) : base(panel)
		{
		}

		protected override double DirectSmallChange => ScrollViewUtils.DefaultPixelSmallChange;

		protected override double DirectWheelChange => ScrollViewUtils.DefaultPixelWheelChange;

		public override ScrollUnit ScrollUnit => ScrollUnit.Pixel;

		private SizeLinkedList SizeList { get; } = new SizeLinkedList();

		private protected override Vector CalcBringIntoViewOffset(int index, BringIntoViewMode mode, ScrollInfo scrollInfo)
		{
			var indexOffset = CalcOffset(index, 0);

			var orientedExtent = scrollInfo.Extent.AsOriented(Orientation);

			orientedExtent.Direct = CalcDirectExtent();

			scrollInfo = scrollInfo.WithExtent(orientedExtent.Size);

			if (mode == BringIntoViewMode.Top)
				return scrollInfo.ClampOffset(indexOffset);

			var itemSize = GetSize(index);
			var orientation = Orientation;
			var orientedOffset = scrollInfo.Offset.AsOriented(orientation);
			var orientedIndexOffset = indexOffset.AsOriented(orientation);
			var viewportDirect = scrollInfo.Viewport.AsOriented(orientation).Direct;

			if (orientedIndexOffset.Direct < orientedOffset.Direct)
				return scrollInfo.ClampOffset(orientedIndexOffset.Vector);

			orientedIndexOffset.Direct += itemSize;
			orientedIndexOffset.Direct -= viewportDirect;

			return scrollInfo.ClampOffset(orientedIndexOffset.Vector);
		}

		private protected override int CalcFirstVisibleIndex(Vector offset, out double localFirstVisibleOffset)
		{
			if (SizeList.Count == 0)
			{
				localFirstVisibleOffset = 0;

				return 0;
			}

			var orientedOffset = offset.AsOriented(Orientation);
			var offsetDirect = orientedOffset.Direct;

			return offsetDirect.IsGreaterThan(SizeList.Size)
				? CalcFirstVisibleIndexOutsideSizeList(offsetDirect, out localFirstVisibleOffset)
				: CalcFirstVisibleIndexWithinSizeList(offsetDirect, out localFirstVisibleOffset);
		}

		private int CalcFirstVisibleIndexOutsideSizeList(double orientedOffsetDirect, out double localFirstVisibleOffset)
		{
			var deltaOffset = orientedOffsetDirect - SizeList.Size;
			var averageSize = SizeList.AverageSize;
			var index = (long) (deltaOffset / averageSize);
			var offset = index * averageSize;
			var localOffset = deltaOffset - offset;

			if (localOffset.IsZero())
			{
				localFirstVisibleOffset = 0;

				return (int) (SizeList.Count + index);
			}

			localFirstVisibleOffset = averageSize - localOffset;

			return (int) (SizeList.Count + index + 1);
		}

		private int CalcFirstVisibleIndexWithinSizeList(double offset, out double localFirstVisibleOffset)
		{
			var index = SizeList.FindIndex(offset, out localFirstVisibleOffset);

			if (index == -1)
				return 0;

			if (localFirstVisibleOffset.IsZero())
				return (int) index;

			var size = SizeList[index];

			localFirstVisibleOffset = size - localFirstVisibleOffset;
			index++;

			return (int) index;
		}

		private Vector CalcOffset(int index, double localOffset)
		{
			var orientedOffset = ScrollInfo.Offset.AsOriented(Orientation);

			if (Source.Count == 0)
				return orientedOffset.Vector;

			index = index.Clamp(0, Source.Count - 1);

			if (index < SizeList.Count)
				orientedOffset.Direct = SizeList.FindOffset(index) - localOffset;
			else
			{
				var deltaCount = index - SizeList.Count;
				var deltaOffset = deltaCount * SizeList.AverageSize;

				orientedOffset.Direct = SizeList.Size + deltaOffset - localOffset;
			}

			return orientedOffset.Vector;
		}

		private double CalcDirectExtent()
		{
			var actualOrientedSize = SizeList.Size;

			if (SizeList.Count < Source.Count)
				actualOrientedSize += SizeList.AverageSize * (Source.Count - SizeList.Count);

			return actualOrientedSize;
		}

		private protected override ScrollInfo CalcScrollInfo(ref VirtualMeasureContext context)
		{
			var orientedAvailable = context.OrientedAvailable;
			var orientation = Orientation;
			var orientedExtent = new OrientedSize(orientation)
			{
				Direct = CalcDirectExtent(),
				Indirect = Math.Max(context.OrientedResult.Indirect, context.OrientedAvailable.Indirect)
			};

			var offset = CalcOffset(context.PreviewFirstVisibleIndex, context.PreviewFirstVisibleOffset);
			var orientedViewport = new OrientedSize(orientation).ChangeDirect(orientedAvailable.Direct).ChangeIndirect(orientedAvailable.Indirect);
			
			return new ScrollInfo(offset, orientedViewport.Size, orientedExtent.Size);
		}

		private double GetSize(int index)
		{
			if (SizeList.Count == 0)
				return 0;

			return index < SizeList.Count ? SizeList[index] : SizeList.AverageSize;
		}

		private protected override void MeasureChild(int index, UIElement child, Size constraint, ref VirtualMeasureContext context)
		{
			base.MeasureChild(index, child, constraint, ref context);

			if (PreserveScrollInfo)
				return;

			SetSize(index, child);
		}

		private protected override void OnSourceCollectionChanged(NotifyCollectionChangedEventArgs args)
		{
			base.OnSourceCollectionChanged(args);

			var scrollInfo = ScrollInfo;
			var firstIndex = CalcFirstVisibleIndex(scrollInfo.Offset, out var localOffset);

			switch (args.Action)
			{
				case NotifyCollectionChangedAction.Add:

					var addCount = args.NewItems?.Count ?? 0;

					if (args.NewStartingIndex < firstIndex)
						firstIndex += addCount;

					if (args.NewStartingIndex >= SizeList.Count || SizeList.Count == 0)
						return;

					var itemSize = SizeList[args.NewStartingIndex];

					if (addCount > 0)
						SizeList.AddSizeRange(addCount, itemSize);

					break;

				case NotifyCollectionChangedAction.Remove:

					var removeCount = args.OldItems?.Count ?? 0;

					if (args.NewStartingIndex < firstIndex)
						firstIndex -= removeCount;

					if (args.OldStartingIndex >= SizeList.Count || SizeList.Count == 0)
						return;

					if (removeCount > 0)
					{
						var coerceCount = Math.Min(SizeList.Count - args.OldStartingIndex, removeCount);

						SizeList.RemoveSizeRange(args.OldStartingIndex, coerceCount);
					}

					break;

				case NotifyCollectionChangedAction.Replace:
					break;

				case NotifyCollectionChangedAction.Move:

					if (SizeList.Count == 0)
						return;

					if (args.OldItems != null && args.OldItems.Count == 1)
					{
						if (args.OldStartingIndex >= SizeList.Count && args.NewStartingIndex >= SizeList.Count)
							return;

						var size = SizeList.AverageSize;

						if (args.OldStartingIndex < SizeList.Count)
						{
							size = SizeList[args.OldStartingIndex];

							SizeList.RemoveSizeAt(args.OldStartingIndex);
						}

						if (args.NewStartingIndex < SizeList.Count)
							SizeList.InsertSize(args.NewStartingIndex, size);
					}

					break;

				case NotifyCollectionChangedAction.Reset:

					ResetSizeList();

					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private void ResetSizeList()
		{
			if (SizeList.Count == 0)
				return;

			SizeList.ResetToAverage();

			if (Source.Count == 0)
				SizeList.Clear();
			else if (SizeList.Count > Source.Count)
			{
				var exceedCount = SizeList.Count - Source.Count;

				SizeList.RemoveSizeRange(SizeList.Count - exceedCount - 1, exceedCount);
			}

			foreach (UIElement child in Children)
			{
				var index = Source.GetIndexFromItem(child);

				if (index == -1)
					continue;

				SetSize(index, child);
			}
		}

		private void SetSize(int index, UIElement element)
		{
			var size = element.DesiredSize.AsOriented(Orientation).Direct;

			if (index == SizeList.Count)
				SizeList.AddSize(size);
			else if (index > SizeList.Count)
			{
				SizeList.AddSizeRange(index - SizeList.Count, SizeList.AverageSize);
				SizeList.AddSize(size);
			}
			else
				SizeList[index] = size;
		}
	}
}