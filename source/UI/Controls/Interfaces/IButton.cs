// <copyright file="IButton.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.PresentationCore.Interfaces;

namespace Zaaml.UI.Controls.Interfaces
{
  internal interface IButton : IControl, ICommandControl
  {
    #region Properties

    bool IsPressed { get; }

    #endregion
  }
}