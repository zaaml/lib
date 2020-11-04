// <copyright file="ISealableExtensions.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using Zaaml.Core.Interfaces;

namespace Zaaml.Core.Extensions
{
  internal static class ISealableExtensions
  {
    #region Static Fields and Constants

    private static readonly string DefaultErrorMessage = "Object is sealed and can not be changed";

    #endregion

    #region  Methods

    public static void ThrowWhenSeal(this ISealable sealable)
    {
      sealable.ThrowWhenSeal(DefaultErrorMessage);
    }

    public static void ThrowWhenSeal(this ISealable sealable, string errorMessage)
    {
      if (sealable.IsSealed)
        throw new InvalidOperationException(errorMessage);
    }

    #endregion
  }
}