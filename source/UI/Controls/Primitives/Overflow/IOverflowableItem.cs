// <copyright file="IOverflowItem.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows.Controls;

namespace Zaaml.UI.Controls.Primitives.Overflow
{
	public interface IOverflowableItem<TItem> : IOverflowableItem
		where TItem : Control, IOverflowableItem
	{
		#region Properties

		OverflowItemController<TItem> OverflowController { get; }

		#endregion
	}

	public interface IOverflowableItem
	{
		#region Properties

		bool IsOverflow { get; }

		#endregion
	}
}