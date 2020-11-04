// <copyright file="ISelector.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;

namespace Zaaml.UI.Controls.Core
{
  internal interface ISelector<T> where T : FrameworkElement, ISelectable
  {
    #region Properties

    DependencyProperty SelectedIndexProperty { get; }

    DependencyProperty SelectedItemProperty { get; }

    DependencyProperty SelectedItemSourceProperty { get; }

    DependencyProperty SelectedValueProperty { get; }

    #endregion

    #region  Methods

    object GetValue(T item, object itemSource);

    void OnSelectedIndexChanged(int oldIndex, int newIndex);

    void OnSelectedItemChanged(T oldItem, T newItem);

    void OnSelectedItemSourceChanged(object oldItemSource, object newItemSource);

    void OnSelectedValueChanged(object oldValue, object newValue);

    void OnSelectionChanged(Selection<T> oldSelection, Selection<T> newSelection);

    #endregion
  }
}