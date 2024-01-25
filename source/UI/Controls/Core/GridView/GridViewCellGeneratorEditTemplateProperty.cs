// <copyright file="EditTemplateGridCellGeneratorProperty.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;

namespace Zaaml.UI.Controls.Core.GridView
{
	internal sealed class GridViewCellGeneratorEditTemplateProperty<TGridCell>
		: GridViewCellGeneratorTargetDependencyProperty<TGridCell, DataTemplate>
		where TGridCell : GridViewCell
	{
		public GridViewCellGeneratorEditTemplateProperty(GridViewCellGenerator<TGridCell> generator)
			: base(generator, GridViewCell.EditContentTemplateProperty)
		{
		}
	}
}