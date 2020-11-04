// <copyright file="ImplementationRootLoadedService.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.Services;

namespace Zaaml.PresentationCore.TemplateCore
{
  internal static class ImplementationRootLoadedService
  {
    #region Static Fields and Constants

    private static readonly List<DependencyProperty> LoaderProperties = new List<DependencyProperty> {ImplementationRootLoadedServiceImpl.TemplateRootProperty};

    #endregion

    #region  Methods

    public static IDisposable InvokeOnImplementationRootLoaded(this FrameworkElement element, Action action)
    {
      return new ImplementationRootLoadedServiceWorker(element, action);
    }

    public static void PulseImplementationRoot(FrameworkElement implementationRoot)
    {
      var templatedParent = (Control) implementationRoot.GetTemplatedParent();
      if (templatedParent == null || ReferenceEquals(implementationRoot, templatedParent.GetImplementationRoot()) == false)
        return;

      foreach (var property in LoaderProperties)
      {
        templatedParent.SetValue(property, implementationRoot);
        templatedParent.ClearValue(property);
      }
    }

    public static void RegisterLoaderProperty(DependencyProperty property)
    {
      LoaderProperties.Add(property);
    }

    #endregion

    #region  Nested Types

    private class ImplementationRootLoadedServiceWorker : IDisposable
    {
      #region Fields

      private readonly Action _action;
      private readonly FrameworkElement _element;
      private readonly ImplementationRootLoadedServiceImpl _service;
      private bool _isDisposed;

      #endregion

      #region Ctors

      public ImplementationRootLoadedServiceWorker(FrameworkElement element, Action action)
      {
        _element = element;
        _action = action;

        _service = element.GetServiceOrCreate(() => new ImplementationRootLoadedServiceImpl());

        _service.ImplementationRootLoaded += OnLoaded;
        _element.LayoutUpdated += OnLoaded;
      }

      #endregion

      #region  Methods

      private void OnLoaded(object sender, EventArgs eventArgs)
      {
        Dispose();
        _action();
      }

      #endregion

      #region Interface Implementations

      #region IDisposable

      public void Dispose()
      {
        if (_isDisposed)
          return;

        _service.ImplementationRootLoaded -= OnLoaded;
        _element.LayoutUpdated -= OnLoaded;
        _element.RemoveService<ImplementationRootLoadedServiceImpl>();

        _isDisposed = true;
      }

      #endregion

      #endregion
    }

    private class ImplementationRootLoadedServiceImpl : ServiceBase<FrameworkElement>
    {
      #region Static Fields and Constants

      public static readonly DependencyProperty TemplateRootProperty = DPM.RegisterAttached<FrameworkElement, ImplementationRootLoadedServiceImpl>
        ("TemplateRootInt", OnImplementationRootChanged);

      #endregion

      #region Fields

      public event EventHandler ImplementationRootLoaded;

      #endregion

      #region  Methods

      protected override void OnAttach()
      {
        base.OnAttach();
        Target.SetBinding(TemplateRootProperty, new Binding
        {
          Path = new PropertyPath(TemplateRootProperty),
          RelativeSource = XamlConstants.TemplatedParent
        });
      }

      protected override void OnDetach()
      {
        Target.ClearValue(TemplateRootProperty);
        base.OnDetach();
      }

      private static void OnImplementationRootChanged(DependencyObject obj)
      {
        obj.GetService<ImplementationRootLoadedServiceImpl>()?.ImplementationRootLoaded?.Invoke(obj.GetService<ImplementationRootLoadedServiceImpl>(), EventArgs.Empty);
      }

      #endregion
    }

    #endregion
  }
}