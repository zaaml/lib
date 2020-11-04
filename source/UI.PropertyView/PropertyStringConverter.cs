// <copyright file="PropertyStringConverter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.UI.Controls.PropertyView
{
	public abstract class PropertyStringConverter
	{
		private protected PropertyStringConverter()
		{
		}

		public abstract string GetStringValue(PropertyItem propertyItem);

		public abstract void SetStringValue(PropertyItem propertyItem, string value);
	}

	public abstract class PropertyStringConverter<T> : PropertyStringConverter
	{
		public sealed override string GetStringValue(PropertyItem propertyItem)
		{
			return GetStringValue((PropertyItem<T>) propertyItem);
		}

		public abstract string GetStringValue(PropertyItem<T> propertyItem);

		public sealed override void SetStringValue(PropertyItem propertyItem, string value)
		{
			SetStringValue((PropertyItem<T>) propertyItem, value);
		}

		public abstract void SetStringValue(PropertyItem<T> propertyItem, string value);
	}
}