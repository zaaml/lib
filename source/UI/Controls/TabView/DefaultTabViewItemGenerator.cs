// <copyright file="DefaultTabViewItemGenerator.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.TabView
{
  internal class DefaultTabViewItemGenerator : TabViewItemGeneratorBase, IDelegatedGenerator<TabViewItem>
  {
    #region  Methods

    protected override void AttachItem(TabViewItem item, object source)
    {
      Implementation.AttachItem(item, source);
    }

    protected override TabViewItem CreateItem(object source)
    {
      return Implementation.CreateItem(source);
    }

    protected override void DetachItem(TabViewItem item, object source)
    {
      Implementation.DetachItem(item, source);
    }

    protected override void DisposeItem(TabViewItem item, object source)
    {
      Implementation.DisposeItem(item, source);
    }

    #endregion

    #region Interface Implementations

    #region IDelegatedGenerator<TabViewItem>

    public IItemGenerator<TabViewItem> Implementation { get; set; }

    #endregion

    #endregion
  }

  internal class DefaultItemTemplateTabViewItemGenerator : DelegateHeaderedIconContentSelectableItemGeneratorImplementation<TabViewItem, DefaultTabViewItemGenerator>
  {
	  public DefaultItemTemplateTabViewItemGenerator(TabViewControl tabViewControl) : base(tabViewControl)
	  {
	  }
  }
}