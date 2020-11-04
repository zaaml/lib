// <copyright file="AnimationBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

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
    #region Static Fields and Constants

    public static readonly DependencyProperty FromProperty = DPM.Register<T, AnimationBase<T>>
      ("From", mt => mt.OnFromChanged);

    public static readonly DependencyProperty ToProperty = DPM.Register<T, AnimationBase<T>>
      ("To", mt => mt.OnToChanged);

    public static readonly DependencyProperty EasingFunctionProperty = DPM.Register<IEasingFunction, AnimationBase<T>>
      ("EasingFunction", mt => mt.OnEasingFunctionChanged);

    private static readonly DependencyPropertyKey CurrentPropertyKey = DPM.RegisterReadOnly<T, AnimationBase<T>>
      ("Current");

    #endregion

    #region Fields

    private IAnimator<T> _animator;
    private bool _initializing;

    #endregion

    #region Properties

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

    public T To
    {
      get => (T) GetValue(ToProperty);
      set => SetValue(ToProperty, value);
    }

    #endregion

    #region  Methods

    internal abstract IAnimator<T> CreateAnimator();

    private void EnsureTimeline()
    {
      if (_initializing || _animator != null)
        return;

      _animator = CreateAnimator();
      _animator.Time = Time;

      UpdateCurrent();
    }

    private void OnEasingFunctionChanged()
    {
      EnsureTimeline();

      if (_animator != null)
        _animator.EasingFunction = EasingFunction;

      UpdateCurrent();
    }

    private void OnFromChanged()
    {
      EnsureTimeline();

      if (_animator != null)
        _animator.Start = From;

      UpdateCurrent();
    }

    internal override void OnTimeChanged()
    {
      EnsureTimeline();

      if (_animator != null)
        _animator.Time = Time;

      UpdateCurrent();
    }

    private void OnToChanged()
    {
      EnsureTimeline();

      if (_animator != null)
        _animator.End = To;

      UpdateCurrent();
    }

    private void UpdateCurrent()
    {
      Current = _animator != null ? _animator.Current : From;
    }

    #endregion

    #region Interface Implementations

    #region ISupportInitialize

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

    #endregion

    #endregion
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