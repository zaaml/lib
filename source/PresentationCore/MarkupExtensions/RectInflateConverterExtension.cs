// <copyright file="RectInflateConverterExtension.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using Zaaml.PresentationCore.Converters;

namespace Zaaml.PresentationCore.MarkupExtensions
{
	public sealed class RectInflateConverterExtension : MarkupExtensionBase
	{
		public Thickness Inflate { get; set; }

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			return new RectInflateConverter
			{
				Inflate = Inflate
			};
		}
	}
}