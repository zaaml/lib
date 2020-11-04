// <copyright file="Interpolator.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using Zaaml.Core.Extensions;

namespace Zaaml.PresentationCore.Animation.Interpolators
{
	public static class Interpolator
  {
    #region Static Fields and Constants

    private static readonly Dictionary<Type, IInterpolator> Interpolators = new Dictionary<Type, IInterpolator>
    {
      {typeof(double), DoubleInterpolator.Instance},
      {typeof(Point), PointInterpolator.Instance},
      {typeof(Size), SizeInterpolator.Instance},
      {typeof(Rect), RectInterpolator.Instance},
      {typeof(Color), ColorInterpolator.Instance}
    };

    #endregion

    #region  Methods

    public static IInterpolator GetInterpolator(Type targetType)
    {
      return Interpolators.GetValueOrDefault(targetType);
    }

    public static IInterpolator<T> GetInterpolator<T>()
    {
      return GetInterpolator(typeof(T)) as IInterpolator<T>;
    }

    #endregion
  }
}