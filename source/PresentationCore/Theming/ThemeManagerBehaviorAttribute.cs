// <copyright file="ThemeManagerBehaviorAttribute.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.PresentationCore.Theming
{
  [AttributeUsage(AttributeTargets.Assembly)]
  public class ThemeManagerBehaviorAttribute : Attribute
  {
    #region  Methods

    protected virtual bool EnableThemeStyle(Type elementType, Type styleType)
    {
      return styleType.IsAssignableFrom(elementType);
    }

    internal bool EnableThemeStyleInt(Type elementType, Type styleType)
    {
      return EnableThemeStyle(elementType, styleType);
    }

    #endregion
  }
}
