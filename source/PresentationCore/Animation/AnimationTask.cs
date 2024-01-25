// <copyright file="AnimationTask.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Zaaml.Core.Utils;

namespace Zaaml.PresentationCore.Animation
{
	public sealed class AnimationTask<TValue> : ITimelineClockCallback, ILinkedListNode<AnimationTask<TValue>>
	{
		private static readonly RunningTaskList RunningTasks = new();

		private readonly AnimationValue<TValue> _animationValue;

		private readonly Action<TValue> _onCurrentChanged;
		private CancellationToken _cancellationToken;
		private TimelineClock _clock;
		private TaskCompletionSource<bool> _completionSource;

		public AnimationTask(Interpolator<TValue> interpolator, TValue from, TValue to, ValueTransition transition, Action<TValue> onCurrentChanged)
		{
			_animationValue = new AnimationValue<TValue>(interpolator)
			{
				EasingFunction = transition.EasingFunction,
				Start = from,
				End = to
			};

			_onCurrentChanged = onCurrentChanged;

			Transition = transition;
		}

		public TValue Current => _animationValue.Current;

		public TValue From => _animationValue.Start;

		public TValue To => _animationValue.End;

		public ValueTransition Transition { get; }

		public void Run()
		{
			RunClock();
		}

		public Task RunAsync()
		{
			_completionSource = new TaskCompletionSource<bool>();

			RunClock();

			return _completionSource.Task;
		}

		public Task RunAsync(CancellationToken cancellationToken)
		{
			_cancellationToken = cancellationToken;
			_completionSource = new TaskCompletionSource<bool>();

			RunClock();

			return _completionSource.Task;
		}

		private void RunClock()
		{
			if (_clock != null)
				throw new InvalidOperationException("Task is already started");

			_clock = new TimelineClock(this, Transition);

			_clock.Begin();

			LinkedListUtils.Append(RunningTasks, this);
		}

		AnimationTask<TValue> ILinkedListNode<AnimationTask<TValue>>.Next { get; set; }

		AnimationTask<TValue> ILinkedListNode<AnimationTask<TValue>>.Prev { get; set; }

		void ITimelineClockCallback.OnCompleted(TimelineClock clock)
		{
			try
			{
				if (_completionSource == null)
					return;

				if (_cancellationToken.IsCancellationRequested)
					_completionSource.SetCanceled();
				else
					_completionSource.SetResult(true);
			}
			finally
			{
				LinkedListUtils.Remove(RunningTasks, this);
			}
		}

		void ITimelineClockCallback.OnPaused(TimelineClock clock)
		{
		}

		void ITimelineClockCallback.OnTimeChanged(TimelineClock clock)
		{
			if (_cancellationToken.IsCancellationRequested)
			{
				_clock.Stop();

				return;
			}

			_animationValue.RelativeTime = clock.RelativeTime;

			_onCurrentChanged(_animationValue.Current);
		}

		void ITimelineClockCallback.OnResumed(TimelineClock clock)
		{
		}

		void ITimelineClockCallback.OnStarted(TimelineClock clock)
		{
		}

		private sealed class RunningTaskList : ILinkedList<AnimationTask<TValue>>
		{
			public AnimationTask<TValue> Head { get; set; }

			public AnimationTask<TValue> Tail { get; set; }

			public int Version { get; set; }
		}
	}

	public static class AnimationTask
	{
		public static void Run<TValue>(TValue from, TValue to, Duration duration, Action<TValue> changed)
		{
			Run(Interpolator.GetInterpolator<TValue>(), from, to, duration, changed);
		}

		public static void Run<TValue>(Interpolator<TValue> interpolator, TValue from, TValue to, Duration duration, Action<TValue> changed)
		{
			new AnimationTask<TValue>(Interpolator.GetInterpolator<TValue>(), from, to, new ValueTransition { Duration = duration }, changed).Run();
		}

		public static Task RunAsync<TValue>(TValue from, TValue to, Duration duration, Action<TValue> changed)
		{
			return RunAsync(Interpolator.GetInterpolator<TValue>(), from, to, duration, changed);
		}

		public static Task RunAsync<TValue>(TValue from, TValue to, Duration duration, Action<TValue> changed, CancellationToken cancellationToken)
		{
			return RunAsync(Interpolator.GetInterpolator<TValue>(), from, to, duration, changed, cancellationToken);
		}

		public static Task RunAsync<TValue>(Interpolator<TValue> interpolator, TValue from, TValue to, Duration duration, Action<TValue> changed)
		{
			return new AnimationTask<TValue>(Interpolator.GetInterpolator<TValue>(), from, to, new ValueTransition { Duration = duration }, changed).RunAsync();
		}

		public static Task RunAsync<TValue>(Interpolator<TValue> interpolator, TValue from, TValue to, Duration duration, Action<TValue> changed, CancellationToken cancellationToken)
		{
			return new AnimationTask<TValue>(Interpolator.GetInterpolator<TValue>(), from, to, new ValueTransition { Duration = duration }, changed).RunAsync(cancellationToken);
		}
	}
}
