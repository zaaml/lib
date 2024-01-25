// <copyright file="PropertyGridViewCell.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Controls.Core;
using Zaaml.UI.Controls.Core.GridView;

namespace Zaaml.UI.Controls.PropertyView
{
	[TemplateContractType(typeof(PropertyGridViewCellTemplateContract))]
	public class PropertyGridViewCell
		: GridViewCell<PropertyGridViewColumn, PropertyGridViewCellsPresenter,
			PropertyGridViewCellsPanel,
			PropertyGridViewCellCollection,
			PropertyGridViewCell>
	{
		static PropertyGridViewCell()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<PropertyGridViewCell>();
		}

		public PropertyGridViewCell()
		{
			this.OverrideStyleKey<PropertyGridViewCell>();
		}

		protected override GridViewColumn GridColumnCore => GridColumn ?? ColumnControllerInternal?.GetColumn(CellIndex);
	}
}