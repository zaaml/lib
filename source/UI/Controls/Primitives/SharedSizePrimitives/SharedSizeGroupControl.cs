// <copyright file="SharedSizeGroupControl.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Markup;
using Zaaml.Core.Extensions;
using Zaaml.Core.Utils;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.Primitives.SharedSizePrimitives
{
  [ContentProperty("Child")]
  public sealed class SharedSizeGroupControl : FixedTemplateControl<SharedSizeGroupPanel>
  {
    #region Static Fields and Constants

    public static readonly DependencyProperty ChildProperty = DPM.Register<FrameworkElement, SharedSizeGroupControl>
      ("Child", s => s.OnChildChanged);

    public static readonly DependencyProperty IsSharingEnabledProperty = DPM.Register<bool, SharedSizeGroupControl>
      ("IsSharingEnabled", true, s => s.OnIsSharingEnabledChanged);

    #endregion

    #region Fields

    internal Dictionary<string, SharedSizeEntry> SharedSizes;

    #endregion

    #region Properties

    public FrameworkElement Child
    {
      get => (FrameworkElement) GetValue(ChildProperty);
      set => SetValue(ChildProperty, value);
    }

    private bool IsInMeasure => TemplateRoot.IsInMeasure;

    public bool IsSharingEnabled
    {
      get => (bool) GetValue(IsSharingEnabledProperty);
      set => SetValue(IsSharingEnabledProperty, value);
    }

    protected override IEnumerator LogicalChildren => TemplateRoot == null || Child == null ? base.LogicalChildren : EnumeratorUtils.Concat(Child, base.LogicalChildren);

    internal SharedSizeGroupPanel SharedSizeGroupPanel => TemplateRoot;

    #endregion

    #region  Methods

    protected override void ApplyTemplateOverride()
    {
      base.ApplyTemplateOverride();

      var child = Child;
      if (child != null)
        RemoveLogicalChild(child);

      TemplateRoot.SharedSizeGroupControl = this;
    }

    private SharedSizeEntry CreateSharedSize(string key)
    {
      var sharedSize = new SharedSizeEntry(key, this);

      if (IsInMeasure)
        sharedSize.BeginMeasurePass(0);

      return sharedSize;
    }

    internal SharedSizeEntry GetSharedSize(string key)
    {
      if (key.IsNullOrEmpty())
        return null;

      if (SharedSizes == null)
        SharedSizes = new Dictionary<string, SharedSizeEntry>();

      return SharedSizes.GetValueOrCreate(key, CreateSharedSize);
    }

    private void OnChildChanged(FrameworkElement oldChild, FrameworkElement newChild)
    {
      if (TemplateRoot != null)
        TemplateRoot.OnChildChanged();
      else
      {
        if (oldChild != null)
          RemoveLogicalChild(oldChild);

        if (newChild != null)
          AddLogicalChild(newChild);
      }
    }

    private void OnIsSharingEnabledChanged()
    {
      TemplateRoot?.InvalidateInternal();
    }

    protected override void UndoTemplateOverride()
    {
      TemplateRoot.SharedSizeGroupControl = null;

      var child = Child;
      if (child != null)
        AddLogicalChild(child);

      base.UndoTemplateOverride();
    }

    #endregion
  }
}