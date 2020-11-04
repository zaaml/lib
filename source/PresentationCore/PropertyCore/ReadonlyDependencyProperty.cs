// <copyright file="ReadonlyDependencyProperty.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

#if SILVERLIGHT

using System.Collections.Generic;

// ReSharper disable once CheckNamespace
namespace System.Windows
{
  public sealed class DependencyPropertyKey
  {
#region Static Fields and Constants

    internal static Dictionary<DependencyProperty, DependencyPropertyKey> DepProp2Key = new Dictionary<DependencyProperty, DependencyPropertyKey>();

#endregion

#region Ctors

    internal DependencyPropertyKey(DependencyProperty depProp)
    {
      IsLocked = true;
      DependencyProperty = depProp;
      DepProp2Key[depProp] = this;
    }

#endregion

#region Properties

    public DependencyProperty DependencyProperty { get; private set; }

    internal bool IsLocked { get; set; }

#endregion
  }
}

#endif