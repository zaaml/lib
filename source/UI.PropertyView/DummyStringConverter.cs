// <copyright file="DummyStringConverter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.UI.Controls.PropertyView
{
	internal sealed class DummyStringConverter : PropertyStringConverter<string>
	{
		public static readonly PropertyStringConverter<string> Instance = new DummyStringConverter();

		private DummyStringConverter()
		{
		}

		public override string GetStringValue(PropertyItem<string> propertyItem)
		{
			return propertyItem.Value;
		}

		public override void SetStringValue(PropertyItem<string> propertyItem, string value)
		{
			propertyItem.Value = value;
		}
	}
}