// <copyright file="ThemeKeyValuePairExtensions.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace Zaaml.PresentationCore.Theming
{
  internal static class ThemeKeyValuePairExtensions
  {
    #region  Methods

    public static KeyValuePair<string, object> WithParentKey(this KeyValuePair<string, object> keyValuePair, string key)
    {
      return new KeyValuePair<string, object>(key + "." + keyValuePair.Key, keyValuePair.Value);
    }

    public static KeyValuePair<string, object> WithValue(this KeyValuePair<string, object> keyValuePair, object value)
    {
      return new KeyValuePair<string, object>(keyValuePair.Key, value);
    }

    #endregion
  }
}