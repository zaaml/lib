// <copyright file="DefaultListGridViewHeaderGenerator.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.UI.Controls.Core.GridView;

namespace Zaaml.UI.Controls.ListView
{
	internal sealed class DefaultListGridViewHeaderGenerator : ListGridViewHeaderGeneratorBase
	{
		public DefaultListGridViewHeaderGenerator(ListGridViewColumn column)
		{
			Column = column;
			MemberProperty = new GridViewCellGeneratorDisplayMemberProperty<ListGridViewHeader>(this);
			TemplateProperty = new GridViewCellGeneratorDisplayTemplateProperty<ListGridViewHeader>(this);
			StyleProperty = new GridViewCellGeneratorCellStyleProperty<ListGridViewHeader>(this);

			Implementation = new GridViewCellGeneratorImplementation<ListGridViewHeader>(MemberProperty, TemplateProperty, StyleProperty);
		}

		public ListGridViewColumn Column { get; }

		private GridViewCellGeneratorImplementation<ListGridViewHeader> Implementation { get; }

		public GridViewCellGeneratorDisplayMemberProperty<ListGridViewHeader> MemberProperty { get; }

		public GridViewCellGeneratorCellStyleProperty<ListGridViewHeader> StyleProperty { get; }

		public GridViewCellGeneratorDisplayTemplateProperty<ListGridViewHeader> TemplateProperty { get; }

		protected override ListGridViewHeader CreateCell()
		{
			return Implementation.CreateCell();
		}

		protected override void DisposeCell(ListGridViewHeader item)
		{
			Implementation.DisposeCell(item);
		}
	}
}