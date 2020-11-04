// <copyright file="VisualStateGroup.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Windows.Markup;

namespace Zaaml.PresentationCore.Interactivity.VSM
{
  [ContentProperty(nameof(States))]
  public sealed class VisualStateGroup
  {
    public VisualStateGroup()
    {
    }

    internal VisualStateGroup(string name, IEnumerable<string> states)
    {
      Name = name;
      foreach (var state in states)
        States.Add(new VisualState {Name = state});
    }

    #region Properties

    public string Name { get; set; }

    public VisualStateCollection States { get; } = new VisualStateCollection();

    #endregion
  }
}
