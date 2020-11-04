// <copyright file="ScrollInfoChangedEventArgs.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.UI.Controls.ScrollView
{
  public sealed class ScrollInfoChangedEventArgs : EventArgs
  {
    #region Ctors

    public ScrollInfoChangedEventArgs(ScrollInfo oldInfo, ScrollInfo newInfo)
    {
      OldInfo = oldInfo;
      NewInfo = newInfo;
    }

    #endregion

    #region Properties

    public ScrollInfo NewInfo { get; }

    public ScrollInfo OldInfo { get; }

    #endregion

    #region  Methods

    private bool Equals(ScrollInfoChangedEventArgs other)
    {
      return NewInfo.Equals(other.NewInfo) && OldInfo.Equals(other.OldInfo);
    }

    public override bool Equals(object obj)
    {
      if (ReferenceEquals(null, obj)) return false;
      if (ReferenceEquals(this, obj)) return true;

      var a = obj as ScrollInfoChangedEventArgs;
      return a != null && Equals(a);
    }

    public override int GetHashCode()
    {
      unchecked
      {
        return (NewInfo.GetHashCode() * 397) ^ OldInfo.GetHashCode();
      }
    }

    #endregion
  }
}