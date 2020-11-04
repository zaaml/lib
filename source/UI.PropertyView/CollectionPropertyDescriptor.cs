// <copyright file="CollectionPropertyDescriptor.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace Zaaml.UI.Controls.PropertyView
{
	public abstract class CollectionPropertyDescriptor<TTarget, TCollection, TItem> : PropertyDescriptor<TTarget, TCollection>, ICollectionPropertyItemFactory<TCollection> where TCollection : ICollection<TItem>
	{
		protected CollectionPropertyDescriptor(PropertyDescriptorProvider provider) : base(provider)
		{
		}

		private protected abstract CollectionPropertyItem<TCollection, TItem> CreateItemProperty(TCollection collection, TItem item, int index, PropertyItem parentItem);

		public abstract IEnumerable<TItem> GetItems(TCollection collection);

		IEnumerable<PropertyItem> ICollectionPropertyItemFactory<TCollection>.CreatePropertyItems(TCollection collection, PropertyItem parentItem)
		{
			if (collection == null)
				yield break;

			var index = 0;

			foreach (var item in GetItems(collection))
				yield return CreateItemProperty(collection, item, index++, parentItem);
		}
	}
}