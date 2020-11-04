// <copyright file="IItemsHost.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.UI.Panels;

namespace Zaaml.UI.Controls.Core
{
	internal interface IItemsHost<TItem>
		where TItem : System.Windows.Controls.Control
	{
		ItemHostCollection<TItem> Items { get; }

		void BringIntoView(BringIntoViewRequest<TItem> request);

		void EnqueueBringIntoView(BringIntoViewRequest<TItem> request);

		ItemLayoutInformation GetLayoutInformation(int index);

		ItemLayoutInformation GetLayoutInformation(TItem item);
	}

	internal enum BringIntoViewMode
	{
		Default,
		Top
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

	internal class BringIntoViewRequest<TItem> : BringIntoViewRequest
		where TItem : System.Windows.Controls.Control
	{
		public BringIntoViewRequest(TItem item, BringIntoViewMode mode = BringIntoViewMode.Default, int fallbackIndex = -1) : base(-1, mode, fallbackIndex)
		{
			Item = item;
		}

		public BringIntoViewRequest(int index, BringIntoViewMode mode = BringIntoViewMode.Default, int fallbackIndex = -1) : base(index, mode, fallbackIndex)
		{
		}

		internal override FrameworkElement ElementInternal => Item;

		public TItem Item { get; set; }
	}
}