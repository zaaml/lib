// <copyright file="PopupControlScope.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.UI.Controls.Primitives.PopupPrimitives
{
	public class PopupControlScope
	{
		private static readonly Lazy<PopupControlScope> LazyInstance = new(() => new PopupControlScope());

		public static PopupControlScope Shared => LazyInstance.Value;
	}
}