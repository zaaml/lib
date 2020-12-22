// <copyright file="DefaultAccordionViewItemGenerator.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.AccordionView
{
  internal sealed class DefaultAccordionViewItemGenerator : AccordionViewItemGeneratorBase, IDelegatedGenerator<AccordionViewItem>
  {
    #region  Methods

    protected override void AttachItem(AccordionViewItem item, object source)
    {
      Implementation.AttachItem(item, source);
    }

    protected override AccordionViewItem CreateItem(object source)
    {
      return Implementation.CreateItem(source);
    }

    protected override void DetachItem(AccordionViewItem item, object source)
    {
      Implementation.DetachItem(item, source);
    }

    protected override void DisposeItem(AccordionViewItem item, object source)
    {
      Implementation.DisposeItem(item, source);
    }

    #endregion

    #region Interface Implementations

    #region IDelegatedGenerator<AccordionViewItem>

    public IItemGenerator<AccordionViewItem> Implementation { get; set; }

    #endregion

    #endregion
  }
}