// <copyright file="PrimitiveXamlPropertyStringConverter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.PresentationCore.Converters;

namespace Zaaml.UI.Controls.PropertyView
{
	internal sealed class PrimitiveXamlPropertyStringConverter<T> : PropertyStringConverter<T>
	{
		public override string GetStringValue(PropertyItem<T> propertyItem)
		{
			var xamlConvertResult = XamlStaticConverter.TryConvertValue(propertyItem.Value, typeof(string));

			return xamlConvertResult.IsValid ? (string) xamlConvertResult.Result : string.Empty;
		}

		public override void SetStringValue(PropertyItem<T> propertyItem, string value)
		{
			var xamlConvertResult = XamlStaticConverter.TryConvertValue(value, typeof(T));

			if (xamlConvertResult.IsValid)
				propertyItem.Value = (T) xamlConvertResult.Result;
			else
				throw xamlConvertResult.Exception.InnerException ?? xamlConvertResult.Exception;
		}
	}
}