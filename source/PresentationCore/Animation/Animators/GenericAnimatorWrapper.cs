// <copyright file="GenericAnimatorWrapper.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows.Media.Animation;
using Zaaml.Core.Utils;

namespace Zaaml.PresentationCore.Animation.Animators
{
  public class GenericAnimatorWrapper<T> : IAnimator
  {
    #region Fields

    private readonly IAnimator<T> _animator;

    #endregion

    #region Ctors

    public GenericAnimatorWrapper(IAnimator<T> animator)
    {
      _animator = animator;
    }

    #endregion

    #region Properties

    public object Current => _animator.Current;

    public IEasingFunction EasingFunction
    {
      get => _animator.EasingFunction;
      set => _animator.EasingFunction = value;
    }

    public object End
    {
      get => _animator.End;
      set => _animator.End = CoerceValue(value);
    }

    public object Start
    {
      get => _animator.Start;
      set => _animator.Start = CoerceValue(value);
    }

    public double RelativeTime
    {
      get => _animator.RelativeTime;
      set => _animator.RelativeTime = value;
    }

    #endregion

    #region  Methods

    private static T CoerceValue(object value)
    {
      return value == null ? (T) RuntimeUtils.CreateDefaultValue(typeof(T)) : (T) value;
    }

    #endregion
  }
}