// <copyright file="ItemCommandParameterSelector.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.UI.Controls.Core
{
	public abstract class ItemCommandParameterSelector<TItem>
	{
		public abstract object SelectCommandParameter(TItem item);
	}
}