// <copyright file="PresentationCoreExtensions.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.PresentationCore.Utils;

namespace Zaaml.PresentationCore.Extensions
{
  internal static class PresentationCoreExtensions
  {
    #region  Methods

    public static object GetAsFrozen(this object value)
    {
      return PresentationCoreUtils.FreezeValue(value);
    }

    #endregion
  }
}
