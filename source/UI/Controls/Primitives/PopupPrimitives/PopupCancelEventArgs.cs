// <copyright file="PopupCancelEventArgs.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.ComponentModel;

namespace Zaaml.UI.Controls.Primitives.PopupPrimitives
{
	public sealed class PopupCancelEventArgs : CancelEventArgs
	{
		internal PopupCancelEventArgs(PopupCloseReason closeReason, bool cancel) : base(cancel)
		{
			CloseReason = closeReason;
		}

		internal PopupCloseReason CloseReason { get; }
	}
}