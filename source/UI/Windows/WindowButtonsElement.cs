// <copyright file="WindowButtonsElement.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Zaaml.Core;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.TemplateCore;

namespace Zaaml.UI.Windows
{
  [TemplateContractType(typeof(WindowButtonsElementTemplateContract))]
  public abstract class WindowButtonsElement : WindowElement
  {
    #region Static Fields and Constants

    public static readonly DependencyProperty ButtonStyleProperty = DPM.Register<Style, WindowButtonsElement>
      ("ButtonStyle");

    public static readonly DependencyProperty ButtonsPresenterTemplateProperty = DPM.Register<ControlTemplate, WindowButtonsElement>
      ("ButtonsPresenterTemplate");

    #endregion

    #region Ctors

    protected WindowButtonsElement()
    {
#if !SILVERLIGHT
      Focusable = false;
#endif
      IsTabStop = false;
    }

    #endregion

    #region Properties

    public ControlTemplate ButtonsPresenterTemplate
    {
      get => (ControlTemplate) GetValue(ButtonsPresenterTemplateProperty);
      set => SetValue(ButtonsPresenterTemplateProperty, value);
    }

    public Style ButtonStyle
    {
      get => (Style) GetValue(ButtonStyleProperty);
      set => SetValue(ButtonStyleProperty, value);
    }

    private WindowButtonsElementTemplateContract TemplateContract => (WindowButtonsElementTemplateContract) TemplateContractInternal;

    protected WindowButtonsPresenter ButtonsPresenter => TemplateContract.ButtonsPresenter;

    internal WindowButtonsPresenter ButtonsPresenterInternal => ButtonsPresenter;

    #endregion

    #region  Methods

    protected override void OnTemplateContractAttached()
    {
      base.OnTemplateContractAttached();

      if (ButtonsPresenter != null)
        ButtonsPresenter.Window = Window;
    }

    protected override void OnTemplateContractDetaching()
    {
      if (ButtonsPresenter != null)
        ButtonsPresenter.Window = null;

      base.OnTemplateContractDetaching();
    }

    internal override IEnumerable<IWindowElement> EnumerateWindowElements()
    {
      if (ButtonsPresenter != null)
        yield return ButtonsPresenter;
    }

    protected override void OnWindowAttached()
    {
      base.OnWindowAttached();

      if (ButtonsPresenter != null)
        ButtonsPresenter.Window = Window;
    }

    protected override void OnWindowDetaching()
    {
      if (ButtonsPresenter != null)
        ButtonsPresenter.Window = null;

      base.OnWindowDetaching();
    }

    #endregion
  }

  public class WindowButtonsElementTemplateContract : TemplateContract
  {
    #region Properties

    [TemplateContractPart(Required = false)]
    public WindowButtonsPresenter ButtonsPresenter { get; [UsedImplicitly] private set; }

    #endregion
  }
}