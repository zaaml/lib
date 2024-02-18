// <copyright file="Unset.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

amespace Zaaml.Core
{
  internal static class Unset
  {
    #region Static Fields and Constants

    public static readonly object Value = new object();

    #endregion

    #region  Methods

    public static T GetSetValueOrDefault<T>(this object value, T defaultValue = default(T))
    {
      return IsUnset(value) ? defaultValue : (T)value;
    }

    public static object GetSetValueOrDefault(this object value, object defaultValue = null)
    {
      return IsUnset(value) ? defaultValue : value;
    }

    public static bool IsSet(this object value)
    {
      return ReferenceEquals(value, Value) == false;
    }

    public static bool IsUnset(this object value)
    {
      return ReferenceEquals(value, Value);
    }

    #endregion
  }
}