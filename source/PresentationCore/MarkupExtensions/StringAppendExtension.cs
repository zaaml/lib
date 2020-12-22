// <copyright file="StringAppendExtension.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using Zaaml.PresentationCore.Converters;

namespace Zaaml.PresentationCore.MarkupExtensions
{
	public sealed class StringAppendConverterExtension : MarkupExtensionBase
	{
		public string Value { get; set; }

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			return new StringAppendConverter {Value = Value};
		}
	}
}