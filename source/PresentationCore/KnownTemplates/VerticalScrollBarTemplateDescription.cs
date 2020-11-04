// <copyright file="VerticalScrollBarTemplateDescription.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Controls.Primitives;
using Zaaml.Core;
using Zaaml.PresentationCore.TemplateCore;

namespace Zaaml.PresentationCore.KnownTemplates
{
  public sealed class VerticalScrollBarTemplateContract : TemplateContract
  {
    #region Ctors

    public VerticalScrollBarTemplateContract(ScrollBar frameworkElement)
      : base(frameworkElement)
    {
    }

    #endregion

    #region Properties

    [TemplateCore.TemplateContractPart]
    public RepeatButton VerticalLargeDecrease { get; [UsedImplicitly] private set; }

    [TemplateCore.TemplateContractPart]
    public RepeatButton VerticalLargeIncrease { get; [UsedImplicitly] private set; }

    [TemplateCore.TemplateContractPart]
    public FrameworkElement VerticalRoot { get; [UsedImplicitly] private set; }

    [TemplateCore.TemplateContractPart]
    public RepeatButton VerticalSmallDecrease { get; [UsedImplicitly] private set; }

    [TemplateCore.TemplateContractPart]
    public RepeatButton VerticalSmallIncrease { get; [UsedImplicitly] private set; }

    [TemplateCore.TemplateContractPart]
    public Thumb VerticalThumb { get; [UsedImplicitly] private set; }

    #endregion
  }
}