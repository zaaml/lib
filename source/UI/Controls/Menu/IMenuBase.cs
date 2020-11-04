// <copyright file="IMenuBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.UI.Controls.Primitives.PopupPrimitives;

namespace Zaaml.UI.Controls.Menu
{
	internal interface IMenuBase : IMenuItemOwner, IMenuRoot
	{
		#region Properties

		bool IsOpen { get; set; }

		MenuController MenuController { get; }

		PopupControlController PopupController { get; }

		#endregion
	}

	public interface IMenuRoot
	{
	}
}