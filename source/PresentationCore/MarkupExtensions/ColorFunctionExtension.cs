// <copyright file="ColorFunctionExtension.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows.Media;

namespace Zaaml.PresentationCore.MarkupExtensions
{
	public abstract class ColorFunctionExtension : MarkupExtensionBase
	{
		public Color Color { get; set; }

		protected abstract Color Apply(Color color);

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			return Apply(Color);
		}
	}
}