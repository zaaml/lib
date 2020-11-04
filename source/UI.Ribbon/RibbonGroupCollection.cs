// <copyright file="RibbonGroupCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.Ribbon
{
  public sealed class RibbonGroupCollection : ItemCollectionBase<RibbonPage, RibbonGroup>
  {
    #region Ctors

    internal RibbonGroupCollection(RibbonPage page) : base(page)
    {
    }

    #endregion

    #region Properties

    protected override ItemGenerator<RibbonGroup> DefaultGenerator => throw new NotImplementedException();

    #endregion
  }
}