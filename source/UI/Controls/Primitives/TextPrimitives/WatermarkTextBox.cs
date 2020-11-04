// <copyright file="WatermarkTextBox.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.Theming;
using Style = System.Windows.Style;
#if !SILVERLIGHT
using System.Windows.Input;
#endif

namespace Zaaml.UI.Controls.Primitives.TextPrimitives
{
  public class WatermarkTextBox : TextBoxBase
  {
    #region Static Fields and Constants

    private static readonly Style DefaultWatermarkTextStyle = new Style(typeof(TextBlock))
    {
      Setters =
      {
        new Setter(TextBlock.FontSizeProperty, 11.0),
        new Setter(TextBlock.FontFamilyProperty, new FontFamily("Portable User Interface")),
        new Setter(TextBlock.FontStyleProperty, FontStyles.Normal),
        new Setter(TextBlock.FontWeightProperty, FontWeights.Normal),
      }
    };

    public static readonly DependencyProperty WatermarkTextProperty = DPM.Register<string, WatermarkTextBox>
      ("WatermarkText");

    public static readonly DependencyProperty WatermarkIconProperty = DPM.Register<ImageSource, WatermarkTextBox>
      ("WatermarkIcon");

    public static readonly DependencyProperty ShowWatermarkProperty = DPM.Register<bool, WatermarkTextBox>
      ("ShowWatermark", true, s => s.OnShowWatermarkChanged);

    private static readonly DependencyPropertyKey ActualShowWatermarkPropertyKey = DPM.RegisterReadOnly<bool, WatermarkTextBox>
      ("ActualShowWatermark", true);

    public static readonly DependencyProperty WatermarkTextStyleProperty = DPM.Register<Style, WatermarkTextBox>
      ("WatermarkTextStyle", DefaultWatermarkTextStyle);

    public static readonly DependencyProperty ActualShowWatermarkProperty = ActualShowWatermarkPropertyKey.DependencyProperty;

    private bool _isInEditState;

    #endregion

    #region Ctors

    static WatermarkTextBox()
    {
      DefaultStyleKeyHelper.OverrideStyleKey<WatermarkTextBox>();
    }

    public WatermarkTextBox()
    {
      this.OverrideStyleKey<WatermarkTextBox>();
    }

    #endregion

    #region Properties

    public Style WatermarkTextStyle
    {
      get => (Style) GetValue(WatermarkTextStyleProperty);
      set => SetValue(WatermarkTextStyleProperty, value);
    }

    public bool ShowWatermark
    {
      get => (bool) GetValue(ShowWatermarkProperty);
      set => SetValue(ShowWatermarkProperty, value);
    }

    public ImageSource WatermarkIcon
    {
      get => (ImageSource) GetValue(WatermarkIconProperty);
      set => SetValue(WatermarkIconProperty, value);
    }

    public string WatermarkText
    {
      get => (string) GetValue(WatermarkTextProperty);
      set => SetValue(WatermarkTextProperty, value);
    }

    public bool ActualShowWatermark
    {
      get => (bool) GetValue(ActualShowWatermarkProperty);
      private set => this.SetReadOnlyValue(ActualShowWatermarkPropertyKey, value);
    }

    #endregion

    #region  Methods

    private void OnShowWatermarkChanged()
    {
      UpdateWatermark();
    }

    private void UpdateWatermark()
    {
      ActualShowWatermark = ShowWatermark && IsInEditState == false && string.IsNullOrEmpty(Text);
    }

#if SILVERLIGHT
    protected override void OnGotFocus(RoutedEventArgs e)
    {
      base.OnGotFocus(e);
      EnterEditState();
    }

    protected override void OnLostFocus(RoutedEventArgs e)
    {
      base.OnLostFocus(e);
      ExitEditState();
    }
#else
    protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
    {
      base.OnGotKeyboardFocus(e);

      EnterEditState();
    }

    protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
    {
      base.OnLostKeyboardFocus(e);

      ExitEditState();
    }

#endif

	  protected override void OnTextChanged(TextChangedEventArgs e)
	  {
		  base.OnTextChanged(e);

			UpdateWatermark();
	  }

	  private void EnterEditState()
    {
      IsInEditState = true;
    }

    private void ExitEditState()
    {
      IsInEditState = false;
    }

    private bool IsInEditState
    {
      get => _isInEditState;
      set
      {
        if (_isInEditState == value)
          return;

        _isInEditState = value;

        UpdateWatermark();
      }
    }

    #endregion
  }
}