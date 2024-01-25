// <copyright file="DefaultListGridViewCellGenerator.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.UI.Controls.Core.GridView;

namespace Zaaml.UI.Controls.ListView
{
	internal sealed class DefaultListGridViewCellGenerator : ListGridViewCellGeneratorBase
	{
		public DefaultListGridViewCellGenerator(ListGridViewColumn listGridViewColumn)
		{
			ListGridViewColumn = listGridViewColumn;
			MemberProperty = new GridViewCellGeneratorDisplayMemberProperty<ListGridViewCell>(this);
			DisplayTemplateProperty = new GridViewCellGeneratorDisplayTemplateProperty<ListGridViewCell>(this);
			EditTemplateProperty = new GridViewCellGeneratorEditTemplateProperty<ListGridViewCell>(this);
			StyleProperty = new GridViewCellGeneratorCellStyleProperty<ListGridViewCell>(this);
			EditModeTriggerProperty = new GridViewCellGeneratorTargetDependencyProperty<ListGridViewCell, GridViewCellEditModeTrigger>(this, GridViewCell.EditModeTriggerProperty);

			Implementation = new GridViewCellGeneratorImplementation<ListGridViewCell>(StyleProperty, MemberProperty, DisplayTemplateProperty, EditTemplateProperty, EditModeTriggerProperty);
		}

		public GridViewCellGeneratorDisplayTemplateProperty<ListGridViewCell> DisplayTemplateProperty { get; }

		public GridViewCellGeneratorTargetDependencyProperty<ListGridViewCell, GridViewCellEditModeTrigger> EditModeTriggerProperty { get; }

		public GridViewCellGeneratorEditTemplateProperty<ListGridViewCell> EditTemplateProperty { get; }

		private GridViewCellGeneratorImplementation<ListGridViewCell> Implementation { get; }

		public ListGridViewColumn ListGridViewColumn { get; }

		public GridViewCellGeneratorDisplayMemberProperty<ListGridViewCell> MemberProperty { get; }

		public GridViewCellGeneratorCellStyleProperty<ListGridViewCell> StyleProperty { get; }

		protected override ListGridViewCell CreateCell()
		{
			return Implementation.CreateCell();
		}

		protected override void DisposeCell(ListGridViewCell item)
		{
			Implementation.DisposeCell(item);
		}
	}
}