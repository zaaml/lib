// <copyright file="InteractivityService.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using System.Windows.Data;
using Zaaml.Core;
using Zaaml.Core.Packed;
using Zaaml.Core.Weak.Collections;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.Services;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Theming;

namespace Zaaml.PresentationCore.Interactivity
{
  internal sealed partial class InteractivityService : ServiceBase<FrameworkElement>, IInteractivityService, IThemeChangeListener
  {
    #region Static Fields and Constants

    private static readonly DependencyProperty ImplementationRootProperty = DPM.RegisterAttached<FrameworkElement, InteractivityService>
      ("ImplementationRootInt", OnImplementationRootIntChanged);

    private static readonly Binding ImplementationRootLoaderBinding = new Binding {Path = new PropertyPath(ImplementationRootProperty), RelativeSource = XamlConstants.Self};

    #endregion

    #region Fields

    private readonly WeakLinkedList<InteractivityRoot> _interactivityRoots = new WeakLinkedList<InteractivityRoot>();
    private int _cleanupCount = GarbageCleanupCounter.CleanupCount;
    private WeakReference _implementationRoot;
    private byte _packedValue;

    #endregion

    #region Ctors

    static InteractivityService()
    {
      ImplementationRootLoadedService.RegisterLoaderProperty(ImplementationRootProperty);
    }

    #endregion

    #region Properties

    private int CleanupCount
    {
      set
      {
        if (_cleanupCount == value)
          return;

        _cleanupCount = value;
        OnCleanup();
      }
    }

    public ElementRoot ElementRoot { get; private set; }

    private FrameworkElement ImplementationRoot
    {
      get => _implementationRoot?.Target as FrameworkElement;
      set
      {
        var implementationRoot = ImplementationRoot;
        if (ReferenceEquals(implementationRoot, value))
          return;

        _implementationRoot = value != null ? new WeakReference(value) : null;

        OnImplementationRootChanged();
      }
    }

    private bool LayoutUpdated
    {
      get => PackedDefinition.LayoutUpdated.GetValue(_packedValue);
      set => PackedDefinition.LayoutUpdated.SetValue(ref _packedValue, value);
    }

    public bool SkinListener
    {
      get => PackedDefinition.SkinListener.GetValue(_packedValue);
      set => PackedDefinition.SkinListener.SetValue(ref _packedValue, value);
    }

    public StyleRoot StyleRoot { get; private set; }

    private bool ThemeListener
    {
      get => PackedDefinition.ThemeListener.GetValue(_packedValue);
      set => PackedDefinition.ThemeListener.SetValue(ref _packedValue, value);
    }

    public bool VisualStateListener
    {
      get => PackedDefinition.VisualStateListener.GetValue(_packedValue);
      set => PackedDefinition.VisualStateListener.SetValue(ref _packedValue, value);
    }

    #endregion

    #region  Methods

    public void AttachRoot(InteractivityRoot root)
    {
      if (ReferenceEquals(root, ElementRoot) || ReferenceEquals(root, StyleRoot))
        return;

      _interactivityRoots.Add(root);
    }

    private void CleanupInteractivityRoots()
    {
      _interactivityRoots.Cleanup();
    }

    public void DetachRoot(InteractivityRoot root)
    {
      if (ReferenceEquals(root, ElementRoot) || ReferenceEquals(root, StyleRoot))
        return;

      _interactivityRoots.Remove(root);
    }

    private void EnsureLayoutUpdatedHandler()
    {
      if (LayoutUpdated)
        return;

      Target.LayoutUpdated += OnLayoutUpdated;
      LayoutUpdated = true;
    }

    internal void EnsureThemeListener()
    {
      if (ThemeListener)
        return;

      ThemeListener = true;
      ThemeManager.AddThemeChangedListener(this);
    }

    public static InteractivityService GetInteractivityService(FrameworkElement element)
    {
      return element.GetServiceOrCreate(() => new InteractivityService());
    }

    protected override void OnAttach()
    {
      base.OnAttach();

      Target.SetBinding(ImplementationRootProperty, ImplementationRootLoaderBinding);
    }

    private void OnCleanup()
    {
      CleanupVisualStateListeners();
      CleanupInteractivityRoots();
    }

    private void OnImplementationRootChanged()
    {
      UpdateGroupListeners();
    }

    private static void OnImplementationRootIntChanged(DependencyObject target, FrameworkElement oldRoot, FrameworkElement newRoot)
    {
      GetInteractivityService((FrameworkElement) target).UpdateImplementationRoot();
    }

    private void OnLayoutUpdated(object sender, EventArgs eventArgs)
    {
      CleanupCount = GarbageCleanupCounter.CleanupCount;
      UpdateImplementationRoot();
    }

    public void OnSkinChanged(SkinBase oldSkin, SkinBase newSkin)
    {
      UpdateSkin(newSkin);
    }

    public void UpdateSkin(SkinBase skin)
    {
      foreach (var interactivityRoot in _interactivityRoots)
        interactivityRoot.UpdateSkin(skin);

      StyleRoot?.UpdateSkin(skin);
      ElementRoot?.UpdateSkin(skin);
    }

    private void UpdateImplementationRoot()
    {
      ImplementationRoot = Target.GetImplementationRoot();
    }

    #endregion

    #region Interface Implementations

    #region IInteractivityService

    public ElementRoot ActualElementRoot => ElementRoot ??= new ElementRoot(this);

    public StyleRoot ActualStyleRoot => StyleRoot ??= new StyleRoot(this);

    public bool UseTransitions
    {
      get => PackedDefinition.UseTransitions.GetValue(_packedValue);
      set => PackedDefinition.UseTransitions.SetValue(ref _packedValue, value);
    }

    #endregion

    #region IThemeChangeListener

    void IThemeChangeListener.OnThemeChanged(Theme oldTheme, Theme newTheme)
    {
      ElementRoot?.UpdateThemeResources();
      StyleRoot?.UpdateThemeResources();
    }

    #endregion

    #endregion

    #region  Nested Types

    private static class PackedDefinition
    {
      #region Static Fields and Constants

      public static readonly PackedBoolItemDefinition UseTransitions;
      public static readonly PackedBoolItemDefinition LayoutUpdated;
      public static readonly PackedBoolItemDefinition ThemeListener;
      public static readonly PackedBoolItemDefinition SkinListener;
      public static readonly PackedBoolItemDefinition VisualStateListener;

      #endregion

      #region Ctors

      static PackedDefinition()
      {
        var allocator = new PackedValueAllocator();

        UseTransitions = allocator.AllocateBoolItem();
        LayoutUpdated = allocator.AllocateBoolItem();
        ThemeListener = allocator.AllocateBoolItem();
        SkinListener = allocator.AllocateBoolItem();
        VisualStateListener = allocator.AllocateBoolItem();
      }

      #endregion
    }

    #endregion
  }

  internal static class InteractivityServiceExtensions
  {
    #region  Methods

    public static InteractivityService GetInteractivityService(this FrameworkElement element)
    {
      return InteractivityService.GetInteractivityService(element);
    }

    #endregion
  }
}