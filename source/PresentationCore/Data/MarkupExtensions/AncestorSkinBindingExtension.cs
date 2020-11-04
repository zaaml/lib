// <copyright file="AncestorSkinBindingExtension.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows.Data;

namespace Zaaml.PresentationCore.Data.MarkupExtensions
{
  public sealed class AncestorSkinBindingExtension : SkinBindingBaseExtension
  {
    #region Properties

    public int AncestorLevel { get; set; } = 1;

    public Type AncestorType { get; set; }

    protected override RelativeSource Source => new RelativeSource(RelativeSourceMode.FindAncestor)
    {
      AncestorType = AncestorType,
      AncestorLevel = AncestorLevel
    };

    #endregion
  }
}