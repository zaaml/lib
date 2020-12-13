// <copyright file="TableViewItemCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.TableView
{
	public class TableViewItemCollection : ItemCollectionBase<TableViewControl, TableViewItem>
	{
		internal TableViewItemCollection(TableViewControl control) : base(control)
		{
		}

		protected override ItemGenerator<TableViewItem> DefaultGenerator { get; } = new TableViewItemGenerator();


		internal TableViewItemGenerator Generator
		{
			get => (TableViewItemGenerator)GeneratorCore;
			set => GeneratorCore = value;
		}
	}
}