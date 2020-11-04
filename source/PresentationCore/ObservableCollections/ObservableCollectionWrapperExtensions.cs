// <copyright file="ObservableCollectionWrapperExtensions.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.ObjectModel;
using System.Windows;

namespace Zaaml.PresentationCore.ObservableCollections
{
  internal static class ObservableCollectionWrapperExtensions
  {
    #region  Methods

    public static ObservableCollectionWrapper<T> ToWrapper<T>(this ObservableCollection<T> collection)
    {
      return new ObservableCollectionWrapper<T>(collection);
    }

#if SILVERLIGHT
    public static ObservableCollectionWrapper<T> ToWrapper<T>(this DependencyObjectCollection<T> collection) where T : DependencyObject
    {
      return new ObservableCollectionWrapper<T>(collection, collection);
    }
#endif

    public static ObservableCollectionWrapper<T> ToWrapper<T>(this DependencyObjectCollectionBase<T> collection) where T : DependencyObject
    {
      return new ObservableCollectionWrapper<T>(collection, collection);
    }

    #endregion
  }
}