// <copyright file="StringConverter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Globalization;
using Zaaml.Core.Extensions;
using Zaaml.Core.Runtime;

namespace Zaaml.PresentationCore.Converters
{
	public sealed class StringConverter : BaseValueConverter
	{
		public static readonly StringConverter IsNullOrEmpty = new(Kind.IsNullOrEmpty);
		public static readonly StringConverter IsNullOrWhiteSpace = new(Kind.IsNullOrWhiteSpace);

		private readonly Kind _kind;

		private StringConverter(Kind kind)
		{
			_kind = kind;
		}

		protected override object ConvertBackCore(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new InvalidOperationException("StringConverter can only be used OneWay.");
		}


		protected override object ConvertCore(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var stringValue = (string)value;

			switch (_kind)
			{
				case Kind.IsNullOrEmpty:
					return stringValue.IsNullOrEmpty() ? BooleanBoxes.True : BooleanBoxes.False;
				case Kind.IsNullOrWhiteSpace:
					return stringValue.IsNullOrWhiteSpace() ? BooleanBoxes.True : BooleanBoxes.False;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private enum Kind
		{
			IsNullOrEmpty,
			IsNullOrWhiteSpace
		}
	}
}