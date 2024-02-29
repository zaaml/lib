// <copyright file="SourceBindingExtension.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.PresentationCore.Theming;

namespace Zaaml.PresentationCore.Data.MarkupExtensions
{
	public sealed class SourceSkinBindingExtension : SkinBindingBaseExtension
	{
		public SkinBase Skin { get; set; }

		protected override void SetBindingSource(System.Windows.Data.Binding binding)
		{
			binding.Source = Skin;
		}
	}
}