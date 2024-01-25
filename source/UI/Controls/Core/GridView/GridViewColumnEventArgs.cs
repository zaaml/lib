// <copyright file="GridViewColumnEventArgs.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.UI.Controls.Core.GridView
{
	public class GridViewColumnEventArgs<TGridColumn> : EventArgs
		where TGridColumn : GridViewColumn
	{
		public GridViewColumnEventArgs(TGridColumn column)
		{
			Column = column;
		}

		public TGridColumn Column { get; }
	}
}