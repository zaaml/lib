// <copyright file="SkinDictionaryCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using Zaaml.Core.Extensions;

namespace Zaaml.PresentationCore.Theming
{
	[TypeConverter(typeof(SkinDictionaryCollectionTypeConverter))]
	public sealed class SkinDictionaryCollection : Collection<SkinDictionary>
	{
		internal SkinDictionary Owner { get; set; }
	}

	public sealed class SkinDictionaryCollectionTypeConverter : TypeConverter
	{
		private static readonly char[] Delimiters = { ',', ' ', '|' };

		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			return sourceType == typeof(string) || typeof(SkinDictionary).IsAssignableFrom(sourceType);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			if (value is string strValue)
			{
				var result = new SkinDictionaryCollection();

				result.AddRange(strValue.Split(Delimiters, StringSplitOptions.RemoveEmptyEntries).Select(key => new SkinDictionary { DeferredKey = key }));

				return result;
			}

			if (value is SkinDictionary skinDictionary)
			{
				var result = new SkinDictionaryCollection();

				result.AddRange(skinDictionary);

				return result;
			}

			return null;
		}
	}
}