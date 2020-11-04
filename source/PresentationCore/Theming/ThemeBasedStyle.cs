// <copyright file="ThemeBasedStyle.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Markup;
using Zaaml.PresentationCore.Interactivity;
using SetterBase = Zaaml.PresentationCore.Interactivity.SetterBase;
using TriggerBase = Zaaml.PresentationCore.Interactivity.TriggerBase;

namespace Zaaml.PresentationCore.Theming
{
  [ContentProperty("Setters")]
  public sealed class ThemeBasedStyle : StyleBase
  {
    #region Fields

    private StyleResourceDictionary _resources;
    private StyleSetterCollection _setters;
    private StyleTriggerCollection _triggers;

    #endregion

    #region Properties

    public ResourceDictionary Resources => _resources ?? (_resources = new StyleResourceDictionary(this));

    public SetterCollectionBase Setters => _setters ?? (_setters = new StyleSetterCollection(this));

    protected override IEnumerable<SetterBase> SettersCore => _setters ?? Enumerable.Empty<SetterBase>();

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

        Initialize();
      }
    }

    public TriggerCollectionBase Triggers => _triggers ?? (_triggers = new StyleTriggerCollection(this));

    protected override IEnumerable<TriggerBase> TriggersCore => _triggers ?? Enumerable.Empty<TriggerBase>();

    #endregion

    #region  Methods

    protected override void Initialize()
    {
      if (BasedOnCore == null && TargetType != null)
        BasedOnCore = ThemeManager.GetThemeStyle(TargetType);
    }

    #endregion
  }
}