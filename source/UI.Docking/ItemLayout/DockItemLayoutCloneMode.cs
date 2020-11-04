// <copyright file="DockItemLayoutCloneMode.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.UI.Controls.Docking
{
  [Flags]
  internal enum DockItemLayoutCloneMode
  {
    Instance = 0,
    BaseProperties = 1,
    LayoutProperties = 2,
    Structure = 4,

    Full = BaseProperties | LayoutProperties | Structure
  }
}