// <copyright file="TableViewDefinitionCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.PresentationCore;

namespace Zaaml.UI.Controls.TableView
{
	public class TableViewDefinitionCollection : InheritanceContextDependencyObjectCollection<TableViewDefinition>
	{
		internal TableViewDefinitionCollection(TableViewControl tableViewControl)
		{
			TableViewControl = tableViewControl;
		}

		public TableViewControl TableViewControl { get; }
	}
}