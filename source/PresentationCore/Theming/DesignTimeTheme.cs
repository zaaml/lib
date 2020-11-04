// <copyright file="DesignTimeTheme.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.PresentationCore.Utils;

namespace Zaaml.PresentationCore.Theming
{
  //public class DesignTimeTheme : MarkupExtensionBase
  //{
  //  #region Properties

  //  public Theme Theme
  //  {
  //    get { return PresentationCoreUtils.IsInDesignMode ? ThemeManager.ApplicationTheme : null; }
  //    set
  //    {
  //      if (PresentationCoreUtils.IsInDesignMode)
  //        ThemeManager.ApplicationTheme = value;
  //    }
  //  }

  //  #endregion

  //  #region  Methods

  //  public override object ProvideValue(IServiceProvider serviceProvider)
  //  {
  //    if (PresentationCoreUtils.IsInDesignMode == false)
  //      return this;

  //    object target;
  //    object targetProperty;
  //    bool reflected;

  //    GetTarget(serviceProvider, out target, out targetProperty, out reflected);

  //    if (target is ResourceDictionary)
  //      return Theme?.CreateThemeResourceDictionary();

  //    return this;
  //  }

  //  #endregion
  //}

  public abstract class DesignTimeThemeResourceDictionary : ResourceDictionaryBase
  {
    #region Ctors

    protected DesignTimeThemeResourceDictionary(Theme theme)
    {
      if (PresentationCoreUtils.IsInDesignMode == false)
        return;

      ThemeManager.ApplicationTheme = theme;

#if SILVERLIGHT
      MergedDictionaries.Clear();
      MergedDictionaries.Add(ThemeManager.CreateThemeResourceDictionary());
#endif
    }
    #endregion
  }
}