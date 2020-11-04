// <copyright file="RibbonPage.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Zaaml.Core;
using Zaaml.Core.Converters;
using Zaaml.Core.Extensions;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.CommandCore;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Controls.Core;
using Zaaml.UI.Controls.Interfaces;
using Zaaml.UI.Controls.Primitives.ContentPrimitives;

namespace Zaaml.UI.Controls.Ribbon
{
  [TemplateContractType(typeof(RibbonPageTemplateContract))]
  public partial class RibbonPage : ItemsControlBase<RibbonPage, RibbonGroup, RibbonGroupCollection, RibbonGroupsPresenter, RibbonGroupsPanel>, ISelectionStateControl, ISelectable, IIconOwner
  {
    #region Static Fields and Constants

    public static readonly DependencyProperty HeaderProperty = DPM.Register<string, RibbonPage>
      ("Header");

    public static readonly DependencyProperty IconProperty = DPM.Register<IconBase, RibbonPage>
      ("Icon", i => i.LogicalChildMentor.OnLogicalChildPropertyChanged);

    public static readonly DependencyProperty IsSelectedProperty = DPM.Register<bool, RibbonPage>
      ("IsSelected", false, p => p.OnIsSelectedChanged);

    private static readonly DependencyPropertyKey RibbonPropertyKey = DPM.RegisterReadOnly<RibbonControl, RibbonPage>
      ("Ribbon", p => p.OnRibbonControlChanged);

    private static readonly DependencyPropertyKey PageCategoryPropertyKey = DPM.RegisterReadOnly<RibbonPageCategory, RibbonPage>
      ("PageCategory");

    public static readonly DependencyProperty PageCategoryProperty = PageCategoryPropertyKey.DependencyProperty;

    public static readonly DependencyProperty GroupSizeReductionOrderProperty = DPM.Register<StringCollection, RibbonPage>
      ("GroupSizeReductionOrder");

    public static readonly DependencyProperty RibbonProperty = RibbonPropertyKey.DependencyProperty;

    private static readonly DependencyPropertyKey ActualGroupsPresenterPropertyKey = DPM.RegisterReadOnly<RibbonGroupsPresenter, RibbonPage>
      ("ActualGroupsPresenter");

    internal static readonly DependencyProperty ActualGroupsPresenterProperty = ActualGroupsPresenterPropertyKey.DependencyProperty;

    #endregion

    #region Ctors

    static RibbonPage()
    {
      DefaultStyleKeyHelper.OverrideStyleKey<RibbonPage>();
    }

    public RibbonPage()
    {
      this.OverrideStyleKey<RibbonPage>();
      SelectCommand = new RelayCommand(OnSelectCommandExecute, () => true);
    }

    #endregion

    #region Properties

    internal IEnumerable<RibbonGroup> ActualGroupSizeReductionOrder => GroupSizeReductionOrder == null ? Items.Reverse() : GroupSizeReductionOrder.Cast<string>().Select(GetRibbonGroup).SkipNull();

    public RibbonGroupsPresenter ActualGroupsPresenter
    {
      get => (RibbonGroupsPresenter) GetValue(ActualGroupsPresenterProperty);
      private set => this.SetReadOnlyValue(ActualGroupsPresenterPropertyKey, value);
    }

    [TypeConverter(typeof(StringCollectionTypeConverter))]
    public StringCollection GroupSizeReductionOrder
    {
      get => (StringCollection) GetValue(GroupSizeReductionOrderProperty);
      set => SetValue(GroupSizeReductionOrderProperty, value);
    }

    internal RibbonGroupsPresenterHost GroupsPresenterHost => TemplateContract.GroupsPresenterHost;

    public string Header
    {
      get => (string) GetValue(HeaderProperty);
      set => SetValue(HeaderProperty, value);
    }

    public RibbonPageCategory PageCategory
    {
      get => (RibbonPageCategory) GetValue(PageCategoryProperty);
      internal set => this.SetReadOnlyValue(PageCategoryPropertyKey, value);
    }

    public RibbonControl Ribbon
    {
      get => (RibbonControl) GetValue(RibbonProperty);
      internal set => this.SetReadOnlyValue(RibbonPropertyKey, value);
    }

    public RelayCommand SelectCommand { get; }

    private RibbonPageTemplateContract TemplateContract => (RibbonPageTemplateContract) TemplateContractInternal;

    #endregion

    #region  Methods

    private void Activate()
    {
      SetIsSelectedInt(true);
      Ribbon?.Activate(this);
    }

    protected override RibbonGroupCollection CreateItemCollection()
    {
      return new RibbonGroupCollection(this);
    }

    private RibbonGroup GetRibbonGroup(string name)
    {
      var ribbonGroup = FindName(name) as RibbonGroup ?? this.GetTemplateElement(name) as RibbonGroup;
      return ribbonGroup;
    }

    protected override void OnGotFocus(RoutedEventArgs e)
    {
      base.OnGotFocus(e);

#if !SILVERLIGHT
      if (e.Handled)
        return;

      e.Handled = true;
#endif

      if (Ribbon?.IsInitializing == false)
        Activate();
    }

    protected virtual void OnIsSelectedChanged()
    {
      UpdateVisualState(true);
      var selected = IsSelected;

      if (selected)
        Ribbon?.Select(this);

      UpdateVisualState(true);

      if (selected)
        RaiseSelectedEvent();
      else
        RaiseUnselectedEvent();

      if (selected == IsSelected)
        IsSelectedChanged?.Invoke(this, EventArgs.Empty);
    }

    internal override void OnItemAttachedInternal(RibbonGroup item)
    {
      item.Page = this;

      base.OnItemAttachedInternal(item);
    }

    internal override void OnItemDetachedInternal(RibbonGroup item)
    {
      base.OnItemDetachedInternal(item);

      item.Page = null;
    }

    protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
    {
      if (e.Handled)
        return;

      e.Handled = true;
      Activate();
    }

    protected virtual void OnRibbonControlChanged()
    {
    }

    private void OnSelectCommandExecute()
    {
      Activate();
    }

    protected override void OnTemplateContractAttached()
    {
      base.OnTemplateContractAttached();

      ItemsPresenter.Page = this;
      GroupsPresenterHost.Page = this;
      ActualGroupsPresenter = ItemsPresenter;
    }

    protected override void OnTemplateContractDetaching()
    {
      ItemsPresenter.Page = null;
      ActualGroupsPresenter = null;
      GroupsPresenterHost.Page = null;

      base.OnTemplateContractDetaching();
    }

    private void SetIsSelectedInt(bool isSelected)
    {
      this.SetCurrentValueInternal(IsSelectedProperty, isSelected ? KnownBoxes.BoolTrue : KnownBoxes.BoolFalse);
    }

    #endregion

    #region Interface Implementations

    #region IIconOwner

    public IconBase Icon
    {
      get => (IconBase) GetValue(IconProperty);
      set => SetValue(IconProperty, value);
    }

    #endregion

    #region ISelectable

    public event EventHandler IsSelectedChanged;

    bool ISelectable.IsSelected
    {
      get => IsSelected;
      set => SetIsSelectedInt(value);
    }

    #endregion

    #region ISelectionStateControl

    public bool IsSelected
    {
      get => (bool) GetValue(IsSelectedProperty);
      set => SetValue(IsSelectedProperty, value);
    }

    #endregion

    #endregion
  }

  public sealed class RibbonPageTemplateContract : ItemsControlBaseTemplateContract<RibbonGroupsPresenter>
  {
    #region Properties

    [TemplateContractPart(Required = true)]
    public RibbonGroupsPresenterHost GroupsPresenterHost { get; [UsedImplicitly] private set; }

    #endregion
  }
}