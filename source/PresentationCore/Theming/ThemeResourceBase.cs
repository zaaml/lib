// <copyright file="ThemeResourceBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.PresentationCore.Theming
{

  internal interface IThemeResource
  {
    string Key { get; set; }
    IThemeResource Parent { get; set; }
  }
}