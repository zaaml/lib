// <copyright file="DefaultTreeGridViewCellGenerator.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.UI.Controls.Core.GridView;

namespace Zaaml.UI.Controls.TreeView
{
	internal sealed class DefaultTreeGridViewCellGenerator : TreeGridViewCellGeneratorBase
	{
		public DefaultTreeGridViewCellGenerator(TreeGridViewColumn column)
		{
			Column = column;
			MemberProperty = new GridViewCellGeneratorDisplayMemberProperty<TreeGridViewCell>(this);
			DisplayTemplateProperty = new GridViewCellGeneratorDisplayTemplateProperty<TreeGridViewCell>(this);
			EditTemplateProperty = new GridViewCellGeneratorEditTemplateProperty<TreeGridViewCell>(this);
			StyleProperty = new GridViewCellGeneratorCellStyleProperty<TreeGridViewCell>(this);
			EditModeTriggerProperty = new GridViewCellGeneratorTargetDependencyProperty<TreeGridViewCell, GridViewCellEditModeTrigger>(this, GridViewCell.EditModeTriggerProperty);

			Implementation = new GridViewCellGeneratorImplementation<TreeGridViewCell>(StyleProperty, MemberProperty, DisplayTemplateProperty, EditTemplateProperty, EditModeTriggerProperty);
		}

		public TreeGridViewColumn Column { get; }

		public GridViewCellGeneratorDisplayTemplateProperty<TreeGridViewCell> DisplayTemplateProperty { get; }

		public GridViewCellGeneratorTargetDependencyProperty<TreeGridViewCell, GridViewCellEditModeTrigger> EditModeTriggerProperty { get; }

		public GridViewCellGeneratorEditTemplateProperty<TreeGridViewCell> EditTemplateProperty { get; }

		private GridViewCellGeneratorImplementation<TreeGridViewCell> Implementation { get; }

		public GridViewCellGeneratorDisplayMemberProperty<TreeGridViewCell> MemberProperty { get; }

		public GridViewCellGeneratorCellStyleProperty<TreeGridViewCell> StyleProperty { get; }

		protected override TreeGridViewCell CreateCell()
		{
			return Implementation.CreateCell();
		}

		protected override void DisposeCell(TreeGridViewCell item)
		{
			Implementation.DisposeCell(item);
		}
	}
}