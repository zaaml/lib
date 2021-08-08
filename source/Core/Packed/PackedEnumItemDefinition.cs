// <copyright file="PackedEnumItemDefinition.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Linq;
using Zaaml.Core.Converters;
using Zaaml.Core.Utils;

namespace Zaaml.Core.Packed
{
  internal class PackedEnumItemDefinition<TEnum> : PackedItemDefinition<TEnum> where TEnum : struct, Enum, IConvertible
  {
    #region Static Fields and Constants

    // ReSharper disable once StaticMemberInGenericType
    private static readonly int EnumBitCount;
    private static readonly TableEnumConverter Converter;

    #endregion

    #region Ctors

    static PackedEnumItemDefinition()
    {
      EnumBitCount = 1 + BitUtils.MostSignificantPosition(Enum.GetValues(typeof(TEnum)).Cast<TEnum>().Select(EnumConverter<TEnum>.Convert).Max());

      if (EnumBitCount < 9)
        Converter = new TableEnumConverter();
    }

    internal PackedEnumItemDefinition(int bitOffset)
      : base(bitOffset, EnumBitCount)
    {
      if (typeof(TEnum).IsEnum == false)
        throw new InvalidOperationException("Only enum type is allowed");
    }

    #endregion

    #region  Methods

    protected override TEnum Convert(ulong value)
    {
      return Converter?.Convert((int) value) ?? EnumConverter<TEnum>.Convert((int) value);
    }

    protected override ulong Convert(TEnum value)
    {
      return (ulong) EnumConverter<TEnum>.Convert(value);
    }

    #endregion

    #region  Nested Types

    private class TableEnumConverter
    {
      #region Fields

      private readonly TEnum[] _convertTable;

      #endregion

      #region Ctors

      public TableEnumConverter()
      {
        _convertTable = new TEnum[1 << EnumBitCount];
        for (var i = 0; i < _convertTable.Length; i++)
        {
          try
          {
            _convertTable[i] = EnumConverter<TEnum>.Convert(i);
          }
          catch (Exception e)
          {
            LogService.LogError(e);
          }
        }
      }

      #endregion

      #region  Methods

      public TEnum Convert(int value)
      {
        return _convertTable[value];
      }

      #endregion
    }

    #endregion
  }
}