// <copyright file="MergedStyle.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Markup;
using Zaaml.PresentationCore.Interactivity;

namespace Zaaml.PresentationCore.Theming
{
  [ContentProperty(nameof(Styles))]
  public sealed class MergedStyle : StyleBase
  {
    #region Fields

    private readonly Style _mergedStyle = new Style();

    private MergedStyleCollection _styles;

    #endregion

    #region Properties

    protected override StyleBase BasedOnCore
    {
      get => _mergedStyle;
      set { }
    }

    public bool BasedOnThemeStyle { get; set; }

    protected override bool IsThemeBasedCore => BasedOnThemeStyle;

    protected override IEnumerable<SetterBase> SettersCore => Enumerable.Empty<SetterBase>();

    public MergedStyleCollection Styles => _styles ??= new MergedStyleCollection();

    public override Type TargetType
    {
      get => base.TargetType;
      set
      {
        if (ReferenceEquals(TargetType, value))
          return;

        if (TargetType != null)
          throw new InvalidOperationException("TargetType is sealed and can not be changed");

        base.TargetType = value;

        ApplyTargetTypeInt();
      }
    }

    protected override IEnumerable<TriggerBase> TriggersCore => Enumerable.Empty<TriggerBase>();

    #endregion

    #region  Methods

    private void ApplyTargetTypeInt()
    {
      if (TargetType == null)
        return;

      foreach (var style in Styles.Where(s => s.TargetType == null))
        style.TargetType = TargetType;

      if (BasedOnThemeStyle && TargetType != null)
        _mergedStyle.BasedOn = ThemeManager.GetThemeStyle(TargetType);
    }

    protected override void Initialize()
    {
      _mergedStyle.Merge(Styles.SelectMany(s => EnumerateBaseStylesAndSelf(s).Reverse()).ToList());

      Styles.IsReadOnly = true;

      ApplyTargetTypeInt();
    }

    #endregion
  }
}