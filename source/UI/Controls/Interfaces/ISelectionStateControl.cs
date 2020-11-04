// <copyright file="ISelectionStateControl.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.PresentationCore.Interfaces;

namespace Zaaml.UI.Controls.Interfaces
{
  internal interface ISelectionStateControl : IControl
  {
    #region Properties

    bool IsSelected { get; }

    #endregion
  }
}