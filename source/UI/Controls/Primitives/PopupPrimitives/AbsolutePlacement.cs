// <copyright file="AbsolutePlacement.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.Utils;

namespace Zaaml.UI.Controls.Primitives.PopupPrimitives
{
  public sealed class AbsolutePlacement : AbsolutePlacementBase
  {
    #region  Methods

    protected override Rect ArrangeOverride(Size desiredSize)
    {
      return RectUtils.CalcAlignBox(ScreenBoundsCore, desiredSize.Rect(), Popup.HorizontalAlignment, Popup.VerticalAlignment);
    }

    #endregion
  }
}