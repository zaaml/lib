// <copyright file="MouseEventPopupCloseReason.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.PresentationCore.Input;

namespace Zaaml.UI.Controls.Primitives.PopupPrimitives
{
	internal abstract class MouseEventPopupCloseReason : PopupCloseReason
	{
		public abstract MouseEventKind EventKind { get; }
	}
}