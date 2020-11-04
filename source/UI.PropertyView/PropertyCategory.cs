// <copyright file="PropertyCategory.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace Zaaml.UI.Controls.PropertyView
{
	public sealed class PropertyCategory : PropertyItemBase
	{
		public PropertyCategory(string displayName, string description, IReadOnlyCollection<PropertyItem> propertyItems)
		{
			DisplayName = displayName;
			Description = description;
			PropertyItems = propertyItems;
		}

		public override string Description { get; }

		public override string DisplayName { get; }

		public IReadOnlyCollection<PropertyItem> PropertyItems { get; }
	}
}