// <copyright file="DefaultTreeGridViewHeaderGenerator.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.UI.Controls.Core.GridView;

namespace Zaaml.UI.Controls.TreeView
{
	internal sealed class DefaultTreeGridViewHeaderGenerator : TreeGridViewHeaderGeneratorBase
	{
		public DefaultTreeGridViewHeaderGenerator(TreeGridViewColumn column)
		{
			Column = column;
			MemberProperty = new GridViewCellGeneratorDisplayMemberProperty<TreeGridViewHeader>(this);
			TemplateProperty = new GridViewCellGeneratorDisplayTemplateProperty<TreeGridViewHeader>(this);
			StyleProperty = new GridViewCellGeneratorCellStyleProperty<TreeGridViewHeader>(this);

			Implementation = new GridViewCellGeneratorImplementation<TreeGridViewHeader>(MemberProperty, TemplateProperty, StyleProperty);
		}

		public TreeGridViewColumn Column { get; }

		private GridViewCellGeneratorImplementation<TreeGridViewHeader> Implementation { get; }

		public GridViewCellGeneratorDisplayMemberProperty<TreeGridViewHeader> MemberProperty { get; }

		public GridViewCellGeneratorCellStyleProperty<TreeGridViewHeader> StyleProperty { get; }

		public GridViewCellGeneratorDisplayTemplateProperty<TreeGridViewHeader> TemplateProperty { get; }

		protected override TreeGridViewHeader CreateCell()
		{
			return Implementation.CreateCell();
		}

		protected override void DisposeCell(TreeGridViewHeader item)
		{
			Implementation.DisposeCell(item);
		}
	}
}