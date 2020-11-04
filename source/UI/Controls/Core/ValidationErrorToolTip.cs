// <copyright file="ValidationErrorToolTip.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Controls.Primitives.PopupPrimitives;

namespace Zaaml.UI.Controls.Core
{
	public class ValidationErrorToolTip : PopupBar
	{
		static ValidationErrorToolTip()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<ValidationErrorToolTip>();
		}

		public ValidationErrorToolTip()
		{
			this.OverrideStyleKey<ValidationErrorToolTip>();
		}
	}
}