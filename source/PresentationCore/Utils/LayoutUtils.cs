// <copyright file="LayoutUtils.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.Core.Utils;

// ReSharper disable MemberHidesStaticFromOuterClass

namespace Zaaml.PresentationCore.Utils
{
  internal static class LayoutUtils
  {
    #region Properties

    private static ILayoutRoundImpl LayoutRoundImpl { get; } = DpiUtils.DpiX == 96 && DpiUtils.DpiY == 96 ? (ILayoutRoundImpl) new StandardDpiLayoutRoundImpl() : new NonStandardDpiLayoutRoundImpl();

    #endregion

    #region  Methods

    public static double RoundX(double value, RoundingMode roundingMode)
    {
      return LayoutRoundImpl.RoundX(value, roundingMode);
    }

    public static double RoundY(double value, RoundingMode roundingMode)
    {
      return LayoutRoundImpl.RoundY(value, roundingMode);
    }

    #endregion

    #region  Nested Types

    private interface ILayoutRoundImpl
    {
      #region  Methods

      double RoundX(double value, RoundingMode roundingMode);

      double RoundY(double value, RoundingMode roundingMode);

      #endregion
    }

    private class StandardDpiLayoutRoundImpl : ILayoutRoundImpl
    {
      #region Interface Implementations

      #region ILayoutRoundImpl

      public double RoundX(double value, RoundingMode roundingMode)
      {
        return DoubleUtils.Round(value, roundingMode);
      }

      public double RoundY(double value, RoundingMode roundingMode)
      {
        return DoubleUtils.Round(value, roundingMode);
      }

      #endregion

      #endregion
    }

    private class NonStandardDpiLayoutRoundImpl : ILayoutRoundImpl
    {
      #region Static Fields and Constants

      private static readonly double DpiUpScaleX = DpiUtils.DpiScaleX;
      private static readonly double DpiDownScaleX = 1 / DpiUpScaleX;

      private static readonly double DpiUpScaleY = DpiUtils.DpiScaleY;
      private static readonly double DpiDownScaleY = 1 / DpiUpScaleY;

      #endregion

      #region  Methods

      private static double DownScaleX(double value)
      {
        return value * DpiDownScaleX;
      }

      private static double DownScaleY(double value)
      {
        return value * DpiDownScaleY;
      }

      private static double FinalRound(double value)
      {
        return DoubleUtils.Round(value, 2, RoundingMode.MidPointToEven);
      }
			
      private static double UpScaleX(double value)
      {
        return value * DpiUpScaleX;
      }

      private static double UpScaleY(double value)
      {
        return value * DpiUpScaleY;
      }

      #endregion

      #region Interface Implementations

      #region ILayoutRoundImpl

      public double RoundX(double value, RoundingMode roundingMode)
      {
        return FinalRound(DownScaleX(DoubleUtils.Round(UpScaleX(value), roundingMode)));
      }

      public double RoundY(double value, RoundingMode roundingMode)
      {
        return FinalRound(DownScaleY(DoubleUtils.Round(UpScaleY(value), roundingMode)));
      }

      #endregion

      #endregion
    }

    #endregion
  }
}