// <copyright file="AncestorSkinBindingExtension.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows.Data;

namespace Zaaml.PresentationCore.Data.MarkupExtensions
{
	public sealed class AncestorSkinBindingExtension : RelativeSkinBindingExtension
	{
		public int AncestorLevel { get; set; } = 1;

		public Type AncestorType { get; set; }

		protected override RelativeSource RelativeSource => new(RelativeSourceMode.FindAncestor)
		{
			AncestorType = AncestorType,
			AncestorLevel = AncestorLevel
		};
	}
}