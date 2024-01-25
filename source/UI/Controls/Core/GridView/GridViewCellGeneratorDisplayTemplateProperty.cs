// <copyright file="DisplayTemplateGridCellGeneratorProperty.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;

namespace Zaaml.UI.Controls.Core.GridView
{
	internal sealed class GridViewCellGeneratorDisplayTemplateProperty<TGridCell>
		: GridViewCellGeneratorTargetDependencyProperty<TGridCell, DataTemplate>
		where TGridCell : GridViewCell
	{
		public GridViewCellGeneratorDisplayTemplateProperty(GridViewCellGenerator<TGridCell> generator)
			: base(generator, GridViewCell.DisplayContentTemplateProperty)
		{
		}
	}
}