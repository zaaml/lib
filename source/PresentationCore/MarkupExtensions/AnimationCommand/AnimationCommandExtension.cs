// <copyright file="AnimationCommandExtension.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.PresentationCore.MarkupExtensions.AnimationCommand
{
	public abstract class AnimationCommandExtension : MarkupExtensionBase
	{
		protected abstract Animation.AnimationCommand CreateCommandCore();

		public sealed override object ProvideValue(IServiceProvider serviceProvider)
		{
			return CreateCommandCore();
		}
	}
}