// <copyright file="IContextPopupControlInternal.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.PresentationCore;

namespace Zaaml.UI.Controls.Primitives.PopupPrimitives
{
  internal interface IContextPopupControlInternal : IContextPopupControl, ISharedItem
  {
    #region Properties

    FrameworkElement Owner { get; set; }

    DependencyObject Target { get; set; }

		bool OwnerAttachSelector { get; }

    #endregion
  }

  internal interface IContextPopupTarget
  {
	  void OnContextPopupControlOpened(IContextPopupControl popupControl);
  }
}
