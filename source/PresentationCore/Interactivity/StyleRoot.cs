// <copyright file="StyleRoot.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using Zaaml.PresentationCore.Theming;

namespace Zaaml.PresentationCore.Interactivity
{
  internal sealed class StyleRoot : InteractivityRoot, IRuntimeSetterFactory
  {
    #region Ctors

    public StyleRoot(InteractivityService service) : base(service)
    {
    }

    #endregion

    #region  Methods

    protected override void EnsureVisualStateObserver()
    {
      RealVisualStateObserver = InteractivityTarget.GetInteractivityService();
    }

    protected override void OnDescendantApiPropertyChanged(Stack<InteractivityObject> descendants, string propertyName)
    {
    }

    public override void UpdateSkin(SkinBase newSkin)
    {
      var service = StyleService.GetRuntimeService(InteractivityTarget);

      if (service == null)
        return;

      UpdateSkin(service.Setters, newSkin);
      UpdateSkin(service.Triggers, newSkin);
    }

    public override void UpdateThemeResources()
    {
      var service = StyleService.GetRuntimeService(InteractivityTarget);

      if (service == null)
        return;

      UpdateThemeResources(service.Setters);
      UpdateThemeResources(service.Triggers);
    }

    #endregion

    #region Interface Implementations

    #region IRuntimeSetterFactory

    RuntimeSetter IRuntimeSetterFactory.CreateSetter()
    {
      return new StyleRuntimeSetter();
    }

    #endregion

    #endregion
  }
}