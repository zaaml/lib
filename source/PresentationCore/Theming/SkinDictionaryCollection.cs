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
		private static readonly char[] Delimiters = {',', ' ', '|'};

		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			return sourceType == typeof(string);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			var strValue = value as string;

			if (strValue == null)
				throw new InvalidOperationException("Expected string value");

			var result = new SkinDictionaryCollection();

			result.AddRange(strValue.Split(Delimiters, StringSplitOptions.RemoveEmptyEntries).Select(key => new SkinDictionary {DeferredKey = key}));

			return result;
		}
	}
}