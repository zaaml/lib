// <copyright file="HorizontalScrollBarTemplateDescription.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Controls.Primitives;
using Zaaml.Core;
using Zaaml.PresentationCore.TemplateCore;

namespace Zaaml.PresentationCore.KnownTemplates
{
  public sealed class HorizontalScrollBarTemplateContract : TemplateContract
  {
    #region Ctors

    public HorizontalScrollBarTemplateContract(ScrollBar frameworkElement)
      : base(frameworkElement)
    {
    }

    #endregion

    #region Properties

    [TemplateCore.TemplateContractPart]
    public RepeatButton HorizontalLargeDecrease { get; [UsedImplicitly] private set; }

    [TemplateCore.TemplateContractPart]
    public RepeatButton HorizontalLargeIncrease { get; [UsedImplicitly] private set; }

    [TemplateCore.TemplateContractPart]
    public FrameworkElement HorizontalRoot { get; [UsedImplicitly] private set; }

    [TemplateCore.TemplateContractPart]
    public RepeatButton HorizontalSmallDecrease { get; [UsedImplicitly] private set; }

    [TemplateCore.TemplateContractPart]
    public RepeatButton HorizontalSmallIncrease { get; [UsedImplicitly] private set; }

    [TemplateCore.TemplateContractPart]
    public Thumb HorizontalThumb { get; [UsedImplicitly] private set; }

    #endregion
  }
}