// <copyright file="RibbonPageCategoryCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.Ribbon
{
  public sealed class RibbonPageCategoryCollection : ItemCollectionBase<RibbonControl, RibbonPageCategory>
  {
    #region Ctors

    internal RibbonPageCategoryCollection(RibbonControl ribbon) : base(ribbon)
    {
    }

    #endregion

    #region Properties

    protected override ItemGenerator<RibbonPageCategory> DefaultGenerator => throw new NotImplementedException();

    #endregion
  }
}