// <copyright file="CollectionItemPropertyDescriptor.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;

namespace Zaaml.UI.Controls.PropertyView
{
	public class CollectionItemPropertyDescriptor<TCollection, TItem> : PropertyDescriptor<TItem> where TCollection : ICollection<TItem>
	{
		internal CollectionItemPropertyDescriptor(PropertyDescriptorProvider provider) : base(provider)
		{
		}

		public override string Category => null;

		public override string Description => null;

		public override string DisplayName => Name;

		public override bool IsReadOnly => true;

		public override string Name => "Item";

		public override Type PropertyType => typeof(TItem);

		public virtual TItem GetItem(TCollection collection, int index)
		{
			throw new NotSupportedException();
		}

		public virtual void SetItem(TCollection collection, int index, TItem item)
		{
			throw new NotSupportedException();
		}
	}
}