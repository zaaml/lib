//  <copyright file="TextBlockExtension.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//    Copyright (c) zaaml. All rights reserved.
//  </copyright>

using System;
using System.Windows;
using System.Windows.Controls;
using Zaaml.Core.Extensions;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.PresentationCore.Extensions
{
  public static class TextBlockExtensions
  {
    #region Static Fields

    private static readonly Type SType = typeof (TextBlockExtensions);

    public static readonly DependencyProperty FontOptionsProperty = DPM.RegisterAttached<FontOptions>
      ("FontOptions", SType, OnFontOptionsChanged);

    private static readonly DependencyProperty FontOptionsDisposerProperty = DPM.RegisterAttached<IDisposable>
      ("FontOptionsDisposer", SType);

    public static readonly DependencyProperty FontOptionsSourceProperty = DPM.RegisterAttached<FrameworkElement>
      ("FontOptionsSource", SType, OnFontOptionsSourceChanged);

    #endregion

    #region Methods

    public static FontOptions GetFontOptions(TextBlock element)
    {
      return (FontOptions) element.GetValue(FontOptionsProperty);
    }

    public static FrameworkElement GetFontOptionsSource(DependencyObject element)
    {
      return (FrameworkElement) element.GetValue(FontOptionsSourceProperty);
    }

    public static void SetFontOptions(TextBlock element, FontOptions value)
    {
      element.SetValue(FontOptionsProperty, value);
    }

    public static void SetFontOptionsSource(DependencyObject element, FrameworkElement value)
    {
      element.SetValue(FontOptionsSourceProperty, value);
    }

    private static void OnFontOptionsChanged(DependencyObject dependencyObject, FontOptions oldFontOptions, FontOptions newFontOptions)
    {
      var textBlock = (TextBlock) dependencyObject;
      textBlock.ClearValue<IDisposable>(FontOptionsDisposerProperty, i => i.DisposeExchange());
      if (newFontOptions != null)
        textBlock.SetValue<IDisposable>(FontOptionsDisposerProperty, newFontOptions.Attach(textBlock));
    }

    private static void OnFontOptionsSourceChanged(DependencyObject dependencyObject, FrameworkElement oldFrameworkElement, FrameworkElement newFrameworkElement)
    {
      var textBlock = (TextBlock)dependencyObject;
      if (newFrameworkElement != null)
        SetFontOptions(textBlock, FontOptions.FromElement(newFrameworkElement));
      else
        textBlock.ClearValue(FontOptionsProperty);
    }

    #endregion
  }
}