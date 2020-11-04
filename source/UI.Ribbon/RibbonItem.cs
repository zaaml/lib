// <copyright file="RibbonItem.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using Zaaml.Core.Converters;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.UI.Controls.Core;
using Zaaml.UI.Controls.Primitives.ContentPrimitives;
using Zaaml.UI.Controls.Primitives.Overflow;
using Zaaml.UI.Panels.Core;

namespace Zaaml.UI.Controls.Ribbon
{
  [TemplateContractType(typeof(RibbonItemTemplateContract))]
  public abstract class RibbonItem : TemplateContractControl, IOverflowableItem<RibbonItem>
  {
    #region Static Fields and Constants

    private static readonly DependencyPropertyKey IsOverflowPropertyKey = DPM.RegisterReadOnly<bool, RibbonItem>
      ("IsOverflow", i => i.OnIsOverflowChanged);

    public static readonly DependencyProperty SizeDefinitionProperty = DPM.Register<RibbonItemSizeDefinition, RibbonItem>
      ("SizeDefinition", i => i.OnSizeDefinitionChanged);

    public static readonly DependencyProperty LargeIconProperty = DPM.Register<IconBase, RibbonItem>
      ("LargeIcon");

    public static readonly DependencyProperty SmallIconProperty = DPM.Register<IconBase, RibbonItem>
      ("SmallIcon");

    public static readonly DependencyProperty TextProperty = DPM.Register<string, RibbonItem>
      ("Text");

    private static readonly DependencyPropertyKey ActualItemStylePropertyKey = DPM.RegisterReadOnly<RibbonItemStyle, RibbonItem>
      ("ActualItemStyle", r => r.OnActualItemStyleChanged);

    private static readonly DependencyPropertyKey ToolBarPropertyKey = DPM.RegisterReadOnly<RibbonToolBar, RibbonItem>
      ("ToolBar", r => r.OnToolBarChanged);

    public static readonly DependencyProperty ToolBarProperty = ToolBarPropertyKey.DependencyProperty;
    public static readonly DependencyProperty ActualItemStyleProperty = ActualItemStylePropertyKey.DependencyProperty;
    public static readonly DependencyProperty IsOverflowProperty = IsOverflowPropertyKey.DependencyProperty;

    #endregion

    #region Ctors

    protected RibbonItem()
    {
      OverflowController = new OverflowItemController<RibbonItem>(this);
    }

    #endregion

    #region Properties

    public RibbonItemStyle ActualItemStyle
    {
      get => (RibbonItemStyle) GetValue(ActualItemStyleProperty);
      internal set => this.SetReadOnlyValue(ActualItemStylePropertyKey, value);
    }

    internal RibbonGroup Group { get; set; }

    public bool IsOverflow
    {
      get => (bool) GetValue(IsOverflowProperty);
      internal set => this.SetReadOnlyValue(IsOverflowPropertyKey, value);
    }

    internal RibbonItemMeasurement ItemMeasurement { get; } = new RibbonItemMeasurement();

    public IconBase LargeIcon
    {
      get => (IconBase) GetValue(LargeIconProperty);
      set => SetValue(LargeIconProperty, value);
    }

    internal OverflowItemController<RibbonItem> OverflowController { get; }

    [TypeConverter(typeof(RibbonItemSizeDefinitionTypeConverter))]
    public RibbonItemSizeDefinition SizeDefinition
    {
      get => (RibbonItemSizeDefinition) GetValue(SizeDefinitionProperty);
      set => SetValue(SizeDefinitionProperty, value);
    }

    public IconBase SmallIcon
    {
      get => (IconBase) GetValue(SmallIconProperty);
      set => SetValue(SmallIconProperty, value);
    }

    public string Text
    {
      get => (string) GetValue(TextProperty);
      set => SetValue(TextProperty, value);
    }


    public RibbonToolBar ToolBar
    {
      get => (RibbonToolBar) GetValue(ToolBarProperty);
      internal set => this.SetReadOnlyValue(ToolBarPropertyKey, value);
    }

    #endregion

    #region  Methods

    protected override Size ArrangeOverride(Size arrangeBounds)
    {
      var arrangeOverride = base.ArrangeOverride(arrangeBounds);
      return arrangeOverride;
    }

    internal void BeginMeasurePass()
    {
      ItemMeasurement.Reset();
      InvalidateMeasureInt();
    }

    internal void EndMeasurePass()
    {
    }

    protected internal virtual RibbonItemStyle GetDefaultRibbonItemStyle()
    {
      return RibbonItemStyle.Default;
    }

    internal void InvalidateMeasureInt()
    {
      var templateRoot = this.GetImplementationRoot() as RibbonItemTemplateRoot;

      if (templateRoot == null)
        return;

      foreach (var templatePart in templateRoot.LayoutDependsOnItemStyle)
      {
        var templateChild = GetTemplateChild(templatePart) as FrameworkElement;

        if (templateChild == null)
          continue;

        foreach (var frameworkElement in templateChild.GetVisualAncestorsAndSelf().OfType<FrameworkElement>())
        {
          frameworkElement.InvalidateMeasure();

          if (ReferenceEquals(frameworkElement, this))
            break;
        }
      }
    }

    protected override Size MeasureOverride(Size availableSize)
    {
      var measureOverride = base.MeasureOverride(availableSize);
      return measureOverride;
    }

    private void OnActualItemStyleChanged()
    {
      InvalidateMeasureInt();
    }

    private void OnIsOverflowChanged()
    {
      OverflowController.IsOverflow = IsOverflow;
    }

    private void OnSizeDefinitionChanged()
    {
      if (SizeDefinition != null)
        ActualItemStyle = SizeDefinition.ItemStyle;

      Group?.OnItemSizeDefinitionChanged(this);
    }

    private void OnToolBarChanged()
    {
      if (ToolBar != null)
        OverflowController.Attach();
      else
        OverflowController.Detach();
    }

    #endregion

    #region Interface Implementations

    #region IOverflowableItem

    bool IOverflowableItem.IsOverflow => IsOverflow;

    #endregion

    #region IOverflowableItem<RibbonItem>

    OverflowItemController<RibbonItem> IOverflowableItem<RibbonItem>.OverflowController => OverflowController;

    #endregion

    #endregion
  }

  internal static class RibbonItemExtensions
  {
    #region  Methods

    public static bool CanBeLarge(this RibbonItem item)
    {
      return RibbonUtils.CanBeLarge(item);
    }

    public static bool CanBeSmall(this RibbonItem item)
    {
      return RibbonUtils.CanBeSmall(item);
    }

    public static RibbonControlGroupSize GetAllowedGroupSize(this RibbonItem item, RibbonItemStyle itemStyle)
    {
      return RibbonUtils.GetAllowedGroupSize(item, itemStyle);
    }

    #endregion
  }

  public sealed class RibbonItemTemplateRoot : ControlTemplateRoot
  {
    #region Static Fields and Constants

    public static readonly DependencyProperty LayoutDependsOnItemStyleProperty = DPM.Register<StringCollection, RibbonItemTemplateRoot>
      ("LayoutDependsOnItemStyle");

    #endregion

    #region Properties

    [TypeConverter(typeof(StringCollectionTypeConverter))]
    public StringCollection LayoutDependsOnItemStyle
    {
      get { return this.GetValueOrCreate(LayoutDependsOnItemStyleProperty, () => new StringCollection()); }
      set => SetValue(LayoutDependsOnItemStyleProperty, value);
    }

    #endregion
  }

  public class RibbonItemTemplateContract : TemplateContract
  {
  }
}