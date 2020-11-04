// <copyright file="ThemeKey.WPF.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Reflection;
using System.Windows;

namespace Zaaml.PresentationCore.Theming
{
  public sealed class ThemeKey : ComponentResourceKey
  {
    #region Ctors

    public ThemeKey(Type elementType, Theme theme, ThemeStyle themeStyle)
    {
      ElementType = elementType;
      Theme = theme;
      ThemeStyle = themeStyle;

      BaseDefaultStyleKeyType = DefaultStyleKeyHelper.DefaultStyleKeyProperty.GetMetadata(elementType).DefaultValue as Type ?? ElementType;
    }

    #endregion

    #region Properties

    public override Assembly Assembly => Theme.GetThemeKeyAssembly(this);

    public Type BaseDefaultStyleKeyType { get; }

    public Type ElementType { get; }

    public Theme Theme { get; }

    public ThemeStyle ThemeStyle { get; }

    #endregion

    #region  Methods

    private bool Equals(ThemeKey other)
    {
      return base.Equals(other) && ElementType == other.ElementType && Equals(Theme, other.Theme);
    }

    public override bool Equals(object obj)
    {
      if (ReferenceEquals(null, obj)) return false;
      if (ReferenceEquals(this, obj)) return true;

      return obj is ThemeKey key && Equals(key);
    }

    internal static Type GetDefaultType(FrameworkElement fre)
    {
      var elementType = fre.GetType();

      if (DefaultStyleKeyHelper.IsItsOwnDefaultStyleKey(elementType))
        return elementType;

      var defKey = fre.GetValue(DefaultStyleKeyHelper.DefaultStyleKeyProperty);

      if (defKey is ThemeKey themeKey)
        return themeKey.ElementType;

      return defKey as Type ?? elementType;
    }

    public override int GetHashCode()
    {
      unchecked
      {
        var hashCode = base.GetHashCode();

        hashCode = (hashCode * 397) ^ (ElementType != null ? ElementType.GetHashCode() : 0);
        hashCode = (hashCode * 397) ^ (Theme != null ? Theme.GetHashCode() : 0);

        return hashCode;
      }
    }

    public override string ToString()
    {
      return $"{ElementType} - {Theme.Name}";
    }

    #endregion
  }
}