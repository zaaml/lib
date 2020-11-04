// <copyright file="IDropDownControlHost.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.UI.Controls.Primitives.PopupPrimitives;

namespace Zaaml.UI.Controls.DropDown
{
	internal interface IDropDownControlHost
	{
		object LogicalChild { get; set; }

		PopupControlBase PopupControl { get; }

		PopupControlHost PopupHost { get; }
	}
}