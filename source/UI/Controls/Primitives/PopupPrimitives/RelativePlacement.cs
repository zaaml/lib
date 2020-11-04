// <copyright file="RelativePlacement.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.Utils;

namespace Zaaml.UI.Controls.Primitives.PopupPrimitives
{
  public sealed class RelativePlacement : RelativePlacementBase
  {
    #region  Methods

    protected override Rect ArrangeOverride(Size desiredSize)
    {
      return RectUtils.CalcAlignBox(TargetScreenBox, desiredSize.Rect(), Popup.HorizontalAlignment, Popup.VerticalAlignment);
    }

    #endregion
  }
}