// <copyright file="ControlTemplateBuilder.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using Zaaml.PresentationCore.Utils;

namespace Zaaml.PresentationCore.TemplateCore
{
  internal class ControlTemplateBuilder<T> where T : FrameworkElement
  {
    #region Fields

    private readonly Lazy<ControlTemplate> _lazyTemplateInstance;

    #endregion

    #region Ctors

    public ControlTemplateBuilder()
    {
      _lazyTemplateInstance = new Lazy<ControlTemplate>(BuildTemplate);
    }

    #endregion

    #region Properties

    public ControlTemplate ControlTemplate => _lazyTemplateInstance.Value;

    #endregion

    #region  Methods

    private ControlTemplate BuildTemplate()
    {
      var type = typeof(T);

      var typeNamespace = type.Namespace;
      var typeAssembly = new AssemblyName(type.Assembly.FullName).Name;
      var typeNamespacePrefix = $"xmlns:t='clr-namespace:{typeNamespace};assembly={typeAssembly}'";

      var templateString = $"<ControlTemplate {GenericControlTemplate.XamlNamespaces} {typeNamespacePrefix}><t:{type.Name} x:Name='TemplateRoot'/></ControlTemplate>";
      return XamlUtils.Load<ControlTemplate>(templateString);
    }

    public T GetTemplateRoot(Control control)
    {
      return control.GetImplementationRoot<T>();
    }

    #endregion
  }
}