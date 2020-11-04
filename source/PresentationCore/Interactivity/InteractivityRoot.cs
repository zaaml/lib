// <copyright file="InteractivityRoot.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Windows;
using Zaaml.PresentationCore.Theming;

namespace Zaaml.PresentationCore.Interactivity
{
  internal abstract partial class InteractivityRoot : IInteractivityRoot
  {
    #region Fields

    private WeakReference _xamlRootWeak;

    #endregion

    #region Ctors

    internal InteractivityRoot(InteractivityService interactivityService)
    {
      InteractivityService = interactivityService;
    }

    #endregion

    #region  Methods

    public virtual DependencyObject FindName(string name)
    {
      return null;
    }

    protected abstract void OnDescendantApiPropertyChanged(Stack<InteractivityObject> descendants, string propertyName);

    protected virtual void OnXamlRootChanged(object oldXamlRoot, object newXamlRoot)
    {
    }

    public void UpdateSkin(InteractivityCollection collection, SkinBase skin)
    {
      collection?.WalkTree(skin ?? UpdateNullSkinVisitor.Instance);
    }

    public virtual void UpdateSkin(SkinBase newSkin)
    {
    }

    public void UpdateThemeResources(InteractivityCollection collection)
    {
      collection?.WalkTree(UpdateThemeResourcesVisitor.Instance);
    }

    public virtual void UpdateThemeResources()
    {
    }

    #endregion

    #region Interface Implementations

    #region IInteractivityObject

    IInteractivityObject IInteractivityObject.Parent => null;

    void IInteractivityObject.OnDescendantApiPropertyChanged(Stack<InteractivityObject> descendants, string propertyName)
    {
      OnDescendantApiPropertyChanged(descendants, propertyName);
    }

    #endregion

    #region IInteractivityRoot

    public InteractivityService InteractivityService { get; }

    public FrameworkElement InteractivityTarget => InteractivityService.Target;

    #endregion

    #region IServiceProvider

    public virtual object GetService(Type serviceType)
    {
      if (serviceType == typeof(IVisualStateObserver))
      {
        EnsureVisualStateObserver();

        return this;
      }

      return null;
    }

    #endregion

    #region IXamlRootOwner

    public object XamlRoot
    {
      get => _xamlRootWeak?.Target;
      set
      {
        var root = XamlRoot;
        if (ReferenceEquals(root, value))
          return;

        _xamlRootWeak = value != null ? new WeakReference(value) : null;

        OnXamlRootChanged(root, value);
      }
    }

    public object ActualXamlRoot => XamlRoot;

    #endregion

    #endregion

    #region  Nested Types

    private class UpdateThemeResourcesVisitor : IInteractivityVisitor
    {
      #region Static Fields and Constants

      public static readonly IInteractivityVisitor Instance = new UpdateThemeResourcesVisitor();

      #endregion

      #region Ctors

      private UpdateThemeResourcesVisitor()
      {
      }

      #endregion

      #region Interface Implementations

      #region IInteractivityVisitor

      public void Visit(InteractivityObject interactivityObject)
      {
        var setter = interactivityObject as Setter;

        setter?.UpdateThemeResources();
      }

      #endregion

      #endregion
    }

    private class UpdateNullSkinVisitor : IInteractivityVisitor
    {
      #region Static Fields and Constants

      public static readonly IInteractivityVisitor Instance = new UpdateNullSkinVisitor();

      #endregion

      #region Ctors

      private UpdateNullSkinVisitor()
      {
      }

      #endregion

      #region Interface Implementations

      #region IInteractivityVisitor

      public void Visit(InteractivityObject interactivityObject)
      {
        var setter = interactivityObject as Setter;

        setter?.UpdateSkin(null);
      }

      #endregion

      #endregion
    }

    #endregion
  }
}