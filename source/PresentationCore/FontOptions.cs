// <copyright file="FontOptions.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using Zaaml.Core.Disposable;
using Zaaml.Core.Extensions;
using Zaaml.PresentationCore.Data;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore.Extensions;

namespace Zaaml.PresentationCore
{
	internal static class DefaultFont
  {
    #region Static Fields and Constants

    public static readonly FontStyle FontStyle = FontStyles.Normal;
    public static readonly FontWeight FontWeight = FontWeights.Normal;
    public static readonly double FontSize = 11.0;
#if SILVERLIGHT
    public static readonly FontFamily FontFamily = new FontFamily("Portable User Interface");
#else
    public static readonly FontFamily FontFamily = new FontFamily("Segoe UI");
#endif
    public static readonly Brush Foreground = Colors.Black.ToSolidColorBrush();
    public static readonly Brush Background = Colors.Transparent.ToSolidColorBrush();

    #endregion
  }

  public class FontOptions : INotifyPropertyChanged, IDisposable
  {
    #region Fields

    private Brush _background = DefaultFont.Background;
    private FontFamily _fontFamily = DefaultFont.FontFamily;
    private double _fontSize = DefaultFont.FontSize;
    private FontStyle _fontStyle = DefaultFont.FontStyle;
    private FontWeight _fontWeight = DefaultFont.FontWeight;
    private Brush _foreground = DefaultFont.Foreground;
    private IDisposable _observeDisposable;

    #endregion

    #region Properties

    public Brush Background
    {
      get => _background;
      set
      {
        _background = value;

        OnPropertyChanged(nameof(Background));
      }
    }

    public FontFamily FontFamily
    {
      get => _fontFamily;
      set
      {
        _fontFamily = value;

        OnPropertyChanged(nameof(FontFamily));
      }
    }

    public double FontSize
    {
      get => _fontSize;
      set
      {
        _fontSize = value;

        OnPropertyChanged(nameof(FontSize));
      }
    }

    public FontStyle FontStyle
    {
      get => _fontStyle;
      set
      {
				_fontStyle = value;

        OnPropertyChanged(nameof(FontStyle));
      }
    }

    public FontWeight FontWeight
    {
      get => _fontWeight;
      set
      {
        _fontWeight = value;

        OnPropertyChanged(nameof(FontWeight));
      }
    }

    public Brush Foreground
    {
      get => _foreground;
      set
      {
        _foreground = value;

        OnPropertyChanged(nameof(Foreground));
      }
    }

    #endregion

    #region  Methods

    public void Assign(TextBlock textBlock)
    {
      textBlock.Foreground = Foreground;
      textBlock.FontStyle = FontStyle;
      textBlock.FontWeight = FontWeight;
      textBlock.FontFamily = FontFamily;
      textBlock.FontSize = FontSize;
    }

    public void Assign(Control control)
    {
      control.Background = Background;
      control.Foreground = Foreground;
      control.FontStyle = FontStyle;
      control.FontWeight = FontWeight;
      control.FontFamily = FontFamily;
      control.FontSize = FontSize;
    }

    public IDisposable Attach(Control control)
    {
      return new DisposableList
      {
        control.SetDisposableBinding(Control.BackgroundProperty, new Binding("Background") {Source = this}),
        control.SetDisposableBinding(Control.ForegroundProperty, new Binding("Foreground") {Source = this}),
        control.SetDisposableBinding(Control.FontStyleProperty, new Binding("FontStyle") {Source = this}),
        control.SetDisposableBinding(Control.FontWeightProperty, new Binding("FontWeight") {Source = this}),
        control.SetDisposableBinding(Control.FontFamilyProperty, new Binding("FontFamily") {Source = this, TargetNullValue = DefaultFont.FontFamily}),
        control.SetDisposableBinding(Control.FontSizeProperty, new Binding("FontSize") {Source = this})
      };
    }

    public IDisposable Attach(TextBlock textBlock)
    {
      return new DisposableList
      {
        textBlock.SetDisposableBinding(TextBlock.ForegroundProperty, new Binding("Foreground") {Source = this}),
        textBlock.SetDisposableBinding(TextBlock.FontStyleProperty, new Binding("FontStyle") {Source = this}),
        textBlock.SetDisposableBinding(TextBlock.FontWeightProperty, new Binding("FontWeight") {Source = this}),
        textBlock.SetDisposableBinding(TextBlock.FontFamilyProperty, new Binding("FontFamily") {Source = this, TargetNullValue = DefaultFont.FontFamily}),
        textBlock.SetDisposableBinding(TextBlock.FontSizeProperty, new Binding("FontSize") {Source = this}),
      };
    }

    public FontOptions Clone()
    {
      var fontOptions = new FontOptions();
      fontOptions.CopyFrom(this);
      return fontOptions;
    }

    public void CopyFrom(FontOptions prototype)
    {
      Background = prototype.Background;
      Foreground = prototype.Foreground;
      FontStyle = prototype.FontStyle;
      FontWeight = prototype.FontWeight;
      FontFamily = prototype.FontFamily;
      FontSize = prototype.FontSize;
    }

    public void CopyFrom(Control control)
    {
      Background = control.Background;
      Foreground = control.Foreground;
      FontStyle = control.FontStyle;
      FontWeight = control.FontWeight;
      FontFamily = control.FontFamily;
      FontSize = control.FontSize;
    }

    public void CopyFrom(TextBlock textBlock)
    {
      Foreground = textBlock.Foreground;
      FontStyle = textBlock.FontStyle;
      FontWeight = textBlock.FontWeight;
      FontFamily = textBlock.FontFamily;
      FontSize = textBlock.FontSize;
    }

    public static FontOptions FromControl(Control control)
    {
      var fontOptions = new FontOptions();

      fontOptions.Observe(control);

      return fontOptions;
    }

    internal static FontOptions FromElement(FrameworkElement fre)
    {
	    if (fre is TextBlock textBlock)
        return FromTextBlock(textBlock);

	    if (fre is Control control)
        return FromControl(control);

      return null;
    }

    public static FontOptions FromTextBlock(TextBlock textBlock)
    {
      var fontOptions = new FontOptions();

      fontOptions.Observe(textBlock);

      return fontOptions;
    }

    public IDisposable Observe(Control control)
    {
      _observeDisposable = _observeDisposable.DisposeExchange();

      CopyFrom(control);

      return _observeDisposable = new DisposableList
      {
        control.OnPropertyChanged<Brush>(Control.BackgroundProperty, (o, n) => Background = n),
        control.OnPropertyChanged<Brush>(Control.ForegroundProperty, (o, n) => Foreground = n),
        control.OnPropertyChanged<FontStyle>(Control.FontStyleProperty, (o, n) => FontStyle = n),
        control.OnPropertyChanged<FontWeight>(Control.FontWeightProperty, (o, n) => FontWeight = n),
        control.OnPropertyChanged<FontFamily>(Control.FontFamilyProperty, (o, n) => FontFamily = n),
        control.OnPropertyChanged<double>(Control.FontSizeProperty, (o, n) => FontSize = n)
      };
    }

    public IDisposable Observe(TextBlock textBlock)
    {
      _observeDisposable = _observeDisposable.DisposeExchange();

      CopyFrom(textBlock);

      return _observeDisposable = new DisposableList
      {
        textBlock.OnPropertyChanged<Brush>(TextBlock.ForegroundProperty, (o, n) => Foreground = n),
        textBlock.OnPropertyChanged<FontStyle>(TextBlock.FontStyleProperty, (o, n) => FontStyle = n),
        textBlock.OnPropertyChanged<FontWeight>(TextBlock.FontWeightProperty, (o, n) => FontWeight = n),
        textBlock.OnPropertyChanged<FontFamily>(TextBlock.FontFamilyProperty, (o, n) => FontFamily = n),
        textBlock.OnPropertyChanged<double>(TextBlock.FontSizeProperty, (o, n) => FontSize = n)
      };
    }

    protected virtual void OnPropertyChanged(string propertyName)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    #endregion

    #region Interface Implementations

    #region INotifyPropertyChanged

    public event PropertyChangedEventHandler PropertyChanged;

    #endregion

    #endregion

    public void Dispose()
    {
	    _observeDisposable = _observeDisposable.DisposeExchange();
    }
  }

	internal readonly struct FontOptionsStruct
	{
		public FontOptionsStruct(FontFamily fontFamily, double fontSize, FontStyle fontStyle, FontWeight fontWeight)
		{
			FontFamily = fontFamily;
			FontSize = fontSize;
			FontStyle = fontStyle;
			FontWeight = fontWeight;
		}

		public FontOptionsStruct(FontOptions fontOptions) : this()
		{
			FontFamily = fontOptions.FontFamily;
			FontSize = fontOptions.FontSize;
			FontStyle = fontOptions.FontStyle;
			FontWeight = fontOptions.FontWeight;
		}

		public readonly FontFamily FontFamily;

		public readonly double FontSize;

		public readonly FontStyle FontStyle;

		public readonly FontWeight FontWeight;
	}
}