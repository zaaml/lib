// <copyright file="EmptyPropertyInfoDescriptor.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Reflection;

namespace Zaaml.UI.Controls.PropertyView
{
	public sealed class EmptyPropertyInfoDescriptor : PropertyDescriptor
	{
		internal EmptyPropertyInfoDescriptor(PropertyInfo propertyInfo, PropertyDescriptorProvider provider) : base(provider)
		{
			PropertyInfo = propertyInfo;

			IsReadOnly = propertyInfo.CanWrite == false;
			Name = PropertyInfoDescriptor.GetName(propertyInfo, provider);
			DisplayName = PropertyInfoDescriptor.GetDescription(propertyInfo, provider);
			Category = PropertyInfoDescriptor.GetCategory(propertyInfo, provider);
			Description = PropertyInfoDescriptor.GetDescription(propertyInfo, provider);
		}

		public override string Category { get; }

		public override string Description { get; }

		public override string DisplayName { get; }

		public override bool IsReadOnly { get; }

		public override string Name { get; }

		public PropertyInfo PropertyInfo { get; }

		public override Type PropertyType => PropertyInfo.PropertyType;
	}
}