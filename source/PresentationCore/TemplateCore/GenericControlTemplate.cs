// <copyright file="GenericControlTemplate.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows.Controls;
using Zaaml.PresentationCore.Utils;

namespace Zaaml.PresentationCore.TemplateCore
{
  internal static class GenericControlTemplate
  {
    #region Static Fields and Constants

    internal static readonly string XamlNamespaces = $"xmlns='{XamlConstants.XamlNamespace}' xmlns:x='{XamlConstants.XamlXNamespace}'";

    private static readonly string BorderTemplateString =
      $"<ControlTemplate {XamlNamespaces}><Border /></ControlTemplate>";

    private static readonly string ContentPresenterTemplateString =
      $"<ControlTemplate {XamlNamespaces}><ContentPresenter /></ControlTemplate>";

    private static readonly string TextBlockTemplateString =
      $"<ControlTemplate {XamlNamespaces}><TextBlock /></ControlTemplate>";

    private static readonly string ItemsControlTemplateString =
      $"<ControlTemplate {XamlNamespaces} TargetType='ItemsControl'><ScrollViewer x:Name='ScrollViewer' BorderThickness='0' Padding='0' VerticalScrollBarVisibility='Auto'><ItemsPresenter/></ScrollViewer></ControlTemplate>";


    private static readonly Lazy<ControlTemplate> LazyBorderTemplateInstance;
    private static readonly Lazy<ControlTemplate> LazyContentPresenterTemplateInstance;
    private static readonly Lazy<ControlTemplate> LazyTextBlockTemplateInstance;
    private static readonly Lazy<ControlTemplate> LazyItemsControlTemplateInstance;

    #endregion

    #region Ctors

    static GenericControlTemplate()
    {
      LazyBorderTemplateInstance = new Lazy<ControlTemplate>(() => XamlUtils.Load<ControlTemplate>(BorderTemplateString));
      LazyContentPresenterTemplateInstance = new Lazy<ControlTemplate>(() => XamlUtils.Load<ControlTemplate>(ContentPresenterTemplateString));
      LazyTextBlockTemplateInstance = new Lazy<ControlTemplate>(() => XamlUtils.Load<ControlTemplate>(TextBlockTemplateString));
      LazyItemsControlTemplateInstance = new Lazy<ControlTemplate>(() => XamlUtils.Load<ControlTemplate>(ItemsControlTemplateString));
    }

    #endregion

    #region Properties

    public static ControlTemplate BorderTemplateInstance => LazyBorderTemplateInstance.Value;
    public static ControlTemplate ContentPresenterTemplateInstance => LazyContentPresenterTemplateInstance.Value;
    internal static ControlTemplate ItemsControlTemplateInstance => LazyItemsControlTemplateInstance.Value;
    public static ControlTemplate TextBlockTemplateInstance => LazyTextBlockTemplateInstance.Value;

    #endregion
  }
}