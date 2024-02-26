// <copyright file="ElementNameSkinBindingExtension.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.PresentationCore.Data.MarkupExtensions
{
	public sealed class ElementNameSkinBindingExtension : SkinBindingBaseExtension
	{
		public string ElementName { get; set; }

		protected override void SetBindingSource(System.Windows.Data.Binding binding)
		{
			binding.Path = ActualSkinPropertyPath;
			binding.ElementName = ElementName;
		}
	}
}