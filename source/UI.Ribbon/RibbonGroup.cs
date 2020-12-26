// <copyright file="RibbonGroup.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Zaaml.Core;
using Zaaml.Core.Extensions;
using Zaaml.Core.Packed;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Controls.Core;
using Zaaml.UI.Controls.Primitives.ContentPrimitives;
using ContentPresenter = System.Windows.Controls.ContentPresenter;

namespace Zaaml.UI.Controls.Ribbon
{
  [TemplateContractType(typeof(RibbonGroupTemplateContract))]
  public class RibbonGroup : ItemsControlBase<Control, RibbonItem, RibbonItemCollection, RibbonItemsPresenter, RibbonItemsPanel>
  {
    #region Static Fields and Constants

    private static readonly DependencyPropertyKey PagePropertyKey = DPM.RegisterReadOnly<RibbonPage, RibbonGroup>
      ("Page");

    public static readonly DependencyProperty PageProperty = PagePropertyKey.DependencyProperty;

    private static readonly DependencyProperty GroupSizeDefinitionsProperty = DPM.Register<RibbonGroupSizeDefinitionCollection, RibbonGroup>
      ("GroupSizeDefinitionsInt");

    public static readonly DependencyProperty HeaderProperty = DPM.Register<string, RibbonGroup>
      ("Header");

    private static readonly DependencyPropertyKey IsCollapsedPropertyKey = DPM.RegisterReadOnly<bool, RibbonGroup>
      ("IsCollapsed", g => g.OnIsCollapsedChanged);

    public static readonly DependencyProperty IsDropDownOpenProperty = DPM.Register<bool, RibbonGroup>
      ("IsDropDownOpen");

    public static readonly DependencyProperty IconProperty = DPM.Register<IconBase, RibbonGroup>
      ("Icon", i => i.LogicalChildMentor.OnLogicalChildPropertyChanged);

    public static readonly DependencyProperty IsCollapsedProperty = IsCollapsedPropertyKey.DependencyProperty;

    #endregion

    #region Fields

    private readonly List<RibbonControlGroupCollection> _reducedGroups = new List<RibbonControlGroupCollection> {null};
    private byte _packedValue;
    private int _reduceLevel;
    private int _reduceLevelsCount;

    #endregion

    #region Ctors

    static RibbonGroup()
    {
      DefaultStyleKeyHelper.OverrideStyleKey<RibbonGroup>();
    }

    public RibbonGroup()
    {
      this.OverrideStyleKey<RibbonGroup>();
    }

    #endregion

    #region Properties

    private ContentPresenter DropDownContentPresenter => TemplateContract.DropDownContentPresenter;

    public RibbonGroupSizeDefinitionCollection GroupSizeDefinitions
    {
      get { return this.GetValueOrCreate(GroupSizeDefinitionsProperty, () => new RibbonGroupSizeDefinitionCollection()); }
    }

    public string Header
    {
      get => (string) GetValue(HeaderProperty);
      set => SetValue(HeaderProperty, value);
    }

    public IconBase Icon
    {
      get => (IconBase) GetValue(IconProperty);
      set => SetValue(IconProperty, value);
    }

    public bool IsCollapsed
    {
      get => (bool) GetValue(IsCollapsedProperty);
      private set => this.SetReadOnlyValue(IsCollapsedPropertyKey, value);
    }

    public bool IsDropDownOpen
    {
      get => (bool) GetValue(IsDropDownOpenProperty);
      set => SetValue(IsDropDownOpenProperty, value);
    }

    internal bool IsFinalMeasure
    {
      get => PackedDefinition.IsFinalMeasure.GetValue(_packedValue);
      set => PackedDefinition.IsFinalMeasure.SetValue(ref _packedValue, value);
    }

    internal bool IsFullMeasurePass
    {
      get => PackedDefinition.IsFullMeasurePass.GetValue(_packedValue);
      private set => PackedDefinition.IsFullMeasurePass.SetValue(ref _packedValue, value);
    }

    private ContentPresenter MainContentPresenter => TemplateContract.MainContentPresenter;

    public RibbonPage Page
    {
      get => this.GetValue<RibbonPage>(PageProperty);
      internal set => this.SetReadOnlyValue(PagePropertyKey, value);
    }

    protected internal virtual int ReduceLevel
    {
      get => _reduceLevel;
      set
      {
        if (_reduceLevel < 0 || _reduceLevel > _reduceLevelsCount)
          throw new ArgumentOutOfRangeException();

        _reduceLevel = value;

        IsCollapsed = _reduceLevel == _reduceLevelsCount;
      }
    }

    protected internal virtual int ReduceLevelsCount => _reduceLevelsCount + 1;

    private IRibbonGroupReducer Reducer => GroupSizeDefinitions.Any() ? GroupSizeDefinitions : DefaultRibbonGroupReducer.Instance;

    private RibbonItemsPresenter RibbonItemsPresenter => TemplateContract.ItemsPresenter;

    private RibbonGroupTemplateContract TemplateContract => (RibbonGroupTemplateContract) TemplateContractInternal;

    #endregion

    #region  Methods

    internal virtual void BeginMeasurePass()
    {
      IsFullMeasurePass = true;
      _reduceLevelsCount = Reducer.GetReduceLevelsCount(ItemCollection);
      _reducedGroups.Clear();
      _reducedGroups.AddRange(Enumerable.Range(0, _reduceLevelsCount).Select(i => (RibbonControlGroupCollection) null));

      ReduceLevel = 0;

      foreach (var ribbonItem in ItemCollection)
        ribbonItem.BeginMeasurePass();
    }


    protected override RibbonItemCollection CreateItemCollection()
    {
      return new RibbonItemCollection(this);
    }

    internal virtual void EndMeasurePass()
    {
      foreach (var ribbonItem in ItemCollection)
        ribbonItem.EndMeasurePass();

      IsFullMeasurePass = false;
    }

    internal RibbonControlGroupCollection GetGroups()
    {
      if (IsCollapsed)
        return _reducedGroups[0];

      return _reducedGroups[ReduceLevel] ?? (_reducedGroups[ReduceLevel] = Reducer.Reduce(ItemCollection, ReduceLevel));
    }

    internal void Invalidate()
    {
      RibbonItemsPresenter?.ItemsHostBaseInternal?.InvalidateMeasure();
    }

    internal void InvalidateInt(RibbonGroupsPanel ribbonGroupPanel)
    {
      RibbonItemsPresenter?.InvalidateInt(ribbonGroupPanel);

      foreach (var ribbonItem in ItemCollection)
        ribbonItem.InvalidateMeasureInt();
    }

    internal Size MeasureInt(Size availableSize, bool force)
    {
      IsFinalMeasure = force;
      Measure(availableSize);
      return DesiredSize;
    }

    private void OnIsCollapsedChanged()
    {
      UpdateItemsHostPanelLayout();
      if (IsCollapsed == false)
        IsDropDownOpen = false;
    }

    internal override void OnItemAttachedInternal(RibbonItem item)
    {
      item.Group = this;
      base.OnItemAttachedInternal(item);
    }

    internal override void OnItemDetachedInternal(RibbonItem item)
    {
      base.OnItemDetachedInternal(item);
      item.Group = null;
    }

    internal void OnItemSizeDefinitionChanged(RibbonItem ribbonItem)
    {
    }

    protected override void OnTemplateContractAttached()
    {
      base.OnTemplateContractAttached();
      RibbonItemsPresenter.Group = this;

      UpdateItemsHostPanelLayout();
    }

    protected override void OnTemplateContractDetaching()
    {
      RibbonItemsPresenter.Group = null;
      base.OnTemplateContractDetaching();
    }

    internal double Shrink(double target)
    {
      var initial = target;
      foreach (var controlGroup in GetGroups())
      {
        target -= controlGroup.Shrink(target);
        if (target.IsZero(XamlConstants.LayoutComparisonPrecision))
          break;
      }

      return initial - target;
    }

    public override string ToString()
    {
      return Name ?? "NoName";
    }

    private void UpdateItemsHostPanelLayout()
    {
      if (IsTemplateAttached == false)
        return;

      if (IsCollapsed)
      {
        MainContentPresenter.Content = null;
        DropDownContentPresenter.Content = RibbonItemsPresenter;
      }
      else
      {
        DropDownContentPresenter.Content = null;
        MainContentPresenter.Content = RibbonItemsPresenter;
      }
    }

    #endregion

    #region  Nested Types

    private static class PackedDefinition
    {
      #region Static Fields and Constants

      public static readonly PackedBoolItemDefinition IsFinalMeasure;
      public static readonly PackedBoolItemDefinition IsFullMeasurePass;

      #endregion

      #region Ctors

      static PackedDefinition()
      {
        var allocator = new PackedValueAllocator();

        IsFinalMeasure = allocator.AllocateBoolItem();
        IsFullMeasurePass = allocator.AllocateBoolItem();
      }

      #endregion
    }

    #endregion
  }

  public sealed class RibbonGroupTemplateContract : ItemsControlBaseTemplateContract<RibbonItemsPresenter>
  {
    #region Properties

    [TemplateContractPart(Required = true)]
    public ContentPresenter DropDownContentPresenter { get; [UsedImplicitly] private set; }

    [TemplateContractPart(Required = true)]
    public ContentPresenter MainContentPresenter { get; [UsedImplicitly] private set; }

    #endregion
  }
}