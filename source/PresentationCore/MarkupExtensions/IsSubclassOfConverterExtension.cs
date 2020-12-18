// <copyright file="IsSubclassOfConverterExtension.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using Zaaml.PresentationCore.Converters;

namespace Zaaml.PresentationCore.MarkupExtensions
{
	public class IsSubclassOfConverterExtension : MarkupExtensionBase
	{
		public bool Self { get; set; } = true;

		public Type Type { get; set; }

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			return new IsSubclassOfConverter { Type = Type, Self = Self };
		}
	}
}