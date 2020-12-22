// <copyright file="ListViewItemsPresenter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.ListView
{
  [TemplateContractType(typeof(ListViewItemsPresenterTemplateContract))]
  public class ListViewItemsPresenter : ScrollableItemsPresenterBase<ListViewControl, ListViewItem, ListViewItemCollection, ListViewItemsPanel>
  {
	  #region Ctors

    static ListViewItemsPresenter()
    {
      DefaultStyleKeyHelper.OverrideStyleKey<ListViewItemsPresenter>();
    }

    public ListViewItemsPresenter()
    {
      this.OverrideStyleKey<ListViewItemsPresenter>();
    }

    #endregion

    #region Properties

    internal ListViewControl ListViewControl { get; set; }

    #endregion

    #region  Methods

    protected override void OnTemplateContractAttached()
    {
      base.OnTemplateContractAttached();

      ItemsHost.ItemsPresenter = this;
    }

    protected override void OnTemplateContractDetaching()
    {
      ItemsHost.ItemsPresenter = null;

      base.OnTemplateContractDetaching();
    }

    #endregion
  }

  public class ListViewItemsPresenterTemplateContract : ItemsPresenterBaseTemplateContract<ListViewItemsPanel, ListViewItem>
  {
  }
}
