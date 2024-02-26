// <copyright file="TemplateSkinBindingExtension.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows.Data;

namespace Zaaml.PresentationCore.Data.MarkupExtensions
{
	public sealed class TemplateSkinBindingExtension : RelativeSkinBindingExtension
	{
		protected override RelativeSource RelativeSource => XamlConstants.TemplatedParent;
	}
}