// <copyright file="BitmapIcon.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Zaaml.Core.Extensions;
using Zaaml.PresentationCore.MarkupExtensions;
using Zaaml.PresentationCore.PropertyCore;
#if !SILVERLIGHT
using System.Windows.Markup;
using System.Windows.Navigation;
#endif

namespace Zaaml.UI.Controls.Primitives.ContentPrimitives
{
  public sealed partial class BitmapIcon : IconBase
  {
    #region Static Fields and Constants

    public static readonly DependencyProperty SourceProperty = DPM.RegisterAttached<ImageSource, BitmapIcon>
      ("Source", OnIconPropertyChanged);

    public static readonly DependencyProperty StretchProperty = DPM.RegisterAttached<Stretch, BitmapIcon>
      ("Stretch", OnIconPropertyChanged);

    public static readonly DependencyProperty StretchDirectionProperty = DPM.RegisterAttached<StretchDirection, BitmapIcon>
      ("StretchDirection", OnIconPropertyChanged);

    private static readonly List<DependencyProperty> Properties = new List<DependencyProperty>
    {
      SourceProperty,
      StretchProperty,
      StretchDirectionProperty,
    };

    private static readonly Dictionary<DependencyProperty, DependencyProperty> PropertyDictionary = new Dictionary<DependencyProperty, DependencyProperty>
    {
      {SourceProperty, Image.SourceProperty},
      {StretchProperty, Image.StretchProperty},
#if !SILVERLIGHT
      {StretchDirectionProperty, Image.StretchDirectionProperty}
#endif
    };

    #endregion

    #region Fields

    private Image _image;

    #endregion

    #region Ctors

    static BitmapIcon()
    {
      Factories[SourceProperty] = () => new BitmapIcon();
    }

    #endregion

    #region Properties

    private ImageSource ActualSource => GetActualValue<ImageSource>(SourceProperty);

    private Stretch ActualStretch => GetActualValue<Stretch>(StretchProperty);

    private StretchDirection ActualStretchDirection => GetActualValue<StretchDirection>(StretchDirectionProperty);

#if !SILVERLIGHT
    internal Uri BaseUri
    {
      get => (Uri) GetValue(BaseUriHelper.BaseUriProperty);
      set => SetValue(BaseUriHelper.BaseUriProperty, value);
    }
#endif

    protected internal override FrameworkElement IconElement => _image ??= CreateImage();

    protected override IEnumerable<DependencyProperty> PropertiesCore => Properties;

    public ImageSource Source
    {
      get => (ImageSource) GetValue(SourceProperty);
      set => SetValue(SourceProperty, value);
    }

    public Stretch Stretch
    {
      get => (Stretch) GetValue(StretchProperty);
      set => SetValue(StretchProperty, value);
    }

    public StretchDirection StretchDirection
    {
      get => (StretchDirection) GetValue(StretchDirectionProperty);
      set => SetValue(StretchDirectionProperty, value);
    }

    #endregion

    #region  Methods

    private Image CreateImage()
    {
      var image = new Image
      {
        Source = ActualSource,
        Stretch = ActualStretch,
#if !SILVERLIGHT
        StretchDirection = ActualStretchDirection
#endif
      };

#if !SILVERLIGHT
      var uriContext = (IUriContext) image;

      uriContext.BaseUri = BaseUri;
#endif

      return image;
    }

    protected override IconBase CreateInstanceCore() => new BitmapIcon();

    protected override Size MeasureOverrideCore(Size availableSize)
    {
#if !SILVERLIGHT
      UpdateUri();
#endif

      return base.MeasureOverrideCore(availableSize);
    }

    protected override void OnIconPropertyChanged(DependencyPropertyChangedEventArgs e)
    {
      if (_image == null)
        return;

      var imageProperty = PropertyDictionary.GetValueOrDefault(e.Property);

      if (imageProperty != null)
        _image.SetValue(imageProperty, GetActualValue<object>(e.Property));
    }
		
#if !SILVERLIGHT
    private void UpdateUri()
    {
	    if (!(_image?.Source is IUriContext sourceUriContext))
        return;

      var baseUri = BaseUriHelper.GetBaseUri(this);

      if (_image.Source.IsFrozen || !(sourceUriContext.BaseUri == null) || !(baseUri != null))
        return;

      sourceUriContext.BaseUri = baseUri;
    }
#endif

    #endregion
  }

  public sealed class BitmapIconExtension : MarkupExtensionBase
  {
    #region Properties

    public ImageSource Source { get; set; }

    public Stretch Stretch { get; set; }

    public StretchDirection StretchDirection { get; set; }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
      return new BitmapIcon
      {
        Source = Source,
        Stretch = Stretch,
        StretchDirection = StretchDirection
      };
    }

    #endregion
  }
}