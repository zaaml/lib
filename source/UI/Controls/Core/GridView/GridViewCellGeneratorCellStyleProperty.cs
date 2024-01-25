// <copyright file="CellStyleGridCellGeneratorProperty.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;

namespace Zaaml.UI.Controls.Core.GridView
{
	internal sealed class GridViewCellGeneratorCellStyleProperty<TGridCell> : GridViewCellGeneratorTargetDependencyProperty<TGridCell, Style>
		where TGridCell : GridViewCell
	{
		public GridViewCellGeneratorCellStyleProperty(GridViewCellGenerator<TGridCell> generator) : base(generator, FrameworkElement.StyleProperty)
		{
		}
	}
}