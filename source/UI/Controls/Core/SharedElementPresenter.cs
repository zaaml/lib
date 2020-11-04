// <copyright file="SharedElementPresenter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Markup;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.UI.Panels.Core;

namespace Zaaml.UI.Controls.Core
{
	[ContentProperty(nameof(Child))]
  public sealed class SharedElementPresenter : FixedTemplateControl<Panel>
  {
    #region Static Fields and Constants

    private static readonly DependencyPropertyKey ActualPresenterPropertyKey = DPM.RegisterAttachedReadOnly<SharedElementPresenter, SharedElementPresenter>
      ("ActualPresenter", OnActualPresenterPropertyChanged);

    public static readonly DependencyProperty ActualPresenterProperty = ActualPresenterPropertyKey.DependencyProperty;

    public static readonly DependencyProperty PresenterProperty = DPM.RegisterAttached<SharedElementPresenter, SharedElementPresenter>
      ("Presenter", OnPresenterPropertyChanged);

    public static readonly DependencyProperty ChildProperty = DPM.Register<FrameworkElement, SharedElementPresenter>
      ("Child", s => s.OnChildChanged);

    private static readonly DependencyProperty ChildOwnerProperty = DPM.RegisterAttached<SharedElementPresenter, SharedElementPresenter>
      ("ChildOwner", OnPresenterPropertyChanged);

    #endregion

    #region Fields

    private FrameworkElement _actualChild;

    #endregion

    #region Properties

    private FrameworkElement ActualChild
    {
      get => _actualChild;
      set
      {
        if (ReferenceEquals(_actualChild, value))
          return;

        if (_actualChild != null)
        {
          if (TemplateRoot != null)
            TemplateRoot.Children.Remove(_actualChild);
          else
            RemoveLogicalChild(_actualChild);
        }

        _actualChild = value;

        if (_actualChild != null)
        {
          if (TemplateRoot != null)
            TemplateRoot.Children.Add(_actualChild);
          else
            AddLogicalChild(_actualChild);
        }
      }
    }

    public FrameworkElement Child
    {
      get => (FrameworkElement) GetValue(ChildProperty);
      set => SetValue(ChildProperty, value);
    }

    #endregion

    #region  Methods

    protected override void ApplyTemplateOverride()
    {
      base.ApplyTemplateOverride();

      if (ActualChild != null)
      {
        RemoveLogicalChild(ActualChild);
        TemplateRoot.Children.Add(ActualChild);
      }
    }

    public static SharedElementPresenter GetActualPresenter(DependencyObject element)
    {
      return (SharedElementPresenter) element.GetValue(ActualPresenterProperty);
    }

    public static SharedElementPresenter GetPresenter(DependencyObject element)
    {
      return (SharedElementPresenter) element.GetValue(PresenterProperty);
    }

    private static void OnActualPresenterPropertyChanged(DependencyObject dependencyObject, SharedElementPresenter oldPresenter, SharedElementPresenter newPresenter)
    {
      var fre = (FrameworkElement) dependencyObject;

      if (oldPresenter != null)
        oldPresenter.ActualChild = null;

      if (newPresenter != null)
        newPresenter.ActualChild = fre;
    }

    private void OnChildChanged(FrameworkElement oldChild, FrameworkElement newChild)
    {
      oldChild?.ClearValue(ChildOwnerProperty);
      newChild?.SetValue(ChildOwnerProperty, this);
    }

    private static void OnPresenterPropertyChanged(DependencyObject dependencyObject, SharedElementPresenter oldPresenter, SharedElementPresenter newPresenter)
    {
      UpdateActualPresenter((FrameworkElement) dependencyObject);
    }

    private static void UpdateActualPresenter(FrameworkElement frameworkElement)
    {
      SetActualPresenter(frameworkElement, GetPresenter(frameworkElement) ?? (SharedElementPresenter)frameworkElement.GetValue(ChildOwnerProperty));
    }

    private static void SetActualPresenter(DependencyObject element, SharedElementPresenter value)
    {
      element.SetReadOnlyValue(ActualPresenterPropertyKey, value);
    }

    public static void SetPresenter(DependencyObject element, SharedElementPresenter value)
    {
      element.SetValue(PresenterProperty, value);
    }

    protected override void UndoTemplateOverride()
    {
      if (ActualChild != null)
      {
        TemplateRoot.Children.Remove(ActualChild);
        AddLogicalChild(ActualChild);
      }

      base.UndoTemplateOverride();
    }

    #endregion
  }
}