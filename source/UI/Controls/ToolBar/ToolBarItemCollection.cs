// <copyright file="ToolBarItemCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.ToolBar
{
	public sealed class ToolBarItemCollection : ItemCollectionBase<ToolBarControl, ToolBarItem>
	{
		#region Ctors

		internal ToolBarItemCollection(ToolBarControl toolBar) : base(toolBar)
		{
		}

		#endregion

		#region Properties

		protected override ItemGenerator<ToolBarItem> DefaultGenerator => throw new NotImplementedException();

		#endregion
	}
}