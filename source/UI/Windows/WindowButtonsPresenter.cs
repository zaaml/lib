// <copyright file="WindowButtonsPresenter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Theming;
using Style = System.Windows.Style;

namespace Zaaml.UI.Windows
{
  [ContentProperty(nameof(Buttons))]
  [TemplateContractType(typeof(WindowButtonsPresenterTemplateContract))]
  public sealed class WindowButtonsPresenter : WindowElement, IWindowButtonCollectionOwner
  {
    #region Static Fields and Constants

    public static readonly DependencyProperty ButtonStyleProperty = DPM.Register<Style, WindowButtonsPresenter>
      ("ButtonStyle");

    public static readonly DependencyProperty ButtonsPresenterTemplateProperty = DPM.Register<ControlTemplate, WindowButtonsPresenter>
      ("ButtonsPresenterTemplate");

    private static readonly DependencyPropertyKey ButtonsPropertyKey = DPM.RegisterReadOnly<WindowButtonCollection, WindowButtonsPresenter>
      ("Buttons");

    public static readonly DependencyProperty ButtonsProperty = ButtonsPropertyKey.DependencyProperty;

    #endregion

    #region Ctors

    static WindowButtonsPresenter()
    {
      DefaultStyleKeyHelper.OverrideStyleKey<WindowButtonsPresenter>();
    }

    public WindowButtonsPresenter()
    {
      this.OverrideStyleKey<WindowButtonsPresenter>();
#if !SILVERLIGHT
      Focusable = false;
#endif
      IsTabStop = false;

      Buttons = new WindowButtonCollection(this);
    }

    #endregion

    #region Properties

    public WindowButtonCollection Buttons
    {
      get => (WindowButtonCollection) GetValue(ButtonsProperty);
      private set => this.SetReadOnlyValue(ButtonsPropertyKey, value);
    }

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

    #endregion

    #region  Methods

    protected override Size MeasureOverride(Size availableSize)
    {
      var templatedParent = this.GetTemplatedParent() as WindowElement;

      Window = templatedParent?.Window;

      return base.MeasureOverride(availableSize);
    }

    private void OnButtonAddedPrivate(WindowButton button)
    {
      button.AsWindowElement.Window = Window;
    }

    private void OnButtonRemovedPrivate(WindowButton button)
    {
      button.AsWindowElement.Window = null;
    }

    protected override void OnWindowAttached()
    {
      base.OnWindowAttached();

      foreach (var button in Buttons)
        button.AsWindowElement.Window = Window;
    }

    protected override void OnWindowDetaching()
    {
      foreach (var button in Buttons)
        button.AsWindowElement.Window = null;

      base.OnWindowDetaching();
    }

    void IWindowButtonCollectionOwner.OnButtonAdded(WindowButton button)
    {
      OnButtonAddedPrivate(button);
    }

    void IWindowButtonCollectionOwner.OnButtonRemoved(WindowButton button)
    {
      OnButtonRemovedPrivate(button);
    }

    #endregion
  }

  public class WindowButtonsPresenterTemplateContract : TemplateContract
  {
  }
}