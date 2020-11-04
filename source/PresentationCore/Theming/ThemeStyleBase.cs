// <copyright file="ThemeStyleBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Linq;
using Zaaml.PresentationCore.Interactivity;
using NativeStyle = System.Windows.Style;

namespace Zaaml.PresentationCore.Theming
{
  public abstract class ThemeStyleBase : StyleBase
  {
    #region Fields

    private NativeStyle _nativeStyle;

    #endregion

    #region Properties

    protected override bool IsThemeBasedCore => true;

    internal NativeStyle NativeStyle
    {
      get
      {
        if (ReferenceEquals(StyleService.NativeStyle, _nativeStyle?.BasedOn) == false)
          _nativeStyle = null;

        return _nativeStyle ??= new NativeStyle {TargetType = TargetType, BasedOn = StyleService.NativeStyle};
      }
    }

    protected override IEnumerable<SetterBase> SettersCore => Enumerable.Empty<SetterBase>();

    protected override IEnumerable<TriggerBase> TriggersCore => Enumerable.Empty<TriggerBase>();

    #endregion
  }
}