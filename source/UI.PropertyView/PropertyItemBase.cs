// <copyright file="PropertyItemBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.UI.Controls.PropertyView
{
	public abstract class PropertyItemBase
	{
		public abstract string Description { get; }

		public abstract string DisplayName { get; }

		public bool IsExpanded { get; set; }
	}
}