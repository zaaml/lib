// <copyright file="ResumeAnimationCommand.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.PresentationCore.Animation
{
	public sealed class ResumeAnimationCommand : AnimationCommand
	{
		protected override void RunCore(AnimationTimeline timeline)
		{
			timeline.Resume();
		}
	}
}