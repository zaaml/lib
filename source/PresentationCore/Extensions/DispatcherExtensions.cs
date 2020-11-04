// <copyright file="DispatcherExtensions.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows.Threading;

namespace Zaaml.PresentationCore.Extensions
{
  public static class DispatcherExtensions
  {
    #region  Methods

    public static void BeginInvoke(this Dispatcher dispatcher, Action action)
    {
      dispatcher.BeginInvoke(action);
    }

#if !SILVERLIGHT
    public static void BeginInvoke(this Dispatcher dispatcher, DispatcherPriority priority, Action action)
    {
      dispatcher.BeginInvoke(priority, action);
    }
#endif

    #endregion
  }
}