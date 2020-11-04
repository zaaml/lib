// <copyright file="FixedTemplateControl.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Controls;
using Zaaml.PresentationCore.Data;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.TemplateCore;
using NativeContentPresenter = System.Windows.Controls.ContentPresenter;

namespace Zaaml.UI.Controls.Core
{
  public class FixedTemplateControl : FixedTemplateControlBase
  {
    #region Fields

    private UIElement _childInternal;

    protected NativeContentPresenter ContentPresenter;

    #endregion

    #region Ctors

    public FixedTemplateControl() : this(false)
    {
    }

    internal FixedTemplateControl(bool raw)
    {
      TemplateInt = raw ? GenericControlTemplate.ContentPresenterTemplateInstance : GenericControlTemplate.BorderTemplateInstance;
    }

    #endregion

    #region Properties

    protected UIElement ChildInternal
    {
      get => _childInternal;
      set
      {
        if (ReferenceEquals(_childInternal, value))
          return;

        _childInternal = value;

        if (ContentPresenter != null)
          ContentPresenter.Content = value;
      }
    }

    #endregion

    #region  Methods

    protected override void ApplyTemplateOverride()
    {
      base.ApplyTemplateOverride();

      var templateRoot = this.GetImplementationRoot();

      if (templateRoot is Border border)
      {
        border.BindStyleProperties(this, BorderStyleProperties.All);
        ContentPresenter = new NativeContentPresenter();
        border.Child = ContentPresenter;
      }
      else
      {
        ContentPresenter = (NativeContentPresenter) templateRoot;
        ContentPresenter.BindProperties(MarginProperty, this, PaddingProperty);
      }

      ContentPresenter.Content = _childInternal;

      BindPropertiesCore();
    }

    protected virtual void BindPropertiesCore()
    {
    }

    protected virtual void UnbindPropertiesCore()
    {
    }
		
    protected override void UndoTemplateOverride()
    {
      base.UndoTemplateOverride();

      UnbindPropertiesCore();
    }

    #endregion
  }

  public class FixedTemplateControl<T> : FixedTemplateControlBase where T : FrameworkElement
  {
    #region Static Fields and Constants

    private static readonly ControlTemplateBuilder<T> TemplateBuilder = new ControlTemplateBuilder<T>();

    #endregion

    #region Ctors

    public FixedTemplateControl()
    {
      TemplateInt = TemplateBuilder.ControlTemplate;
    }

    #endregion

    #region Properties

    protected T TemplateRoot { get; private set; }

    #endregion

    #region  Methods

    protected override void ApplyTemplateOverride()
    {
      TemplateRoot = TemplateBuilder.GetTemplateRoot(this);
    }

    protected override void UndoTemplateOverride()
    {
      TemplateRoot = null;
    }

    #endregion
  }
}