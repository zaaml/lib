// <copyright file="FixedTemplateControlBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows.Controls;

namespace Zaaml.UI.Controls.Core
{
  public class FixedTemplateControlBase : Control
  {
    #region Fields

    private ControlTemplate _templateInternal;
    private bool _isTemplateApplied;

    #endregion

    #region Properties

    protected ControlTemplate TemplateInternal
    {
      get => _templateInternal;
      set
      {
        if (ReferenceEquals(_templateInternal, value))
          return;

        _templateInternal = value;
        Template = value;
      }
    }

    #endregion

    #region  Methods

    public sealed override void OnApplyTemplate()
    {
      if (ReferenceEquals(Template, TemplateInternal) == false)
        throw new Exception("Template property can not be set on FixedTemplateControl.");

      if (_isTemplateApplied)
        UndoTemplateOverride();

      base.OnApplyTemplate();

      ApplyTemplateOverride();

      _isTemplateApplied = true;
    }

    protected virtual void UndoTemplateOverride()
    {
    }

    protected virtual void ApplyTemplateOverride()
    {
    }

    #endregion
  }
}