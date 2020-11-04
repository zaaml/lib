// <copyright file="MouseButtonEventPopupCloseReason.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows.Input;
using Zaaml.PresentationCore.Input;

namespace Zaaml.UI.Controls.Primitives.PopupPrimitives
{
	internal sealed class MouseButtonEventPopupCloseReason : MouseEventPopupCloseReason
	{
		public MouseButtonEventPopupCloseReason(MouseButtonEventArgs eventArgs):this(MouseUtils.FromMouseButton(eventArgs.ChangedButton), MouseUtils.FromButtonState(eventArgs.ButtonState), eventArgs)
		{
		}

		public MouseButtonEventPopupCloseReason(MouseButtonKind buttonKind, MouseButtonEventKind buttonEventKind, MouseButtonEventArgs eventArgs)
		{
			ButtonKind = buttonKind;
			ButtonEventKind = buttonEventKind;
			EventArgs = eventArgs;
		}

		public MouseButtonEventKind ButtonEventKind { get; }

		public MouseButtonKind ButtonKind { get; }

		public MouseButtonEventArgs EventArgs { get; }

		public override MouseEventKind EventKind => MouseEventKind.Button;
	}
}