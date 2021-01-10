// <copyright file="GridRowCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.UI.Panels.Primitives
{
	public sealed class GridRowCollection : GridDefinitionCollection<GridRow>
	{
		internal GridRowCollection(GridPanel gridPanel) : base(gridPanel)
		{
		}
	}
}