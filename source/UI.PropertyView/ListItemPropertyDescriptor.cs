// <copyright file="ListItemPropertyDescriptor.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace Zaaml.UI.Controls.PropertyView
{
	public class ListItemPropertyDescriptor<TCollection, TItem> : CollectionItemPropertyDescriptor<TCollection, TItem> where TCollection : IList<TItem>
	{
		internal ListItemPropertyDescriptor(PropertyDescriptorProvider provider) : base(provider)
		{
		}

		public override bool IsReadOnly => false;

		public override TItem GetItem(TCollection collection, int index)
		{
			return collection[index];
		}

		public override void SetItem(TCollection collection, int index, TItem item)
		{
			collection[index] = item;
		}
	}
}