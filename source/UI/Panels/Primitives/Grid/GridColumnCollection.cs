// <copyright file="ColumnDefinitionCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.UI.Panels.Primitives
{
	public sealed class GridColumnCollection : GridDefinitionCollection<GridColumn>
	{
		internal GridColumnCollection(GridPanel gridPanel) : base(gridPanel)
		{
		}
	}
}