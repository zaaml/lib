// <copyright file="PrimitivePropertyStringConverter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.Core.Converters;

namespace Zaaml.UI.Controls.PropertyView
{
	internal sealed class PrimitivePropertyStringConverter<T> : PropertyStringConverter<T>
	{
		private readonly IPrimitiveConverter<T, string> _fromConverter;
		private readonly IPrimitiveConverter<string, T> _toConverter;

		public PrimitivePropertyStringConverter(IPrimitiveConverter<T, string> fromConverter, IPrimitiveConverter<string, T> toConverter)
		{
			_fromConverter = fromConverter;
			_toConverter = toConverter;
		}

		public override string GetStringValue(PropertyItem<T> propertyItem)
		{
			return _fromConverter.Convert(propertyItem.Value);
		}

		public override void SetStringValue(PropertyItem<T> propertyItem, string value)
		{
			propertyItem.Value = _toConverter.Convert(value);
		}
	}
}