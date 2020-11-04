// <copyright file="FreezableExtensions.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) zaaml. All rights reserved.
// </copyright>

#if !SILVERLIGHT
using System.Windows;
#endif

namespace Zaaml.PresentationCore.Extensions
{
  internal static class FreezableExtensions
  {
#if SILVERLIGHT
    public static T AsFrozen<T>(this T freezable) where T : class
    {
      return freezable;
    }
#else
    public static T AsFrozen<T>(this T freezable) where T : Freezable
    {
      if (freezable.CanFreeze && freezable.IsFrozen == false)
        freezable.Freeze();

      return freezable;
    }
#endif

  }
}