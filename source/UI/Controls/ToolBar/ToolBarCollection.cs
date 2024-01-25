// <copyright file="ToolBarCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using Zaaml.PresentationCore.ObservableCollections;

namespace Zaaml.UI.Controls.ToolBar
{
	public sealed class ToolBarCollection : DelegateDependencyObjectCollection<ToolBarControl>
	{
		internal ToolBarCollection(Action<ToolBarControl> onItemAdded, Action<ToolBarControl> onItemRemoved) : base(onItemAdded, onItemRemoved)
		{
		}
	}
}