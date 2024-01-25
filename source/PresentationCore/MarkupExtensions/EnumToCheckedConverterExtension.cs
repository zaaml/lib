// <copyright file="EnumToCheckedConverterExtension.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using Zaaml.PresentationCore.Converters;

namespace Zaaml.PresentationCore.MarkupExtensions
{
	public sealed class EnumBoolConverterExtension : MarkupExtensionBase
	{
		public object FalseEnumValue { get; set; }

		public object TrueEnumValue { get; set; }

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			return new EnumBoolConverter { TrueEnumValue = TrueEnumValue, FalseEnumValue = FalseEnumValue };
		}
	}
}