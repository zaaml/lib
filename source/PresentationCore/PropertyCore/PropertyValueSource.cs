// <copyright file="PropertyValueSource.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.PresentationCore.PropertyCore
{
  internal enum PropertyValueSource
  {
    Default,
    Inherited,
    TemplatedParent,
    Local,
    LocalBinding
  }
}