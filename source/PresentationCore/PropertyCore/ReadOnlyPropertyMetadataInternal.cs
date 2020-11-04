// <copyright file="ReadOnlyPropertyMetadata.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;

#if SILVERLIGHT
using System;
#endif

namespace Zaaml.PresentationCore.PropertyCore
{
  internal class ReadOnlyPropertyMetadataInternal : PropertyMetadata
  {
    #region Ctors

    public ReadOnlyPropertyMetadataInternal(PropertyChangedCallback propertyChangedCallback)
      : this(null, propertyChangedCallback)
    {
    }

    public ReadOnlyPropertyMetadataInternal(object defaultValue)
      : this(defaultValue, null)
    {
    }

#if SILVERLIGHT
    public ReadOnlyPropertyMetadataInternal(object defaultValue, PropertyChangedCallback propertyChangedCallback)
      : base(defaultValue, OnPropertyChanged + propertyChangedCallback)
    {
    }
#else
    public ReadOnlyPropertyMetadataInternal(object defaultValue, PropertyChangedCallback propertyChangedCallback)
      : base(defaultValue, propertyChangedCallback)
    {
    }
#endif

    #endregion

    #region  Methods

#if SILVERLIGHT
    private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      DependencyPropertyKey accessKey;

      if (DependencyPropertyKey.DepProp2Key.TryGetValue(e.Property, out accessKey) && accessKey.IsLocked)
        throw new AccessViolationException("Readonly property could only be set using its access key.");
    }
#endif

    #endregion
  }
}