// <copyright file="HostAnchor.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.Utils;

namespace Zaaml.PresentationCore
{
  internal class HostAnchor : FrameworkElement, ILayoutInfoProvider
  {
    #region Static Fields and Constants

    public static readonly HostAnchor TopLeft = new HostAnchor(RectPoint.TopLeft);
    public static readonly HostAnchor TopRight = new HostAnchor(RectPoint.TopRight);
    public static readonly HostAnchor BottomLeft = new HostAnchor(RectPoint.BottomLeft);
    public static readonly HostAnchor BottomRight = new HostAnchor(RectPoint.BottomRight);

    #endregion

    #region Fields

    private readonly RectPoint _position;

    #endregion

    #region Ctors

    private HostAnchor(RectPoint position)
    {
      _position = position;
    }

    #endregion

    #region Interface Implementations

    #region ILayoutInfoProvider

    public Rect HostRelativeBox => new Rect(Screen.VirtualScreenRect.GetPoint(_position), XamlConstants.ZeroSize);

    #endregion

    #endregion
  }
}