// <copyright file="VirtualUnitStackPanelLayout.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using System.Windows.Controls;
using Zaaml.Core.Extensions;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.Extensions;
using Zaaml.UI.Controls.Core;
using Zaaml.UI.Controls.ScrollView;
using Zaaml.UI.Panels.Interfaces;
using ScrollUnit = Zaaml.UI.Controls.ScrollView.ScrollUnit;

namespace Zaaml.UI.Panels.VirtualStackPanelLayout
{
	internal class VirtualUnitStackPanelLayout : VirtualStackPanelLayoutBase
	{
		public VirtualUnitStackPanelLayout(IVirtualStackPanel panel) : base(panel)
		{
		}

		protected override double DirectSmallChange => ScrollViewUtils.DefaultUnitSmallChange;

		protected override double DirectWheelChange => ScrollViewUtils.DefaultUnitWheelChange;

		public override ScrollUnit ScrollUnit => ScrollUnit.Item;

		private protected override bool TryCalcBringIntoViewOffset(int index, BringIntoViewMode mode, ScrollInfo scrollInfo, out Vector offset)
		{
			var viewport = scrollInfo.Viewport;
			
			if (index < 0 || index >= ItemsCount)
			{
				offset = scrollInfo.Offset;

				return true;
			}

			var orientation = Orientation;
			var orientedViewPort = viewport.AsOriented(orientation);
			var orientedOffset = scrollInfo.Offset.AsOriented(orientation);
			var orientedScrollInfo = new OrientedScrollInfo(orientation, orientedOffset.Direct, orientedViewPort.Direct, Source.Count);

			orientedScrollInfo.Offset = orientedScrollInfo.Offset - index > 0.0 || mode == BringIntoViewMode.Begin
				? index
				: index - orientedScrollInfo.Viewport + 1.0;

			orientedOffset.Direct = orientedScrollInfo.Offset;

			offset = orientedOffset.Vector;

			return true;
		}

		private protected override int CalcFirstVisibleIndex(Vector offset, out double localFirstVisibleOffset)
		{
			localFirstVisibleOffset = 0;

			return (int) (Orientation == Orientation.Vertical ? offset.Y : offset.X).RoundToZero();
		}

		private protected override ScrollInfo CalcScrollInfo(ref VirtualMeasureContext context)
		{
			Vector offset;
			var orientation = Orientation;

			if (context.PreviewFirstVisibleIndex == context.FirstVisibleIndex)
			{
				offset = Offset;
			}
			else
			{
				var orientedVector = Offset.AsOriented(orientation);

				orientedVector.Direct = context.FirstVisibleIndex;

				offset = orientedVector.Vector;
			}

			var extentCount = context.SourceCount;
			var visibleCount = context.VisibleCount;
			var orientedAvailable = context.OrientedAvailable;

			if (context.OrientedResult.Direct > orientedAvailable.Direct && visibleCount > 1)
				visibleCount--;

			//if (LastIndex + 1 == SourceCount && OrientedAvailable.Direct >= OrientedResult.Direct)
			//	extentCount = visibleCount;

			var viewport = new OrientedSize(orientation).ChangeDirect(visibleCount).ChangeIndirect(orientedAvailable.Indirect).Size;
			var extent = new OrientedSize(orientation).ChangeDirect(extentCount).ChangeIndirect(Math.Max(context.OrientedResult.Indirect, orientedAvailable.Indirect)).Size;

			return new ScrollInfo(offset, viewport, extent);
		}
	}
}