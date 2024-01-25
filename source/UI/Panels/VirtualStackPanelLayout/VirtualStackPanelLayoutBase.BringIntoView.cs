// <copyright file="VirtualStackPanelLayoutBase.BringIntoView.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.UI.Controls.Core;
using Zaaml.UI.Controls.ScrollView;

namespace Zaaml.UI.Panels.VirtualStackPanelLayout
{
	internal abstract partial class VirtualStackPanelLayoutBase
	{
		private protected BringIntoViewRequest BringIntoViewRequest { get; private set; }

		public override bool BringIntoView(BringIntoViewRequest request)
		{
			var index = request.Index;

			if (request.ElementInternal != null)
				index = Source.GetIndexFromItem(request.ElementInternal);

			if (index == -1)
				index = request.FallbackIndex;

			return BringIntoView(index, request.Mode);
		}

		private bool BringIntoView(int index, BringIntoViewMode mode)
		{
			if (index < 0 || index >= ItemsCount)
				return false;

			if (TryCalcBringIntoViewOffset(index, mode, ScrollInfo, out var offset))
			{
				Offset = offset;

				BringIntoViewRequest = null;

				return true;
			}

			BringIntoViewRequest = new BringIntoViewRequest<FrameworkElement>(index, mode);

			Panel.InvalidateMeasure();

			return false;
		}

		private protected bool TryHandleBringIntoView(BringIntoViewRequest bringIntoViewRequest, ref ScrollInfo scrollInfo)
		{
			if (bringIntoViewRequest == null)
				return false;

			var bringIntoViewMode = BringIntoViewRequest.Mode;
			var frameworkElement = BringIntoViewRequest.ElementInternal;

			var bringIntoViewIndex = frameworkElement != null
				? Source.GetIndexFromItem(frameworkElement)
				: BringIntoViewRequest.Index;

			if (bringIntoViewIndex == -1)
				bringIntoViewIndex = BringIntoViewRequest.FallbackIndex;

			if (bringIntoViewIndex == -1)
				return false;

			if (TryCalcBringIntoViewOffset(bringIntoViewIndex, bringIntoViewMode, scrollInfo, out var offset))
			{
				scrollInfo = scrollInfo.WithOffset(offset, true);

				return true;
			}

			return false;
		}

		private protected abstract bool TryCalcBringIntoViewOffset(int index, BringIntoViewMode mode, ScrollInfo scrollInfo, out Vector offset);
	}
}