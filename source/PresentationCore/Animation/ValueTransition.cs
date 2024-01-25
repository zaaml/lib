// <copyright file="ValueTransition.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace Zaaml.PresentationCore.Animation
{
	public struct ValueTransition
	{
		public double AccelerationRatio { get; set; }

		public TimeSpan? BeginTime { get; set; }

		public double DecelerationRatio { get; set; }

		public Duration Duration { get; set; }

		public IEasingFunction EasingFunction { get; set; }

		public double SpeedRatio { get; set; }
	}
}
