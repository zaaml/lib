// <copyright file="IOverflowItem.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;

namespace Zaaml.UI.Controls.Primitives.Overflow
{
	internal interface IOverflowItem
	{
		Size MeasurePanel(Size availableSize);
	}
}