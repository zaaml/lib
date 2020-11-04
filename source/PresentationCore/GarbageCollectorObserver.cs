// <copyright file="GarbageCollectorObserver.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows.Media;
using Zaaml.Core;

namespace Zaaml.PresentationCore
{
  internal sealed class GarbageCollectorObserver : IDisposable
  {
    #region Fields

    private int _gcCount;

    private bool _isDisposed;
    public event EventHandler GarbageCollected;

    #endregion

    #region Ctors

    public GarbageCollectorObserver()
    {
      CompositionTarget.Rendering += CompositionTargetOnRendering;
    }

    #endregion

    #region  Methods

    private void CompositionTargetOnRendering(object sender, EventArgs eventArgs)
    {
      var gcCount = GarbageCleanupCounter.CleanupCount;
      var isAlive = _gcCount == gcCount;
      try
      {
        if (isAlive == false)
          GarbageCollected?.Invoke(this, EventArgs.Empty);
      }
      finally
      {
        _gcCount = gcCount;
      }
    }

    #endregion

    #region Interface Implementations

    #region IDisposable

    public void Dispose()
    {
      if (_isDisposed)
        return;

      _isDisposed = true;
      CompositionTarget.Rendering -= CompositionTargetOnRendering;
    }

    #endregion

    #endregion
  }
}