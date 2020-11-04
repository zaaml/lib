// <copyright file="DefaultAccordionViewItemGenerator.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.AccordionView
{
  internal sealed class DefaultAccordionViewItemGenerator : AccordionViewItemGeneratorBase, IDelegatedGenerator<AccordionViewItem>
  {
    #region  Methods

    protected override void AttachItem(AccordionViewItem item, object itemSource)
    {
      Implementation.AttachItem(item, itemSource);
    }

    protected override AccordionViewItem CreateItem(object itemSource)
    {
      return Implementation.CreateItem(itemSource);
    }

    protected override void DetachItem(AccordionViewItem item, object itemSource)
    {
      Implementation.DetachItem(item, itemSource);
    }

    protected override void DisposeItem(AccordionViewItem item, object itemSource)
    {
      Implementation.DisposeItem(item, itemSource);
    }

    #endregion

    #region Interface Implementations

    #region IDelegatedGenerator<AccordionViewItem>

    public IItemGenerator<AccordionViewItem> Implementation { get; set; }

    #endregion

    #endregion
  }
}