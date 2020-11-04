// <copyright file="ParentLayoutPass.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;

namespace Zaaml.UI.Panels
{
  internal struct ParentLayoutPass
  {
    public ParentLayoutPass(ParentLayoutPassKind passKind) : this()
    {
      PassKind = passKind;
    }

    public static ParentLayoutPass CreateMeasurePass(Size availableSize)
    {
      return new ParentLayoutPass(ParentLayoutPassKind.Measure)
      {
        AvailableSize = availableSize
      };
    }

    public static ParentLayoutPass CreateArrangePass(Size finalSize)
    {
      return new ParentLayoutPass(ParentLayoutPassKind.Arrange)
      {
        FinalSize = finalSize
      };
    }

    public int ChildIndex { get; set; }

    public int LayoutPassVersion { get; set; }

    public Size AvailableSize { get; private set; }

    public Size DesiredSize { get; set; }

    public Size FinalSize { get; private set; }

    public Size ActualSize { get; set; }

    public bool IsMeasurePassDirty { get; private set; }

    public bool IsArrangePassDirty { get; private set; }

    public ParentLayoutPassKind PassKind { get; }

    public void InvalidateMeasurePass()
    {
      IsMeasurePassDirty = true;
    }

    public void InvalidateArrangePass()
    {
      IsArrangePassDirty = true;
    }
  }
}