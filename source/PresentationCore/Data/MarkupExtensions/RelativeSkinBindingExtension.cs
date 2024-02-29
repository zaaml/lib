// <copyright file="RelativeSkinBindingExtension.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows.Data;

namespace Zaaml.PresentationCore.Data.MarkupExtensions
{
	public abstract class RelativeSkinBindingExtension : SkinBindingBaseExtension
	{
		protected abstract RelativeSource RelativeSource { get; }

		protected sealed override void SetBindingSource(System.Windows.Data.Binding binding)
		{
			binding.Path = ActualSkinPropertyPath;
			binding.RelativeSource = RelativeSource;
		}
	}
}