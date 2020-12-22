// <copyright file="RibbonControl.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using Zaaml.Core;
using Zaaml.Core.Packed;
using Zaaml.Core.Utils;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.Data;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Controls.Core;
using Zaaml.UI.Controls.Menu;
using Zaaml.UI.Controls.Primitives;

namespace Zaaml.UI.Controls.Ribbon
{
  [TemplateContractType(typeof(RibbonControlTemplateContract))]
  public class RibbonControl : ItemsControlBase<RibbonControl, RibbonPageCategory, RibbonPageCategoryCollection, RibbonPageCategoriesPresenter, RibbonPageCategoriesPanel>, ISupportInitialize, ISelector<RibbonPage>
  {
    #region Static Fields and Constants

    public static readonly DependencyProperty ApplicationMenuProperty = DPM.Register<ApplicationMenu, RibbonControl>
      ("ApplicationMenu", r => r.OnApplicationMenuChanged);

    public static readonly DependencyProperty IsApplicationMenuOpenProperty = DPM.Register<bool, RibbonControl>
      ("IsApplicationMenuOpen", r => r.OnIsApplicationMenuOpenChanged);

    public static readonly DependencyProperty SelectedPageProperty = DPM.Register<RibbonPage, RibbonControl>
      ("SelectedPage", s => s.SelectorController.OnSelectedItemPropertyChanged, s => s.SelectorController.CoerceSelectedItem);

    public static readonly DependencyProperty SelectedPageIndexProperty = DPM.Register<int, RibbonControl>
      ("SelectedPageIndex", -1, s => s.SelectorController.OnSelectedIndexPropertyChanged, s => s.SelectorController.CoerceSelectedIndex);

    private static readonly DependencyPropertyKey QuickAccessToolBarPropertyKey = DPM.RegisterReadOnly<RibbonToolBar, RibbonControl>
      ("QuickAccessToolBar");

    public static readonly DependencyProperty QuickAccessToolBarProperty = QuickAccessToolBarPropertyKey.DependencyProperty;

    #endregion

    #region Fields

    private byte _packedValue;
    private RibbonPresenter _presenter;
    private bool _suspendApplicationMenuIsOpenChanged;

    #endregion

    #region Ctors

    static RibbonControl()
    {
      DefaultStyleKeyHelper.OverrideStyleKey<RibbonControl>();
    }

    public RibbonControl()
    {
      this.OverrideStyleKey<RibbonControl>();

      QuickAccessToolBar = new RibbonToolBar();
      QuickAccessToolBar.SetBinding(Extension.SkinProperty, new Binding {Path = new PropertyPath(RibbonControlStyling.QuickAccessToolBarSkinProperty), Source = this});

      var ribbonPageCategory = new RibbonPageCategory(this);

      Pages = new RibbonPageCollection(ribbonPageCategory);

      SelectorController = new SelectorController<RibbonControl, RibbonPage>(this,  new RibbonControlSelectorAdvisor(this, ribbonPageCategory))
      {
        AllowNullSelection = false
      };
    }

    #endregion

    #region Properties

    internal bool AllowNullSelection
    {
      get => PackedDefinition.AllowNullSelection.GetValue(_packedValue);
      set
      {
        if (AllowNullSelection == value)
          return;

        PackedDefinition.AllowNullSelection.SetValue(ref _packedValue, value);
        SelectorController.AllowNullSelection = value;
      }
    }

    public ApplicationMenu ApplicationMenu
    {
      get => (ApplicationMenu) GetValue(ApplicationMenuProperty);
      set => SetValue(ApplicationMenuProperty, value);
    }

    private RibbonGroupsPresenterHost GroupsPresenterHost => TemplateContract.GroupsPresenterHost;

    private RibbonHeaderPresenter HeaderPresenter => TemplateContract.HeaderPresenter;

    public bool IsApplicationMenuOpen
    {
      get => (bool) GetValue(IsApplicationMenuOpenProperty);
      set => SetValue(IsApplicationMenuOpenProperty, value);
    }

    internal bool IsInitializing
    {
      get => PackedDefinition.IsInitializing.GetValue(_packedValue);
      private set => PackedDefinition.IsInitializing.SetValue(ref _packedValue, value);
    }

    protected override IEnumerator LogicalChildren => ApplicationMenu != null ? EnumeratorUtils.Concat(ApplicationMenu, base.LogicalChildren) : base.LogicalChildren;

    private RibbonPageCategoriesPresenter PageCategoriesPresenter => TemplateContract.ItemsPresenter;

    internal RibbonPageCollection Pages { get; }

    internal RibbonPagesPresenter PagesPresenter => TemplateContract.PagesPresenter;

    internal bool PreferSelection
    {
      get => PackedDefinition.PreferSelection.GetValue(_packedValue);
      set
      {
        if (PreferSelection == value)
          return;

        PackedDefinition.PreferSelection.SetValue(ref _packedValue, value);
        SelectorController.PreferSelection = value;
      }
    }

    internal RibbonPresenter Presenter
    {
      get => _presenter;
      set
      {
        if (ReferenceEquals(_presenter, value))
          return;

        _presenter = value;

        if (HeaderPresenter == null)
          return;

        if (_presenter != null)
          InitHeaderPresenter();
        else
          CleanHeaderPresenter();
      }
    }

    public RibbonItemCollection QuickAccessItems => QuickAccessToolBar.Items;

    public RibbonToolBar QuickAccessToolBar
    {
      get => (RibbonToolBar) GetValue(QuickAccessToolBarProperty);
      private set => this.SetReadOnlyValue(QuickAccessToolBarPropertyKey, value);
    }

    public RibbonPage SelectedPage
    {
      get => (RibbonPage) GetValue(SelectedPageProperty);
      set => SetValue(SelectedPageProperty, value);
    }

    public int SelectedPageIndex
    {
      get => (int) GetValue(SelectedPageIndexProperty);
      set => SetValue(SelectedPageIndexProperty, value);
    }

    internal SelectorController<RibbonControl, RibbonPage> SelectorController { get; }

    private RibbonControlTemplateContract TemplateContract => (RibbonControlTemplateContract) TemplateContractInternal;

    #endregion

    #region  Methods

    internal void Activate(RibbonPage ribbonPage)
    {
      if (ReferenceEquals(SelectedPage, ribbonPage) == false)
        SelectorController.SelectItem(ribbonPage);

      HideBackContent();
    }

    private void CleanHeaderPresenter()
    {
      HeaderPresenter.ClearValue(RibbonHeaderPresenter.HeaderProperty);
      HeaderPresenter.ClearValue(RibbonHeaderPresenter.FooterProperty);
      HeaderPresenter.ClearValue(RibbonHeaderPresenter.TitleProperty);
    }

    protected override RibbonPageCategoryCollection CreateItemCollection()
    {
      return new RibbonPageCategoryCollection(this);
    }

    private void HideBackContent()
    {
      IsApplicationMenuOpen = false;
    }

    private void InitHeaderPresenter()
    {
      HeaderPresenter.BindProperties(RibbonHeaderPresenter.HeaderProperty, _presenter, RibbonPresenter.HeaderProperty);
      HeaderPresenter.BindProperties(RibbonHeaderPresenter.FooterProperty, _presenter, RibbonPresenter.FooterProperty);
      HeaderPresenter.BindProperties(RibbonHeaderPresenter.TitleProperty, _presenter, RibbonPresenter.TitleProperty);
    }

    private void OnApplicationMenuChanged(ApplicationMenu oldMenu, ApplicationMenu newMenu)
    {
      if (oldMenu != null)
      {
        oldMenu.IsOpenChanged -= OnApplicationMenuIsOpenChanged;

        RemoveLogicalChild(oldMenu);
      }

      if (newMenu != null)
      {
        AddLogicalChild(newMenu);

        newMenu.IsOpenChanged += OnApplicationMenuIsOpenChanged;
      }
    }

    private void OnApplicationMenuIsOpenChanged(object sender, EventArgs e)
    {
      if (_suspendApplicationMenuIsOpenChanged)
        return;

      try
      {
        _suspendApplicationMenuIsOpenChanged = true;

        IsApplicationMenuOpen = ApplicationMenu.IsOpen;
      }
      finally
      {
        _suspendApplicationMenuIsOpenChanged = false;
      }
    }

    private void OnIsApplicationMenuOpenChanged()
    {
      if (_suspendApplicationMenuIsOpenChanged)
        return;

      try
      {
        _suspendApplicationMenuIsOpenChanged = true;

        if (ApplicationMenu != null)
          ApplicationMenu.IsOpen = IsApplicationMenuOpen;
      }
      finally
      {
        _suspendApplicationMenuIsOpenChanged = false;
      }
    }

    internal override void OnItemAttachedInternal(RibbonPageCategory item)
    {
      item.Ribbon = this;

      base.OnItemAttachedInternal(item);
    }

    internal override void OnItemDetachedInternal(RibbonPageCategory item)
    {
      base.OnItemDetachedInternal(item);

      item.Ribbon = null;
    }

    protected override void OnTemplateContractAttached()
    {
      base.OnTemplateContractAttached();

      PagesPresenter.Ribbon = this;
      HeaderPresenter.Ribbon = this;
      PageCategoriesPresenter.Ribbon = this;
      GroupsPresenterHost.Ribbon = this;

      InitHeaderPresenter();
      UpdateSelectedGroupContainer();
    }

    protected override void OnTemplateContractDetaching()
    {
      GroupsPresenterHost.Ribbon = null;
      PagesPresenter.Ribbon = null;
      PageCategoriesPresenter.Ribbon = null;
      HeaderPresenter.Ribbon = null;

      GroupsPresenterHost.ClearValue(RibbonGroupsPresenterHost.GroupsPresenterProperty);

      CleanHeaderPresenter();

      base.OnTemplateContractDetaching();
    }

    internal void Select(RibbonPage ribbonPage)
    {
    }

    private void UpdatePreferSelection()
    {
      PreferSelection = IsApplicationMenuOpen == false;
    }

    internal void UpdateSelectedGroupContainer()
    {
      if (GroupsPresenterHost == null)
        return;

      if (SelectedPage == null)
        GroupsPresenterHost.ClearValue(RibbonGroupsPresenterHost.GroupsPresenterProperty);
      else
        GroupsPresenterHost.BindProperties(RibbonGroupsPresenterHost.GroupsPresenterProperty, SelectedPage, RibbonPage.ActualGroupsPresenterProperty);
    }

    #endregion

    #region Interface Implementations

    #region ISelector<RibbonPage>

    DependencyProperty ISelector<RibbonPage>.SelectedIndexProperty => SelectedPageIndexProperty;

    DependencyProperty ISelector<RibbonPage>.SelectedItemProperty => SelectedPageProperty;

    DependencyProperty ISelector<RibbonPage>.SelectedSourceProperty => null;

    DependencyProperty ISelector<RibbonPage>.SelectedValueProperty => null;

    object ISelector<RibbonPage>.GetValue(RibbonPage item, object source)
    {
      return null;
    }

    void ISelector<RibbonPage>.OnSelectedIndexChanged(int oldIndex, int newIndex)
    {
    }

    void ISelector<RibbonPage>.OnSelectedItemChanged(RibbonPage oldItem, RibbonPage newItem)
    {
      UpdateSelectedGroupContainer();
    }

    void ISelector<RibbonPage>.OnSelectedSourceChanged(object oldSource, object newSource)
    {
    }

    void ISelector<RibbonPage>.OnSelectedValueChanged(object oldValue, object newValue)
    {
    }

    void ISelector<RibbonPage>.OnSelectionChanged(Selection<RibbonPage> oldSelection, Selection<RibbonPage> newSelection)
    {
    }

    #endregion

    #region ISupportInitialize

    void ISupportInitialize.BeginInit()
    {
#if !SILVERLIGHT
      BeginInit();
#endif
      IsInitializing = true;
			SelectorController.BeginInit();
			PreferSelection = false;
    }

    void ISupportInitialize.EndInit()
    {
      IsInitializing = false;
      SelectorController.EndInit();
      UpdatePreferSelection();

#if !SILVERLIGHT
      EndInit();
#endif
    }

    #endregion

    #endregion

    #region  Nested Types

    private static class PackedDefinition
    {
      #region Static Fields and Constants

      public static readonly PackedBoolItemDefinition IsInitializing;
      public static readonly PackedBoolItemDefinition AllowNullSelection;
      public static readonly PackedBoolItemDefinition PreferSelection;

      #endregion

      #region Ctors

      static PackedDefinition()
      {
        var allocator = new PackedValueAllocator();

        IsInitializing = allocator.AllocateBoolItem();
        AllowNullSelection = allocator.AllocateBoolItem();
        PreferSelection = allocator.AllocateBoolItem();
      }

      #endregion
    }

    #endregion
  }

  public class RibbonControlTemplateContract : ItemsControlBaseTemplateContract<RibbonPageCategoriesPresenter>
  {
    #region Properties

    [TemplateContractPart(Required = true)]
    public ToggleButton ApplicationMenuButton { get; [UsedImplicitly] private set; }

    [TemplateContractPart(Required = true)]
    public RibbonGroupsPresenterHost GroupsPresenterHost { get; [UsedImplicitly] private set; }

    [TemplateContractPart(Required = true)]
    public RibbonHeaderPresenter HeaderPresenter { get; [UsedImplicitly] private set; }

    [TemplateContractPart(Required = true)]
    public RibbonPagesPresenter PagesPresenter { get; [UsedImplicitly] private set; }

    #endregion
  }
}