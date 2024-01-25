// <copyright file="SkinTypeConverter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.ComponentModel;
using System.Globalization;

namespace Zaaml.PresentationCore.Theming
{
	public sealed class SkinTypeConverter : TypeConverter
	{
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			return sourceType == typeof(string) || sourceType.IsAssignableFrom(typeof(SkinDictionary));
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			return value switch
			{
				string strValue => new DeferSkin { Key = strValue },
				SkinDictionary skinDictionary => ThemeManager.GetSkin(skinDictionary),
				_ => null
			};
		}
	}
}