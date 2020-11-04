// <copyright file="WeakReferenceExtensions.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.Core.Extensions
{
  internal static class WeakReferenceExtensions
  {
    #region  Methods

    public static T GetTarget<T>(this WeakReference weakReference) where T : class
    {
      return (T) weakReference?.Target;
    }

    #endregion
  }
}