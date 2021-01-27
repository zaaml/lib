// <copyright file="IControl.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.PresentationCore.Interfaces
{
  internal interface IControl : IFrameworkElement
  {
    #region Properties

		bool Focusable { get; }

    bool IsEnabled { get; }

    bool IsFocused { get; }

    bool IsMouseOver { get; }

		bool IsLoaded { get; }

    #endregion
  }
}