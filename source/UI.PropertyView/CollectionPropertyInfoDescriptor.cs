// <copyright file="CollectionPropertyInfoDescriptor.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Reflection;

namespace Zaaml.UI.Controls.PropertyView
{
	public class CollectionPropertyInfoDescriptor<TTarget, TCollection, TItem> : CollectionPropertyDescriptor<TTarget, TCollection, TItem> where TCollection : ICollection<TItem>
	{
		private static readonly TCollection DefaultValue = default;
		private readonly Func<TTarget, TCollection> _getter;
		private readonly Func<TTarget, TCollection, TTarget> _setter;

		public CollectionPropertyInfoDescriptor(PropertyInfo propertyInfo, PropertyDescriptorProvider provider) : base(provider)
		{
			PropertyInfo = propertyInfo;

			IsReadOnly = propertyInfo.CanWrite == false;
			Name = PropertyInfoDescriptor.GetName(propertyInfo, provider);
			DisplayName = PropertyInfoDescriptor.GetDescription(propertyInfo, provider);
			Category = PropertyInfoDescriptor.GetCategory(propertyInfo, provider);
			Description = PropertyInfoDescriptor.GetDescription(propertyInfo, provider);

			ItemPropertyDescriptor = new CollectionItemPropertyDescriptor<TCollection, TItem>(provider);

			_getter = PropertyInfoDescriptor.CreatePropertyGetter<TTarget, TCollection>(propertyInfo);
			_setter = propertyInfo.CanWrite ? PropertyInfoDescriptor.CreatePropertySetter<TTarget, TCollection>(propertyInfo) : null;
		}

		public override string Category { get; }

		public override string Description { get; }

		public override string DisplayName { get; }

		public override bool IsReadOnly { get; }

		private CollectionItemPropertyDescriptor<TCollection, TItem> ItemPropertyDescriptor { get; }

		public override string Name { get; }

		public PropertyInfo PropertyInfo { get; }

		private protected override CollectionPropertyItem<TCollection, TItem> CreateItemProperty(TCollection collection, TItem item, int index, PropertyItem parentItem)
		{
			return new CollectionPropertyItem<TCollection, TItem>(collection, item, index, ItemPropertyDescriptor, parentItem);
		}

		public override IEnumerable<TItem> GetItems(TCollection collection)
		{
			return collection;
		}

		public override TCollection GetValue(TTarget component)
		{
			return _getter(component);
		}

		public override TTarget ResetValue(TTarget component)
		{
			if (IsReadOnly)
				throw new NotSupportedException();

			return _setter(component, DefaultValue);
		}

		public override TTarget SetValue(TTarget component, TCollection value)
		{
			if (_setter == null)
				throw new NotSupportedException();

			return _setter(component, value);
		}
	}
}