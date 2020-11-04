// <copyright file="ControlTemplateRoot.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Windows;
using Zaaml.PresentationCore.Interactivity;
using Zaaml.PresentationCore.Interactivity.VSM;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Theming;
using VisualStateManager = System.Windows.VisualStateManager;
using ZaamlVisualStateGroup = Zaaml.PresentationCore.Interactivity.VSM.VisualStateGroup;
using ZaamlVisualState = Zaaml.PresentationCore.Interactivity.VSM.VisualState;

namespace Zaaml.UI.Panels.Core
{
#if DEV
  public class ControlTemplateRoot : TemplateRoot, IVisualStateManagerAdvisor, IDynamicSkinElementHost
#else
  public class ControlTemplateRoot : TemplateRoot, IVisualStateManagerAdvisor
#endif
  {
    #region Static Fields and Constants

    public static readonly DependencyProperty VisualStateGroupsProperty = DPM.Register<VisualStateGroupCollection, ControlTemplateRoot>
      ("VisualStateGroups");

    #endregion

    #region Fields

#if DEV
    private DynamicSkinElement _dynamicSkinElement;
#endif

    #endregion

    #region Ctors

    public ControlTemplateRoot()
    {
      VisualStateManager.SetCustomVisualStateManager(this, new PresentationCore.Interactivity.VisualStateManager());
      VisualStateManager.GetVisualStateGroups(this)?.Clear();
    }

    #endregion

    #region Properties

    internal virtual bool IsDynamicSkinEnabled => true;

    public VisualStateGroupCollection VisualStateGroups
    {
      get => (VisualStateGroupCollection) GetValue(VisualStateGroupsProperty);
      set => SetValue(VisualStateGroupsProperty, value);
    }

    #endregion

    #region  Methods

#if DEV
    private void EnsureDynamicSkinElement()
    {
      if (IsDynamicSkinEnabled)
      {
        if (_dynamicSkinElement == null)
        {
          var templatedParent = this.GetTemplatedParent();

          if (templatedParent == null)
            return;

          var dynamicElementType = typeof(DynamicSkinElement<>).MakeGenericType(templatedParent.GetType());

          _dynamicSkinElement = (DynamicSkinElement) Activator.CreateInstance(dynamicElementType);

          Children.Add(_dynamicSkinElement);

          return;
        }

        if (Children.Contains(_dynamicSkinElement) == false)
          Children.Add((_dynamicSkinElement));
      }
      else if (_dynamicSkinElement != null)
        Children.Remove(_dynamicSkinElement);
    }
#endif

    protected override Size MeasureCoreOverride(Size availableSize)
    {
#if DEV
      //EnsureDynamicSkinElement();
#endif

      return base.MeasureCoreOverride(availableSize);
    }

    #endregion

    #region Interface Implementations

    #region IVisualStateManagerAdvisor

    IEnumerable<ZaamlVisualStateGroup> IVisualStateManagerAdvisor.VisualStateGroups => VisualStateGroups;

    #endregion

    #endregion
  }
}