// <copyright file="SplitButton.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.PresentationCore.Theming;

namespace Zaaml.UI.Controls.DropDown
{
	public class SplitButton : SplitButtonBase
	{
		static SplitButton()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<SplitButton>();
		}

		public SplitButton()
		{
			this.OverrideStyleKey<SplitButton>();
		}
	}
}