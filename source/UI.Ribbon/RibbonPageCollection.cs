// <copyright file="RibbonPageCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.Ribbon
{
  public sealed class RibbonPageCollection : ItemCollectionBase<RibbonPageCategory, RibbonPage>
  {
    #region Ctors

    internal RibbonPageCollection(RibbonPageCategory ribbonPageCategory) : base(ribbonPageCategory)
    {
    }

    #endregion

    #region Properties

    protected override ItemGenerator<RibbonPage> DefaultGenerator => throw new NotImplementedException();

    private RibbonControl Ribbon => Control.Ribbon;

    #endregion

    #region  Methods

    protected override void OnItemAttached(RibbonPage item)
    {
      if (Ribbon != null)
        item.Ribbon = Ribbon;

      if (Control != null)
        item.PageCategory = Control;

      base.OnItemAttached(item);
    }

    protected override void OnItemDetached(RibbonPage item)
    {
      base.OnItemDetached(item);

      if (ReferenceEquals(Ribbon, item.Ribbon))
        item.Ribbon = null;

      if (ReferenceEquals(Control, item.PageCategory))
        item.PageCategory = null;
    }

    #endregion
  }
}