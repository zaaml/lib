// <copyright file="SelfBindingExtension.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

#if SILVERLIGHT
using System.ComponentModel;
#endif
using NativeBinding = System.Windows.Data.Binding;

namespace Zaaml.PresentationCore.Data.MarkupExtensions
{
	public sealed class SelfBindingExtension : PathBindingBase
	{
		protected override void InitSource(NativeBinding binding)
		{
			binding.RelativeSource = XamlConstants.Self;
		}
	}
}