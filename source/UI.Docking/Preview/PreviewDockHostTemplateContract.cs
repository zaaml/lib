// <copyright file="PreviewDockHostTemplateContract.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows.Shapes;
using Zaaml.Core;
using Zaaml.PresentationCore.TemplateCore;

namespace Zaaml.UI.Controls.Docking
{
  public sealed class PreviewDockHostTemplateContract : DockControlTemplateContract
  {
    #region Static Fields and Constants

    public const string PreviewPathName = "PART_PreviewPath";

    #endregion

    #region Properties

    [TemplateContractPart(Required = true, Name = PreviewPathName)]
    public Path PreviewPath { get; [UsedImplicitly] private set; }

    #endregion
  }
}