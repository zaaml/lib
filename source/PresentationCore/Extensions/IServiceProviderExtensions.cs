// <copyright file="IServiceProviderExtensions.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Reflection;
using System.Windows;
using System.Windows.Markup;
using System.Xaml;
using Zaaml.Core.Monads;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.PresentationCore.Extensions
{
  // ReSharper disable once InconsistentNaming
  internal static class IServiceProviderExtensions
  {
    #region  Methods

    public static object GetRootObject(this IServiceProvider serviceProvider)
    {
      return ((IRootObjectProvider) serviceProvider.GetService(typeof(IRootObjectProvider)))?.RootObject;
    }

    public static DependencyProperty GetTargetDependencyProperty(this IServiceProvider serviceProvider)
    {
      var property = serviceProvider.GetTargetProperty();
      return property.As<DependencyProperty>() ?? property.As<PropertyInfo>().Return(DependencyPropertyManager.GetDependencyProperty);
    }

    public static object GetTargetObject(this IServiceProvider serviceProvider)
    {
      var targetProvider = (IProvideValueTarget) serviceProvider.GetService(typeof(IProvideValueTarget));

      return targetProvider?.TargetObject;
    }

    public static object GetTargetProperty(this IServiceProvider serviceProvider)
    {
      var targetProvider = (IProvideValueTarget) serviceProvider.GetService(typeof(IProvideValueTarget));

      return targetProvider?.TargetProperty;
    }

    #endregion
  }
}