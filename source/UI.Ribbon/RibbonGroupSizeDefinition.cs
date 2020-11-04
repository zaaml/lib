// <copyright file="RibbonGroupSizeDefinition.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Markup;

namespace Zaaml.UI.Controls.Ribbon
{
  [ContentProperty("Definitions")]
  public class RibbonGroupSizeDefinition
  {
    #region Fields

    private RibbonItemSizeDefinitionCollection _definitions;

    #endregion

    #region Properties

    [TypeConverter(typeof(RibbonItemSizeDefinitionCollectionTypeConverter))]
    public RibbonItemSizeDefinitionCollection Definitions
    {
      get => _definitions ?? (_definitions = new RibbonItemSizeDefinitionCollection());
      set => _definitions = value;
    }

    #endregion
  }

  public class RibbonItemSizeDefinitionCollection : List<RibbonItemSizeDefinition>
  {
  }

  public class RibbonItemSizeDefinitionCollectionTypeConverter : TypeConverter
  {
    #region Static Fields and Constants

    private static readonly char[] Separators = {','};

    #endregion

    #region  Methods

    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
    {
      return sourceType == typeof(string);
    }

    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
      var ribbonItemSizeDefinitionCollection = new RibbonItemSizeDefinitionCollection();

      var strValue = value as string;
      if (strValue != null)
      {
        var splitted = strValue.Split(Separators, StringSplitOptions.RemoveEmptyEntries);
        foreach (var s in splitted)
        {
          RibbonItemSizeDefinition itemDefinition;
          if (RibbonItemSizeDefinition.TryParse(s, out itemDefinition))
            ribbonItemSizeDefinitionCollection.Add(itemDefinition);
          else
            throw new ArgumentException("Can not convert specified string value to RibbonItemSizeDefinitionCollection");
        }
      }
      else
        throw new ArgumentException("Converter argument exception. Argument of string type expected");

      return ribbonItemSizeDefinitionCollection;
    }

    #endregion
  }
}