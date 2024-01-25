// <copyright file="PropertyDescriptor.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.UI.Controls.PropertyView
{
	public abstract class PropertyDescriptor
	{
		private protected PropertyDescriptor(PropertyDescriptorProvider provider)
		{
			Provider = provider;
		}

		public abstract string Category { get; }

		public abstract string Description { get; }

		public abstract string DisplayName { get; }

		public abstract bool IsReadOnly { get; }

		public abstract string Name { get; }

		public abstract Type PropertyType { get; }

		public PropertyDescriptorProvider Provider { get; }
	}

	public abstract class PropertyDescriptor<TValue> : PropertyDescriptor
	{
		private protected PropertyDescriptor(PropertyDescriptorProvider provider) : base(provider)
		{
		}
	}

	public abstract class PropertyDescriptor<TTarget, TValue> : PropertyDescriptor<TValue>, IPropertyItemFactory
	{
		protected PropertyDescriptor(PropertyDescriptorProvider provider) : base(provider)
		{
		}

		public sealed override Type PropertyType => typeof(TValue);

		public abstract TValue GetValue(TTarget propertyObject);

		public abstract TTarget ResetValue(TTarget propertyObject);

		public abstract TTarget SetValue(TTarget propertyObject, TValue value);

		PropertyItem IPropertyItemFactory.CreatePropertyItem(object propertyObject, PropertyItem parentItem)
		{
			return new PropertyItem<TTarget, TValue>((TTarget) propertyObject, this, parentItem);
		}
	}
}