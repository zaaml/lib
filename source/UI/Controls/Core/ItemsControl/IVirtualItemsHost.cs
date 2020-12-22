// <copyright file="IVirtualItemsHost.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;

namespace Zaaml.UI.Controls.Core
{
	internal interface IVirtualItemsHost
	{
		#region Properties

		bool IsVirtualizing { get; }

		IVirtualItemCollection VirtualSource { get; set; }

		void OnItemAttaching(UIElement element);

		void OnItemAttached(UIElement element);

		void OnItemDetaching(UIElement element);

		void OnItemDetached(UIElement element);

		#endregion
	}

	internal interface IVirtualItemsHost<TItem> : IItemsHost<TItem>, IVirtualItemsHost
		where TItem : FrameworkElement
	{
	}
}