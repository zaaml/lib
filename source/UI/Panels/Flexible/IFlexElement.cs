// <copyright file="IFlexElement.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.UI.Panels.Flexible
{
  public interface IFlexElement
  {
    #region Properties

    double CurrentLength { get; }

    double DesiredLength { get; }

    short ExpandPriority { get; }

    double MaxLength { get; }

    double MinLength { get; }

    FlexOverflowBehavior OverflowBehavior { get; }

    short ShrinkPriority { get; }

    FlexStretchDirection StretchDirection { get; }

    #endregion
  }
}