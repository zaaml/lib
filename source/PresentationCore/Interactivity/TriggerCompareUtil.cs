// <copyright file="TriggerCompareUtil.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.PresentationCore.Converters;

namespace Zaaml.PresentationCore.Interactivity
{
  internal static class TriggerCompareUtil
  {
    #region  Methods

    public static bool Compare(object sourceValue, object targetValue, ITriggerValueComparer comparer)
    {
      var actualComparer = comparer ?? XamlValueComparer.TriggerComparer;

      var actualSourceValue = sourceValue;
      var actualTargetValue = XamlStaticConverter.ConvertValue(targetValue, sourceValue?.GetType() ?? typeof(object));

      return actualComparer.Compare(actualSourceValue, actualTargetValue);
    }

    public static TriggerState UpdateState(InteractivityObject interactivityObject, InteractivityProperty sourceProperty, ref object sourceStore, InteractivityProperty valueProperty, ref object valueStore, ITriggerValueComparer comparer)
    {
      var actualComparer = comparer ?? XamlValueComparer.TriggerComparer;

      var sourceValue = interactivityObject.GetValue(sourceProperty, ref sourceStore);
      var value = interactivityObject.CacheConvert(valueProperty, sourceValue?.GetType(), ref valueStore);

      return actualComparer.Compare(sourceValue, value) ? TriggerState.Opened : TriggerState.Closed;
    }

    #endregion
  }
}