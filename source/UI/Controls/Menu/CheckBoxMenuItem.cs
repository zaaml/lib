// <copyright file="CheckBoxMenuItem.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.PresentationCore.Theming;

namespace Zaaml.UI.Controls.Menu
{
	public class CheckBoxMenuItem : ToggleMenuItem
	{
		#region Ctors

		static CheckBoxMenuItem()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<CheckBoxMenuItem>();
		}

		public CheckBoxMenuItem()
		{
			this.OverrideStyleKey<CheckBoxMenuItem>();
			IsSubmenuEnabled = false;
		}

		#endregion
	}
}