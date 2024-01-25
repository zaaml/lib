// <copyright file="AccordionViewPanel.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.UI.Panels.Primitives;

namespace Zaaml.UI.Controls.AccordionView
{
	public class AccordionViewPanel : StackItemsPanelBase<AccordionViewItem>
	{
		protected override Size MeasureOverrideCore(Size availableSize)
		{
			return base.MeasureOverrideCore(availableSize);
		}
	}
}