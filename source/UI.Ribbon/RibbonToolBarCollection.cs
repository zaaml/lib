// <copyright file="RibbonToolBarCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using Zaaml.PresentationCore.ObservableCollections;

namespace Zaaml.UI.Controls.Ribbon
{
  public sealed class RibbonToolBarCollection : DelegateDependencyObjectCollection<RibbonToolBar>
  {
    #region Ctors

    internal RibbonToolBarCollection(Action<RibbonToolBar> onItemAdded, Action<RibbonToolBar> onItemRemoved) :
      base(onItemAdded, onItemRemoved)
    {
    }

    #endregion
  }
}