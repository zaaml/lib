// <copyright file="Label.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.PresentationCore.Theming;
using Zaaml.PresentationCore.Utils;
using ZaamlContentControl = Zaaml.UI.Controls.Core.ContentControl;

namespace Zaaml.UI.Controls.Primitives
{
	public class Label : ZaamlContentControl
	{
		static Label()
		{
			ControlUtils.OverrideIsTabStop<Label>(false);

			DefaultStyleKeyHelper.OverrideStyleKey<Label>();
		}

		public Label()
		{
			this.OverrideStyleKey<Label>();
		}
	}
}