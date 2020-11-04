// <copyright file="Button.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) zaaml. All rights reserved.
// </copyright>

using Zaaml.PresentationCore.Theming;

namespace Zaaml.UI.Controls.Primitives
{
	public class Button : ButtonBase
	{
		#region Ctors

	  static Button()
	  {
	    DefaultStyleKeyHelper.OverrideStyleKey<Button>();
	  }

    public Button()
		{
			this.OverrideStyleKey<Button>();
		}

		#endregion
	}
}