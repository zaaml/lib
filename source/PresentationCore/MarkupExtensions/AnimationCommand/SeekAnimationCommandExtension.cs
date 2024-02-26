// <copyright file="SeekAnimationCommandExtension.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using Zaaml.PresentationCore.Animation;

namespace Zaaml.PresentationCore.MarkupExtensions.AnimationCommand
{
	public sealed class SeekAnimationCommandExtension : AnimationCommandExtension
	{
		private double? _relativeTime;
		private TimeSpan? _time;

		public double RelativeTime
		{
			get => _relativeTime ?? 0.0;
			set => _relativeTime = value;
		}

		public TimeSpan Time
		{
			get => _time ?? TimeSpan.Zero;
			set => _time = value;
		}

		protected override Animation.AnimationCommand CreateCommandCore()
		{
			var command = new SeekAnimationCommand();

			if (_time.HasValue)
				command.Time = _time.Value;

			if (_relativeTime.HasValue)
				command.RelativeTime = _relativeTime.Value;

			return command;
		}
	}
}