// <copyright file="AnimationBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media.Animation;
using Zaaml.Core.Extensions;
using Zaaml.PresentationCore.Animation.Animators;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.PresentationCore.Animation
{
	public abstract class AnimationBase : AnimationTimeline
	{
	}

	public abstract class AnimationBase<T> : AnimationBase, ISupportInitialize
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

		private IAnimator<T> _animator;
		private bool _initializing;

		protected override double ActualAccelerationRatio => Transition?.AccelerationRatio ?? base.ActualAccelerationRatio;

		protected override TimeSpan? ActualBeginTime => Transition != null ? Transition?.BeginTime : base.ActualBeginTime;

		protected override double ActualDecelerationRatio => Transition?.DecelerationRatio ?? base.ActualDecelerationRatio;

		protected override Duration ActualDuration => Transition?.Duration ?? base.ActualDuration;

		protected override double ActualSpeedRatio => Transition?.SpeedRatio ?? base.ActualSpeedRatio;

		public T Current
		{
			get => this.GetValue<T>(CurrentPropertyKey);
			private set => this.SetReadOnlyValue(CurrentPropertyKey, value);
		}

		public IEasingFunction EasingFunction
		{
			get => (IEasingFunction) GetValue(EasingFunctionProperty);
			set => SetValue(EasingFunctionProperty, value);
		}

		public T From
		{
			get => (T) GetValue(FromProperty);
			set => SetValue(FromProperty, value);
		}

		public bool Invert
		{
			get => (bool) GetValue(InvertProperty);
			set => SetValue(InvertProperty, value);
		}

		public T To
		{
			get => (T) GetValue(ToProperty);
			set => SetValue(ToProperty, value);
		}

		public Transition Transition
		{
			get => (Transition) GetValue(TransitionProperty);
			set => SetValue(TransitionProperty, value);
		}

		internal abstract IAnimator<T> CreateAnimator();

		private void EnsureTimeline()
		{
			if (_initializing || _animator != null)
				return;

			_animator = CreateAnimator();
			_animator.RelativeTime = RelativeTime;

			UpdateCurrent();
		}

		private void OnEasingFunctionChanged()
		{
			UpdateEasingFunction();
		}

		private void UpdateEasingFunction()
		{
			EnsureTimeline();

			if (_animator != null)
				_animator.EasingFunction = ActualEasingFunction;

			UpdateCurrent();
		}

		protected IEasingFunction ActualEasingFunction => Transition != null ? Transition.EasingFunction : EasingFunction;

		private void OnFromChanged()
		{
			EnsureTimeline();

			if (_animator != null)
				_animator.Start = From;

			UpdateCurrent();
		}

		private void OnInvertChanged()
		{
			EnsureTimeline();

			if (_animator != null)
				_animator.Invert = Invert;

			UpdateCurrent();
		}

		internal override void OnRelativeTimeChanged()
		{
			EnsureTimeline();

			if (_animator != null)
				_animator.RelativeTime = RelativeTime;

			UpdateCurrent();
		}

		private void OnToChanged()
		{
			EnsureTimeline();

			if (_animator != null)
				_animator.End = To;

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
			Current = _animator != null ? _animator.Current : From;
		}

		void ISupportInitialize.BeginInit()
		{
			_initializing = true;
		}

		void ISupportInitialize.EndInit()
		{
			_initializing = false;

			EnsureTimeline();
			UpdateCurrent();
		}
	}

	public struct AccelerationDecelerationRatio
	{
		public double AccelerationRatio { get; set; }
		public double DecelerationRatio { get; set; }

		public double CalcProgress(double progress)
		{
			if (progress.IsLessThanOrClose(0.0))
				return 0.0;

			if (progress.IsGreaterThanOrClose(1.0))
				return 1.0;

			var isAccelerationZero = AccelerationRatio.IsZero();
			var isDecelerationZero = DecelerationRatio.IsZero();

			if (isAccelerationZero && isDecelerationZero)
				return progress;

			if (isAccelerationZero)
				return CalcDeceleration(progress);

			if (isDecelerationZero)
				return CalcAcceleration(progress);

			return CalcAccelerationDeceleration(progress);
		}

		private double CalcDeceleration(double progress)
		{
			var speed = 1 / (1 / (2 * DecelerationRatio) + 1 - AccelerationRatio - 1 / (2 * AccelerationRatio));

			var acceleration = speed / AccelerationRatio;

			if (progress.IsLessThan(AccelerationRatio))
				return acceleration * progress * progress / 2;

			if (progress.IsLessThan(1 - DecelerationRatio))
				return acceleration * AccelerationRatio * AccelerationRatio / 2 + speed * (progress - AccelerationRatio);

			var t = DecelerationRatio - (1 - progress);
			var deceleration = -speed / DecelerationRatio;

			return acceleration * AccelerationRatio * AccelerationRatio / 2 + speed * (1 - DecelerationRatio - AccelerationRatio) + speed * t + deceleration * t * t / 2;
		}

		private double CalcAcceleration(double progress)
		{
			var speed = 1 / (1 / (2 * DecelerationRatio) + 1 - AccelerationRatio - 1 / (2 * AccelerationRatio));

			var acceleration = speed / AccelerationRatio;

			if (progress.IsLessThan(AccelerationRatio))
				return acceleration * progress * progress / 2;

			if (progress.IsLessThan(1 - DecelerationRatio))
				return acceleration * AccelerationRatio * AccelerationRatio / 2 + speed * (progress - AccelerationRatio);

			var t = DecelerationRatio - (1 - progress);
			var deceleration = -speed / DecelerationRatio;

			return acceleration * AccelerationRatio * AccelerationRatio / 2 + speed * (1 - DecelerationRatio - AccelerationRatio) + speed * t + deceleration * t * t / 2;
		}

		private double CalcAccelerationDeceleration(double progress)
		{
			var speed = 1 / (1 / (2 * DecelerationRatio) + 1 - AccelerationRatio - 1 / (2 * AccelerationRatio));

			var acceleration = speed / AccelerationRatio;

			if (progress.IsLessThan(AccelerationRatio))
				return acceleration * progress * progress / 2;

			if (progress.IsLessThan(1 - DecelerationRatio))
				return acceleration * AccelerationRatio * AccelerationRatio / 2 + speed * (progress - AccelerationRatio);

			var t = DecelerationRatio - (1 - progress);
			var deceleration = -speed / DecelerationRatio;

			return acceleration * AccelerationRatio * AccelerationRatio / 2 + speed * (1 - DecelerationRatio - AccelerationRatio) + speed * t + deceleration * t * t / 2;
		}
	}
}