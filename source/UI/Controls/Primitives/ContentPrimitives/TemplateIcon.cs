// <copyright file="TemplateIcon.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.UI.Controls.Primitives.ContentPrimitives
{
  //[ContentProperty(nameof(Template))]
  public partial class TemplateIcon : IconBase
  {
    #region Static Fields and Constants

    public static readonly DependencyProperty TemplateProperty = DPM.RegisterAttached<DataTemplate, TemplateIcon>
      ("Template", OnIconPropertyChanged);

    private static readonly List<DependencyProperty> Properties = new List<DependencyProperty>
    {
      TemplateProperty
    };

    #endregion

    #region Fields

    private ContentPresenter _contentPresenter;

    #endregion

    #region Ctors

    static TemplateIcon()
    {
      Factories[TemplateProperty] = () => new TemplateIcon();
    }

    #endregion

    #region Properties

    private DataTemplate ActualTemplate => GetActualValue<DataTemplate>(TemplateProperty);

    protected internal override FrameworkElement IconElement => _contentPresenter ?? (_contentPresenter = CreateContentPresenter());

    protected override IEnumerable<DependencyProperty> PropertiesCore => Properties;

    public DataTemplate Template
    {
      get => (DataTemplate) GetValue(TemplateProperty);
      set => SetValue(TemplateProperty, value);
    }

    #endregion

    #region  Methods

    private ContentPresenter CreateContentPresenter()
    {
      return new ContentPresenter {ContentTemplate = ActualTemplate};
    }

    protected override IconBase CreateInstanceCore() => new TemplateIcon();

    protected override void OnIconPropertyChanged(DependencyPropertyChangedEventArgs e)
    {
      if (_contentPresenter == null)
        return;

      var contentPresenterProperty = e.Property == TemplateProperty ? ContentPresenter.ContentTemplateProperty : null;
      if (contentPresenterProperty != null)
        _contentPresenter.SetValue(contentPresenterProperty, GetActualValue<object>(e.Property));
    }

    #endregion
  }
}