// <copyright file="ToolBarControlBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.Core;
using Zaaml.Core.Runtime;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.Runtime;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Utils;
using Zaaml.UI.Controls.Core;
using Zaaml.UI.Controls.DropDown;
using Zaaml.UI.Panels.Core;
using NativeControl = System.Windows.Controls.Control;

// ReSharper disable StaticMemberInGenericType

namespace Zaaml.UI.Controls.ToolBar
{
  public abstract class ToolBarControlBase<TToolBarControl, TItem, TCollection, TPresenter, TPanel> : ItemsControlBase<TToolBarControl, TItem, TCollection, TPresenter, TPanel>
    where TItem : NativeControl
		where TCollection : ItemCollectionBase<TToolBarControl, TItem>
    where TPresenter : ItemsPresenterBase<TToolBarControl, TItem, TCollection, TPanel>
    where TPanel : ItemsPanel<TItem>
    where TToolBarControl : NativeControl
  {
    #region Static Fields and Constants

    private static readonly DependencyPropertyKey HasOverflowItemsPropertyKey = DPM.RegisterReadOnly<bool, ToolBarControlBase<TToolBarControl, TItem, TCollection, TPresenter, TPanel>>
      ("HasOverflowItems", t => t.OnHasOverflowedItemsChanged);

    public static readonly DependencyProperty MenuButtonVisibilityProperty = DPM.Register<ElementVisibility, ToolBarControlBase<TToolBarControl, TItem, TCollection, TPresenter, TPanel>>
      ("MenuButtonVisibility", ElementVisibility.Auto, t => t.OnMenuButtonVisibilityChanged);

    private static readonly DependencyPropertyKey ActualMenuButtonVisibilityPropertyKey = DPM.RegisterReadOnly<Visibility, ToolBarControlBase<TToolBarControl, TItem, TCollection, TPresenter, TPanel>>
      ("ActualMenuButtonVisibility", Visibility.Collapsed, t => t.OnActualMenuButtonVisibilityChanged);

    public static readonly DependencyProperty IsMenuOpenProperty = DPM.Register<bool, ToolBarControlBase<TToolBarControl, TItem, TCollection, TPresenter, TPanel>>
      ("IsMenuOpen");

    public static readonly DependencyProperty ActualMenuButtonVisibilityProperty = ActualMenuButtonVisibilityPropertyKey.DependencyProperty;
    public static readonly DependencyProperty HasOverflowItemsProperty = HasOverflowItemsPropertyKey.DependencyProperty;

    #endregion

    #region Properties

    public Visibility ActualMenuButtonVisibility
    {
      get => (Visibility) GetValue(ActualMenuButtonVisibilityProperty);
      protected set => this.SetReadOnlyValue(ActualMenuButtonVisibilityPropertyKey, value.Box());
    }

    public bool HasOverflowItems
    {
      get => (bool) GetValue(HasOverflowItemsProperty);
      internal set => this.SetReadOnlyValue(HasOverflowItemsPropertyKey, value.Box());
    }

    private DropDownButton MenuButton => TemplateContract.MenuButton;

    public ElementVisibility MenuButtonVisibility
    {
      get => (ElementVisibility) GetValue(MenuButtonVisibilityProperty);
      set => SetValue(MenuButtonVisibilityProperty, value.Box());
    }

    private ToolBarControlBaseTemplateContract<TToolBarControl, TItem, TCollection, TPresenter, TPanel> TemplateContract => (ToolBarControlBaseTemplateContract<TToolBarControl, TItem, TCollection, TPresenter, TPanel>) TemplateContractCore;

    #endregion

    #region  Methods

    private void InvalidateMenuButton()
    {
      var overflowMenuButton = MenuButton;

      if (overflowMenuButton != null)
        PanelUtils.InvalidateAncestorsMeasure(overflowMenuButton, this);
    }

    private void OnActualMenuButtonVisibilityChanged()
    {
      InvalidateMenuButton();
    }

    private void OnHasOverflowedItemsChanged()
    {
      UpdateActualMenuButtonVisible();
    }

    private void OnMenuButtonVisibilityChanged()
    {
      UpdateActualMenuButtonVisible();
    }

    protected virtual void UpdateActualMenuButtonVisible()
    {
      ActualMenuButtonVisibility = VisibilityUtils.EvaluateElementVisibility(MenuButtonVisibility, HasOverflowItems ? Visibility.Visible : Visibility.Collapsed);
    }

    #endregion
  }

  public class ToolBarControlBaseTemplateContract<TToolBarControl, TItem, TCollection, TPresenter, TPanel> : ItemsControlBaseTemplateContract<TPresenter>
    where TItem : NativeControl
		where TCollection : ItemCollectionBase<TToolBarControl, TItem>
    where TPresenter : ItemsPresenterBase<TToolBarControl, TItem, TCollection, TPanel>
    where TPanel : ItemsPanel<TItem>
    where TToolBarControl : NativeControl
	{
    #region Properties

    [TemplateContractPart(Required = false)]
    public DropDownButton MenuButton { get; [UsedImplicitly] private set; }

    #endregion
  }
}