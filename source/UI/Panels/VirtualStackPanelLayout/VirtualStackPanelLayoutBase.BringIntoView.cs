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
		private BringIntoViewRequest _enqueueBringIntoViewRequest;

		public override void BringIntoView(BringIntoViewRequest request)
		{
			var index = request.Index;

			if (request.ElementInternal != null)
				index = Source.GetIndexFromItem(request.ElementInternal);

			if (index == -1)
				index = request.FallbackIndex;

			BringIntoView(index, request.Mode);
		}

		private void BringIntoView(int index, BringIntoViewMode mode)
		{
			if (index < 0 || index >= ItemsCount)
				return;

			Offset = CalcBringIntoViewOffset(index, mode, ScrollInfo);

			_enqueueBringIntoViewRequest = null;
		}

		private protected abstract Vector CalcBringIntoViewOffset(int index, BringIntoViewMode mode, ScrollInfo scrollInfo);

		private protected ScrollInfo HandleBringIntoView(ScrollInfo scrollInfo, bool resetRequest)
		{
			try
			{
				if (_enqueueBringIntoViewRequest == null)
					return scrollInfo;

				var bringIntoViewMode = _enqueueBringIntoViewRequest.Mode;
				var frameworkElement = _enqueueBringIntoViewRequest.ElementInternal;

				var bringIntoViewIndex = frameworkElement != null
					? Source.GetIndexFromItem(frameworkElement)
					: _enqueueBringIntoViewRequest.Index;

				if (bringIntoViewIndex == -1)
					bringIntoViewIndex = _enqueueBringIntoViewRequest.FallbackIndex;

				return bringIntoViewIndex == -1 ? scrollInfo : scrollInfo.WithOffset(CalcBringIntoViewOffset(bringIntoViewIndex, bringIntoViewMode, scrollInfo), true);
			}
			finally
			{
				if (resetRequest)
					_enqueueBringIntoViewRequest = null;
			}
		}
	}
}