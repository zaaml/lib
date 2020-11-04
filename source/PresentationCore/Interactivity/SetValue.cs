// <copyright file="SetValue.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.PresentationCore.PropertyCore.Extensions;

namespace Zaaml.PresentationCore.Interactivity
{
  public sealed class SetValue : PropertyValueActionBase
  {
    #region  Methods

    protected override InteractivityObject CreateInstance()
    {
      return new SetValue();
    }

    protected override void InvokeOverride()
    {
      var actualTarget = ActualTarget;
      var actualProperty = ActualProperty;

      var actualValue = ActualValue;
      if (actualValue.IsDependencyPropertyUnsetValue())
        actualTarget.ClearValue(actualProperty);
      else
        actualTarget.SetValue(actualProperty, actualValue);
    }

    #endregion
  }
}