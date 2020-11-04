// <copyright file="ExplicitCloseReason.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.UI.Controls.Primitives.PopupPrimitives
{
	internal sealed class ExplicitCloseReason : PopupCloseReason
	{
		public static readonly PopupCloseReason Instance = new ExplicitCloseReason();

		private ExplicitCloseReason()
		{
		}
	}
}