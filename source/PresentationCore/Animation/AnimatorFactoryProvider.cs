// <copyright file="AnimatorFactoryProvider.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using Zaaml.Core.Extensions;
using Zaaml.PresentationCore.Animation.Animators;
using Zaaml.PresentationCore.Animation.Interpolators;

namespace Zaaml.PresentationCore.Animation
{
  internal static class AnimatorFactoryProvider
  {
    #region Static Fields and Constants

    private static readonly Dictionary<Type, Func<IAnimator>> RegisteredAnimatorFactories = new Dictionary<Type, Func<IAnimator>>();

    #endregion

    #region Ctors

    static AnimatorFactoryProvider()
    {
      // Primitive animators
      RegisterFactory(() => DoubleInterpolator.Instance.CreateAnimator());
      RegisterFactory(() => SingleInterpolator.Instance.CreateAnimator());
      RegisterFactory(() => DecimalInterpolator.Instance.CreateAnimator());
      RegisterFactory(() => ByteInterpolator.Instance.CreateAnimator());
      RegisterFactory(() => BoolInterpolator.Instance.CreateAnimator());
      RegisterFactory(() => ShortInterpolator.Instance.CreateAnimator());
      RegisterFactory(() => IntInterpolator.Instance.CreateAnimator());
      RegisterFactory(() => LongInterpolator.Instance.CreateAnimator());
      RegisterFactory(() => MatrixInterpolator.Instance.CreateAnimator());
      RegisterFactory(() => ColorInterpolator.Instance.CreateAnimator());
      RegisterFactory(() => SizeInterpolator.Instance.CreateAnimator());
      RegisterFactory(() => PointInterpolator.Instance.CreateAnimator());
      RegisterFactory(() => RectInterpolator.Instance.CreateAnimator());


      // Statefull animators
      RegisterFactory(() => new SolidColorBrushAnimator());
      RegisterFactory(() => new TranslateTransformAnimator());
      RegisterFactory(() => new ScaleTransformAnimator());
      RegisterFactory(() => new SkewTransformAnimator());
      RegisterFactory(() => new RotateTransformAnimator());
      RegisterFactory(() => new MatrixTransformAnimator());
    }

    #endregion

    #region  Methods

    public static Func<IAnimator> GetAnimatorFactory(Type type)
    {
      return RegisteredAnimatorFactories.GetValueOrDefault(type);
    }

    public static void RegisterFactory(Type type, Func<IAnimator> factory)
    {
      RegisteredAnimatorFactories[type] = factory;
    }

    public static void RegisterFactory<T>(Func<IAnimator<T>> factory)
    {
      RegisterFactory(typeof(T), () => new GenericAnimatorWrapper<T>(factory()));
    }

    #endregion
  }
}