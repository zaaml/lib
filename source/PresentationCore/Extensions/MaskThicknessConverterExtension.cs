// <copyright file="MaskThicknessConverterExtension.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using Zaaml.PresentationCore.Converters;
using Zaaml.PresentationCore.MarkupExtensions;

namespace Zaaml.PresentationCore.Extensions
{
	public sealed class MaskThicknessConverterExtension : MarkupExtensionBase
	{
		public bool Bottom
		{
			get => Converter.Bottom;
			set => Converter.Bottom = value;
		}

		private MaskThicknessConverter Converter { get; } = new MaskThicknessConverter();

		public bool Left
		{
			get => Converter.Left;
			set => Converter.Left = value;
		}

		public bool Right
		{
			get => Converter.Right;
			set => Converter.Right = value;
		}

		public bool Top
		{
			get => Converter.Top;
			set => Converter.Top = value;
		}

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			return Converter;
		}
	}
}