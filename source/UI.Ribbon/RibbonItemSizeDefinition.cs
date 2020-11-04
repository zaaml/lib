// <copyright file="RibbonItemSizeDefinition.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using Zaaml.Core.Interfaces;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.UI.Controls.Ribbon
{
  public class RibbonItemSizeDefinition : DependencyObject, ISealable
  {
    #region Static Fields and Constants

    public static readonly DependencyProperty IsFlexibleProperty = DPM.Register<bool, RibbonItemSizeDefinition>
      ("IsFlexible");

    public static readonly DependencyProperty MinWidthProperty = DPM.Register<double, RibbonItemSizeDefinition>
      ("MinWidth");

    public static readonly DependencyProperty MaxWidthProperty = DPM.Register<double, RibbonItemSizeDefinition>
      ("MaxWidth");

    public static readonly DependencyProperty WidthProperty = DPM.Register<double, RibbonItemSizeDefinition>
      ("Width");

    public static readonly DependencyProperty ItemStyleProperty = DPM.Register<RibbonItemStyle, RibbonItemSizeDefinition>
      ("ItemStyle");

    public static readonly RibbonItemSizeDefinition DefaultDefinition = new RibbonItemSizeDefinition();

    #endregion

    #region Fields

    private bool _isSealed;

    #endregion

    #region Ctors

    static RibbonItemSizeDefinition()
    {
      ((ISealable) DefaultDefinition).Seal();
    }

    #endregion

    #region Properties

    public bool IsFlexible
    {
      get => (bool) GetValue(IsFlexibleProperty);
      set => SetValue(IsFlexibleProperty, value);
    }

    public RibbonItemStyle ItemStyle
    {
      get => (RibbonItemStyle) GetValue(ItemStyleProperty);
      set => SetValue(ItemStyleProperty, value);
    }

    public double MaxWidth
    {
      get => (double) GetValue(MaxWidthProperty);
      set => SetValue(MaxWidthProperty, value);
    }

    public double MinWidth
    {
      get => (double) GetValue(MinWidthProperty);
      set => SetValue(MinWidthProperty, value);
    }

    public double Width
    {
      get => (double) GetValue(WidthProperty);
      set => SetValue(WidthProperty, value);
    }

    #endregion

    #region  Methods

    public override string ToString()
    {
      return ItemStyle.ToString();
    }

    public static bool TryParse(string strValue, out RibbonItemSizeDefinition itemDefinition)
    {
      itemDefinition = null;
      strValue = strValue.Trim();

      if (strValue.Equals("L", StringComparison.OrdinalIgnoreCase) || strValue.Equals("Large", StringComparison.OrdinalIgnoreCase))
      {
        itemDefinition = new RibbonItemSizeDefinition {ItemStyle = RibbonItemStyle.Large};
        return true;
      }

      if (strValue.Equals("M", StringComparison.OrdinalIgnoreCase) || strValue.Equals("Medium", StringComparison.OrdinalIgnoreCase))
      {
        itemDefinition = new RibbonItemSizeDefinition {ItemStyle = RibbonItemStyle.Medium};
        return true;
      }

      if (strValue.Equals("S", StringComparison.OrdinalIgnoreCase) || strValue.Equals("Small", StringComparison.OrdinalIgnoreCase))
      {
        itemDefinition = new RibbonItemSizeDefinition {ItemStyle = RibbonItemStyle.Small};
        return true;
      }

      return false;
    }

    #endregion

    #region Interface Implementations

    #region ISealable

    bool ISealable.IsSealed => _isSealed;

    void ISealable.Seal()
    {
      _isSealed = true;
    }

    #endregion

    #endregion
  }

  public class RibbonItemSizeDefinitionTypeConverter : TypeConverter
  {
    #region  Methods

    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
    {
      return sourceType == typeof(string);
    }

    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
      var strValue = value as string;
      RibbonItemSizeDefinition ribbonSizeDefinition;
      if (strValue != null && RibbonItemSizeDefinition.TryParse(strValue, out ribbonSizeDefinition))
        return ribbonSizeDefinition;

      return base.ConvertFrom(context, culture, value);
    }

    #endregion
  }
}