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

		protected override void OnItemAdded(TableViewDefinition tableViewDefinition)
		{
			tableViewDefinition.TableViewControl = TableViewControl;

			base.OnItemAdded(tableViewDefinition);

			TableViewControl.OnDefinitionsChanged();
		}

		protected override void OnItemRemoved(TableViewDefinition tableViewDefinition)
		{
			tableViewDefinition.TableViewControl = null;

			base.OnItemRemoved(tableViewDefinition);

			TableViewControl.OnDefinitionsChanged();
		}
	}
}