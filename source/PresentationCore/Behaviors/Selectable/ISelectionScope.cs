// <copyright file="ISelectionScope.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.PresentationCore.Behaviors.Selectable
{
  internal interface ISelectionScope<T>
  {
    #region Properties

    T SelectedItem { get; set; }

    #endregion
  }
}