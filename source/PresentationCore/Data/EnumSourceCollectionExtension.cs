// <copyright file="EnumSourceCollectionExtension.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using Zaaml.PresentationCore.MarkupExtensions;

namespace Zaaml.PresentationCore.Data
{
	public sealed class EnumSourceCollectionExtension : MarkupExtensionBase
	{
		public Type EnumType { get; set; }

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			return new EnumSourceCollection(EnumType);
		}
	}
}