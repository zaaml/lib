// <copyright file="StopAnimationCommand.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.PresentationCore.Animation
{
	public sealed class StopAnimationCommand : AnimationCommand
	{
		protected override void RunCore(AnimationTimeline timeline)
		{
			timeline.Stop();
		}
	}
}