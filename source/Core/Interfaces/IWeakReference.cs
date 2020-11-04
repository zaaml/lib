// <copyright file="IWeakReference.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Core.Interfaces
{
  internal interface IWeakReference
  {
    #region Properties

    bool IsAlive { get; }

    #endregion
  }
}