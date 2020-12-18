// <copyright file="RuntimeSetterTransition.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Animation;
using Zaaml.Core.Pools;
using Zaaml.PresentationCore.Animation;
using Zaaml.PresentationCore.Animation.Animators;
using Zaaml.PresentationCore.PropertyCore;
using DoubleAnimation = System.Windows.Media.Animation.DoubleAnimation;

#if SILVERLIGHT
#elif NETCOREAPP
using System.Windows.Threading;
#else
using Zaaml.Core.Extensions;
using System.Windows.Threading;
#endif

namespace Zaaml.PresentationCore.Interactivity
{
  internal class RuntimeSetterTransition : DependencyObject
  {
    #region Static Fields and Constants

    public static readonly DependencyProperty ProgressProperty = DPM.Register<double, RuntimeSetterTransition>
      ("Progress", e => e.OnProgressChanged);

    private static readonly Dictionary<Type, RuntimeTransitionPool> TransitionPools = new Dictionary<Type, RuntimeTransitionPool>();

    #endregion

    #region Fields

    private readonly IAnimator _animator;
    private readonly RuntimeTransitionPool _pool;
    private readonly Action _releaseAction;
    private readonly Storyboard _storyboard;
    private RuntimeSetter _setter;

    #endregion

    #region Ctors

    private RuntimeSetterTransition(IAnimator animator, RuntimeTransitionPool pool)
    {
      _releaseAction = Release;

      _animator = animator;
      _pool = pool;
      var doubleAnimation = new DoubleAnimation
      {
        From = 0.0,
        To = 1.0
      };

      Storyboard.SetTarget(doubleAnimation, this);
      Storyboard.SetTargetProperty(doubleAnimation, new PropertyPath(ProgressProperty));

      _storyboard = new Storyboard
      {
        Children = {doubleAnimation}
      };

      _storyboard.Begin();
      _storyboard.Stop();

      _storyboard.Completed += (sender, args) => OnCompleted(true);
    }

    #endregion

    #region Properties

    public object CurrentValue => _animator.Current;

    public bool IsAnimating => _storyboard.GetCurrentState() != ClockState.Stopped && _setter != null;

    public object ValueStore { get; private set; }

    #endregion

    #region  Methods

    private void OnCompleted(bool timeLineFinished)
    {
      if (_setter == null)
        return;

      if (timeLineFinished)
        _animator.Time = 1.0;

      var setter = _setter;
      _setter = null;

      setter.OnTransitionCompleted();

#if SILVERLIGHT
      setter.EffectiveValue.Target.Dispatcher.BeginInvoke(_releaseAction);
#else
      setter.EffectiveValue.Target.Dispatcher.BeginInvoke(_releaseAction, DispatcherPriority.Background);
#endif
    }

    private void OnProgressChanged(double oldValue, double newValue)
    {
      if (IsAnimating == false)
        return;

      _animator.Time = newValue;

      if (_setter.SetAnimatedValue(_animator.Current) == false)
        StopImpl();
    }

    private void Release()
    {
      _pool.Release(this);
    }

    public static RuntimeSetterTransition RunTransition(RuntimeSetter setter, object from, object to)
    {
      var type = from?.GetType() ?? to?.GetType();

      if (type == null)
        return null;

      var pool = TransitionPools.GetValueOrDefault(type);

      if (pool == null)
      {
        var animatorFactory = AnimatorFactoryProvider.GetAnimatorFactory(type);

        if (animatorFactory == null)
          return null;

        TransitionPools[type] = pool = new RuntimeTransitionPool(animatorFactory);
      }

      var runtimeTransition = pool.GeTransition();

      runtimeTransition.ValueStore = setter.ActualValueStore;
      runtimeTransition.Start(setter, from, to);

      return runtimeTransition;
    }

    public void Start(RuntimeSetter setter, object from, object to)
    {
      var transition = setter.Transition;

      _animator.Start = from;
      _animator.End = to;
      _animator.EasingFunction = transition.EasingFunction;
      _animator.Time = 0;

      var doubleAnimation = (DoubleAnimation) _storyboard.Children[0];

      doubleAnimation.BeginTime = transition.BeginTime;
      doubleAnimation.Duration = transition.Duration;

      _storyboard.Begin();

      _setter = setter;
    }

    public void Stop()
    {
      StopImpl();
    }

    private void StopImpl()
    {
      OnCompleted(false);

      _storyboard.Stop();
    }

    #endregion

    #region  Nested Types

    private class RuntimeTransitionPool
    {
      #region Fields

      private readonly LightObjectPool<RuntimeSetterTransition> _transitionsPool;

      #endregion

      #region Ctors

      public RuntimeTransitionPool(Func<IAnimator> animatorFactory)
      {
        _transitionsPool = new LightObjectPool<RuntimeSetterTransition>(() => new RuntimeSetterTransition(animatorFactory(), this));
      }

      #endregion

      #region  Methods

      public RuntimeSetterTransition GeTransition()
      {
        return _transitionsPool.GetObject();
      }

      public void Release(RuntimeSetterTransition runtimeTransition)
      {
        _transitionsPool.Release(runtimeTransition);
      }

      #endregion
    }

    #endregion
  }
}