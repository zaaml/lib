// <copyright file="ModalState.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows.Interop;
using System.Windows.Threading;

namespace Zaaml.PresentationCore
{
  internal sealed class ModalStateCancellationToken
  {
    #region Properties

    public DispatcherFrame DispatcherFrame { get; } = new DispatcherFrame(true);

    #endregion

    #region  Methods

    public void Cancel()
    {
      DispatcherFrame.Continue = false;
    }

    #endregion
  }

  internal static class ModalState
  {
    #region  Methods

    public static void Enter(ModalStateCancellationToken cancellationToken)
    {
      try
      {
        ComponentDispatcher.PushModal();
        Dispatcher.PushFrame(cancellationToken.DispatcherFrame);
      }
      finally
      {
        ComponentDispatcher.PopModal();
      }
    }

    #endregion
  }
}
