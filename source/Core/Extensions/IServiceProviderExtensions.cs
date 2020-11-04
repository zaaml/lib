// <copyright file="IServiceProviderExtension.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.Core.Extensions
{
// ReSharper disable once InconsistentNaming
  internal static class IServiceProviderExtensions
  {
    #region  Methods

    public static T GetService<T>(this IServiceProvider serviceProvider)
    {
      return (T) serviceProvider.GetService(typeof(T));
    }

    #endregion
  }
}