// <copyright file="ItemCollectionBase.IEnumerable.Generic.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace Zaaml.UI.Controls.Core
{
	public abstract partial class ItemCollectionBase<TItemsControl, TItem>
	{
		#region Interface Implementations

		#region IEnumerable<TItem>

		IEnumerator<TItem> IEnumerable<TItem>.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion

		#endregion
	}
}