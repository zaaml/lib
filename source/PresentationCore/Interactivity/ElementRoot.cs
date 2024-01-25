// <copyright file="ElementRoot.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.PropertyCore.Extensions;
using Zaaml.PresentationCore.Theming;

namespace Zaaml.PresentationCore.Interactivity
{
  internal sealed class ElementRoot : InteractivityRoot, IRuntimeSetterFactory
  {
    #region Static Fields and Constants

    private static readonly DependencyProperty TemplatedParentProperty =
      DPM.RegisterAttached<FrameworkElement, ElementRoot>
        ("TemplatedParent", OnTemplatedParentChanged);

    private static readonly Binding TemplatedParentBinding = new Binding
    {
      BindsDirectlyToSource = true,
      RelativeSource = XamlConstants.TemplatedParent
    };

    #endregion

    #region Ctors

    public ElementRoot(InteractivityService service) : base(service)
    {
    }

    #endregion

    #region Properties

    public FrameworkElement TemplatedParent
    {
      get
      {
        EnsureTemplatedParent();
        return GetTemplatedParent(InteractivityTarget);
      }
    }

    public FrameworkElement XamlElementRoot => XamlRoot as FrameworkElement;

    #endregion

    #region  Methods

    public void EnsureSkinListener()
    {
      if (InteractivityService.SkinListener)
        return;

      EnsureTemplatedParent();

      TemplatedParent?.GetInteractivityService().AttachRoot(this);

      InteractivityService.SkinListener = true;
    }

    private void EnsureTemplatedParent()
    {
      var interactivityTarget = InteractivityTarget;

      if (interactivityTarget.ReadLocalValue(TemplatedParentProperty).IsDependencyPropertyUnsetValue() == false)
        return;

      interactivityTarget.SetBinding(TemplatedParentProperty, TemplatedParentBinding);

      if (RealVisualStateObserver != null) 
	      return;

      RealVisualStateObserver = XamlElementRoot?.GetInteractivityService();
    }

    protected override void EnsureVisualStateObserver()
    {
      if (InteractivityService.VisualStateListener)
        return;

      EnsureTemplatedParent();

      InteractivityService.VisualStateListener = true;

      UpdateVisualStateObserver();
    }

    private static FrameworkElement GetTemplatedParent(DependencyObject element)
    {
      return (FrameworkElement) element.GetValue(TemplatedParentProperty);
    }

    protected override void OnDescendantApiPropertyChanged(Stack<InteractivityObject> descendants, string propertyName)
    {
    }

    private static void OnTemplatedParentChanged(DependencyObject dependencyObject, FrameworkElement oldTemplatedParent, FrameworkElement newTemplatedParent)
    {
      var interactivityService = ((FrameworkElement) dependencyObject).GetInteractivityService();
      var interactivityRoot = interactivityService.ActualElementRoot;

      if (interactivityRoot.InteractivityService.VisualStateListener)
        interactivityRoot.UpdateVisualStateObserver();

      if (interactivityRoot.InteractivityService.SkinListener == false)
        return;

      oldTemplatedParent?.GetInteractivityService().DetachRoot(interactivityRoot);
      newTemplatedParent?.GetInteractivityService().AttachRoot(interactivityRoot);

      if (newTemplatedParent != null)
        interactivityRoot.UpdateSkin(Extension.GetActualSkin(newTemplatedParent));
    }

    public override void UpdateSkin(SkinBase skin)
    {
      var interactivityTarget = InteractivityTarget;

      UpdateSkin(Extension.GetSettersInternal(interactivityTarget, false), skin);
      UpdateSkin(Extension.GetTriggersInternal(interactivityTarget, false), skin);
    }

    public override void UpdateThemeResources()
    {
      var interactivityTarget = InteractivityTarget;

      UpdateThemeResources(Extension.GetSettersInternal(interactivityTarget, false));
      UpdateThemeResources(Extension.GetTriggersInternal(interactivityTarget, false));
    }


    private void UpdateVisualStateObserver()
    {
      RealVisualStateObserver = TemplatedParent?.GetInteractivityService();
    }

    #endregion

    #region Interface Implementations

    #region IRuntimeSetterFactory

    RuntimeSetter IRuntimeSetterFactory.CreateSetter()
    {
      return new LocalRuntimeSetter();
    }

    #endregion

    #endregion
  }
}