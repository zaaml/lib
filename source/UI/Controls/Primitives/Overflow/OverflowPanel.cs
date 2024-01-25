// <copyright file="OverflowPanel.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.PresentationCore;
using Zaaml.UI.Panels.Core;

namespace Zaaml.UI.Controls.Primitives.Overflow
{
	public sealed class OverflowPanel : Panel
	{
		internal IOverflowItem Item { get; set; }

		protected override Size MeasureOverrideCore(Size availableSize)
		{
			return Item?.MeasurePanel(availableSize) ?? XamlConstants.ZeroSize;
		}
	}
}