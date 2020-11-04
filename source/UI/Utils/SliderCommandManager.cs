// <copyright file="SliderCommandManager.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Zaaml.PresentationCore.PropertyCore;
#if !SILVERLIGHT

#endif

namespace Zaaml.UI.Utils
{
  public enum SliderCommand
  {
    Undefined,
    DecreaseLarge,
    IncreaseLarge,
  }

  public static class SliderCommandManager
  {
#region Static Fields and Constants

    public static readonly DependencyProperty CommandProperty = DPM.RegisterAttached
      ("Command", typeof(SliderCommandManager), SliderCommand.Undefined, OnCommandChanged);

#endregion

#region  Methods

#if !SILVERLIGHT
    private static RoutedCommand GetCommand(SliderCommand scrollBarCommand)
    {
      switch (scrollBarCommand)
      {
        case SliderCommand.DecreaseLarge:
          return System.Windows.Controls.Slider.DecreaseLarge;
        case SliderCommand.IncreaseLarge:
          return System.Windows.Controls.Slider.IncreaseLarge;
        default:
          return null;
      }
    }
#endif

    public static SliderCommand GetCommand(DependencyObject element)
    {
      return (SliderCommand) element.GetValue(CommandProperty);
    }

    private static void OnCommandChanged(DependencyObject button, SliderCommand oldCommand, SliderCommand newCommand)
    {
#if !SILVERLIGHT
      var repeatButton = button as RepeatButton;
      if (repeatButton != null)
        repeatButton.Command = GetCommand(newCommand);
#endif
    }

    public static void SetCommand(DependencyObject element, SliderCommand value)
    {
      element.SetValue(CommandProperty, value);
    }

#endregion
  }
}