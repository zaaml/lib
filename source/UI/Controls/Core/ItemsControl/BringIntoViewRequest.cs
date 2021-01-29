// <copyright file="BringIntoViewRequest.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;

namespace Zaaml.UI.Controls.Core
{
	internal class BringIntoViewRequest<TItem> : BringIntoViewRequest
		where TItem : FrameworkElement
	{
		public BringIntoViewRequest(TItem item, BringIntoViewMode mode = BringIntoViewMode.Auto, int fallbackIndex = -1) : base(-1, mode, fallbackIndex)
		{
			Item = item;
		}

		public BringIntoViewRequest(int index, BringIntoViewMode mode = BringIntoViewMode.Auto, int fallbackIndex = -1) : base(index, mode, fallbackIndex)
		{
		}

		internal override FrameworkElement ElementInternal => Item;

		public TItem Item { get; set; }
	}

	internal abstract class BringIntoViewRequest
	{
		protected BringIntoViewRequest(int index, BringIntoViewMode mode, int fallbackIndex)
		{
			Index = index;
			Mode = mode;
			FallbackIndex = fallbackIndex;
		}

		internal abstract FrameworkElement ElementInternal { get; }

		public int FallbackIndex { get; }

		public int Index { get; }

		internal virtual int IndexInternal => Index;

		public BringIntoViewMode Mode { get; }
	}
}