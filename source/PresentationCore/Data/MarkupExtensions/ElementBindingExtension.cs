// <copyright file="ElementBindingExtension.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using NativeBinding = System.Windows.Data.Binding;

namespace Zaaml.PresentationCore.Data.MarkupExtensions
{
	public sealed class ElementBindingExtension : PathBindingBase
	{
		public string ElementName { get; set; }

		protected override void InitSource(NativeBinding binding)
		{
			binding.ElementName = ElementName;
		}
	}
}