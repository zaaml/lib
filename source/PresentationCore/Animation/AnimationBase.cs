// <copyright file="AnimationBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using System.Windows.Media.Animation;
using Zaaml.Core.Runtime;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.PresentationCore.Animation
{
	public abstract class AnimationBase : AnimationTimeline
	{
	}

	public abstract class AnimationBase<T> : AnimationBase
	{
		public static readonly DependencyProperty FromProperty = DPM.Register<T, AnimationBase<T>>
			("From", mt => mt.OnFromChanged);

		public static readonly DependencyProperty ToProperty = DPM.Register<T, AnimationBase<T>>
			("To", mt => mt.OnToChanged);

		public static readonly DependencyProperty EasingFunctionProperty = DPM.Register<IEasingFunction, AnimationBase<T>>
			("EasingFunction", mt => mt.OnEasingFunctionChanged);

		private static readonly DependencyPropertyKey CurrentPropertyKey = DPM.RegisterReadOnly<T, AnimationBase<T>>
			("Current");

		public static readonly DependencyProperty InvertProperty = DPM.Register<bool, AnimationBase<T>>
			("Invert", false, d => d.OnInvertChanged);

		public static readonly DependencyProperty TransitionProperty = DPM.Register<Transition, AnimationBase<T>>
			("Transition", null, d => d.OnTransitionPropertyChangedPrivate);

		private readonly IAnimationValue<T> _animationValue;

		protected AnimationBase(Interpolator<T> interpolator)
		{
			_animationValue = new AnimationValue<T>(interpolator);
		}

		protected override double ActualAccelerationRatio => Transition?.AccelerationRatio ?? base.ActualAccelerationRatio;

		protected override TimeSpan? ActualBeginTime => Transition?.BeginTime ?? base.ActualBeginTime;

		protected override double ActualDecelerationRatio => Transition?.DecelerationRatio ?? base.ActualDecelerationRatio;

		protected override Duration ActualDuration => Transition?.Duration ?? base.ActualDuration;

		protected IEasingFunction ActualEasingFunction => Transition?.EasingFunction ?? EasingFunction;

		protected override double ActualSpeedRatio => Transition?.SpeedRatio ?? base.ActualSpeedRatio;

		public T Current
		{
			get => this.GetValue<T>(CurrentPropertyKey);
			private set => this.SetReadOnlyValue(CurrentPropertyKey, value);
		}

		public IEasingFunction EasingFunction
		{
			get => (IEasingFunction)GetValue(EasingFunctionProperty);
			set => SetValue(EasingFunctionProperty, value);
		}

		public T From
		{
			get => (T)GetValue(FromProperty);
			set => SetValue(FromProperty, value);
		}

		public bool Invert
		{
			get => (bool)GetValue(InvertProperty);
			set => SetValue(InvertProperty, value.Box());
		}

		public T To
		{
			get => (T)GetValue(ToProperty);
			set => SetValue(ToProperty, value);
		}

		public Transition Transition
		{
			get => (Transition)GetValue(TransitionProperty);
			set => SetValue(TransitionProperty, value);
		}

		protected override void EndInitCore()
		{
			base.EndInitCore();

			EnsureTimeline();
			UpdateCurrent();
		}

		private void EnsureTimeline()
		{
			if (Initializing)
				return;

			_animationValue.RelativeTime = RelativeTime;

			UpdateCurrent();
		}

		private void OnEasingFunctionChanged()
		{
			UpdateEasingFunction();
		}

		private void OnFromChanged()
		{
			EnsureTimeline();

			_animationValue.Start = From;

			UpdateCurrent();
		}

		private void OnInvertChanged()
		{
			EnsureTimeline();

			_animationValue.Invert = Invert;

			UpdateCurrent();
		}

		internal override void OnRelativeTimeChanged()
		{
			EnsureTimeline();

			_animationValue.RelativeTime = RelativeTime;

			UpdateCurrent();
		}

		private void OnToChanged()
		{
			EnsureTimeline();

			_animationValue.End = To;

			UpdateCurrent();
		}

		private void OnTransitionPropertyChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if (e.Property == Transition.AccelerationRatioProperty)
				UpdateAccelerationRatio();
			else if (e.Property == Transition.DecelerationRatioProperty)
				UpdateDecelerationRatio();
			else if (e.Property == Transition.SpeedRatioProperty)
				UpdateSpeedRatio();
			else if (e.Property == Transition.BeginTimeProperty)
				UpdateBeginTime();
			else if (e.Property == Transition.DurationProperty)
				UpdateDuration();
			else if (e.Property == Transition.EasingFunctionProperty)
				UpdateEasingFunction();
		}

		private void OnTransitionPropertyChangedPrivate(Transition oldValue, Transition newValue)
		{
			if (ReferenceEquals(oldValue, newValue))
				return;

			if (oldValue != null)
				oldValue.DependencyPropertyChangedInternal -= OnTransitionPropertyChanged;

			if (newValue != null)
				newValue.DependencyPropertyChangedInternal += OnTransitionPropertyChanged;

			UpdateAccelerationRatio();
			UpdateDecelerationRatio();
			UpdateSpeedRatio();
			UpdateBeginTime();
			UpdateDuration();
			UpdateEasingFunction();
		}

		private void UpdateCurrent()
		{
			Current = _animationValue.Current;
		}

		private void UpdateEasingFunction()
		{
			EnsureTimeline();

			_animationValue.EasingFunction = ActualEasingFunction;

			UpdateCurrent();
		}
	}
}