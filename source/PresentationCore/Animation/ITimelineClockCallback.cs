// <copyright file="ITimeline.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.PresentationCore.Animation
{
	internal interface ITimelineClockCallback
	{
		void OnCompleted(TimelineClock clock);

		void OnPaused(TimelineClock clock);

		void OnTimeChanged(TimelineClock clock);

		void OnResumed(TimelineClock clock);

		void OnStarted(TimelineClock clock);
	}
}
