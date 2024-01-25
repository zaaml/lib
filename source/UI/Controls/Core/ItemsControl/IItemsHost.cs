// <copyright file="IItemsHost.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.UI.Panels;

namespace Zaaml.UI.Controls.Core
{
	internal interface IItemsHost<TItem>
		where TItem : FrameworkElement
	{
		ItemHostCollection<TItem> Items { get; }

		void BringIntoView(BringIntoViewRequest<TItem> request);

		void EnqueueBringIntoView(BringIntoViewRequest<TItem> request);

		ItemLayoutInformation GetLayoutInformation(int index);

		ItemLayoutInformation GetLayoutInformation(TItem item);
	}
}