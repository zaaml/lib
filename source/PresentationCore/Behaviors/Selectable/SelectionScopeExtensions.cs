// <copyright file="SelectionScopeExtensions.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.PresentationCore.Behaviors.Selectable
{
  internal static class SelectionScopeExtensions
  {
    #region  Methods

    public static void Select<T>(this ISelectionScope<T> scope, T item)
    {
      scope.SelectedItem = item;
    }

    #endregion
  }
}