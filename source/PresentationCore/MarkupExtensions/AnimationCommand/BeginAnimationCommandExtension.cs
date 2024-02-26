// <copyright file="BeginAnimationCommandExtension.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.PresentationCore.MarkupExtensions.AnimationCommand
{
	public sealed class BeginAnimationCommandExtension : AnimationCommandExtension
	{
		protected override Animation.AnimationCommand CreateCommandCore()
		{
			return Animation.AnimationCommand.Begin;
		}
	}
}