// <copyright file="DispatcherUtils.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Threading;

namespace Zaaml.PresentationCore.Utils
{
  internal static class DispatcherUtils
  {
    public static Dispatcher ApplicationDispatcher
    {
      get
      {
#if SILVERLIGHT
        return Deployment.Current.Dispatcher;
#else
        return Application.Current.Dispatcher;
#endif
      }
    }
  }
}