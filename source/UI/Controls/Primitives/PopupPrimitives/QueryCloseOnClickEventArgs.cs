// <copyright file="QueryCloseOnClickEventArgs.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.UI.Controls.Primitives.PopupPrimitives
{
	internal sealed class QueryCloseOnClickEventArgs : EventArgs
	{
		public QueryCloseOnClickEventArgs(Popup popup, object mouseEventSource)
		{
			Popup = popup;
			MouseEventSource = mouseEventSource;
		}

		public bool CanClose { get; set; } = true;

		public object MouseEventSource { get; }

		public Popup Popup { get; }
	}
}