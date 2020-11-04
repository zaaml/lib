// <copyright file="FlexDefinitionExtension.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using Zaaml.PresentationCore.MarkupExtensions;

namespace Zaaml.UI.Panels.Flexible
{
  public sealed class FlexDefinitionExtension : MarkupExtensionBase
  {
    #region Fields

    private readonly FlexDefinition _flexDefinition = new FlexDefinition();

    #endregion

    #region Properties

    public short ExpandPriority
    {
      get => _flexDefinition.ExpandPriority;
      set => _flexDefinition.ExpandPriority = value;
    }

    public double MaxLength
    {
      get => _flexDefinition.MaxLength;
      set => _flexDefinition.MaxLength = value;
    }

    public double MinLength
    {
      get => _flexDefinition.MinLength;
      set => _flexDefinition.MinLength = value;
    }

    public FlexOverflowBehavior OverflowBehavior
    {
      get => _flexDefinition.OverflowBehavior;
      set => _flexDefinition.OverflowBehavior = value;
    }

    public short ShrinkPriority
    {
      get => _flexDefinition.ShrinkPriority;
      set => _flexDefinition.ShrinkPriority = value;
    }

    public FlexStretchDirection StretchDirection
    {
      get => _flexDefinition.StretchDirection;
      set => _flexDefinition.StretchDirection = value;
    }

    #endregion

    #region  Methods

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
      return _flexDefinition;
    }

    #endregion
  }
}