// <copyright file="ObjectExtensions.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using Zaaml.PresentationCore.Converters;

namespace Zaaml.PresentationCore.Extensions
{
	internal static class ObjectExtensions
	{
		public static object XamlConvert(this object value, Type targetType)
		{
			return XamlStaticConverter.ConvertValue(value, targetType);
		}

		public static bool XamlTryConvert(this object value, Type targetType, out object convertedValue)
		{
			var xamlConvertResult = XamlStaticConverter.TryConvertValue(value, targetType);

			if (xamlConvertResult.IsValid)
			{
				convertedValue = xamlConvertResult.Result;

				return true;
			}

			convertedValue = default;

			return false;
		}
	}
}