// <copyright file="GridDefinitionCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.PresentationCore;

namespace Zaaml.UI.Panels.Primitives
{
	public abstract class GridDefinitionCollection<TDefinition> : InheritanceContextDependencyObjectCollection<TDefinition> where TDefinition : GridPanelDefinition
	{
		protected GridDefinitionCollection(GridPanel gridPanel)
		{
			GridPanel = gridPanel;
		}

		public GridPanel GridPanel { get; }

		protected override void OnItemAdded(TDefinition definition)
		{
			base.OnItemAdded(definition);

			definition.GridPanel = GridPanel;

			UpdateIndices();
		}

		protected override void OnItemRemoved(TDefinition definition)
		{
			definition.GridPanel = null;

			base.OnItemRemoved(definition);

			UpdateIndices();
		}

		private void UpdateIndices()
		{
			for (var index = 0; index < Count; index++)
				this[index].Index = index;
		}
	}
}