// <copyright file="ReadOnlyUIElementCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Zaaml.UI.Panels.Core
{
  internal sealed class ReadOnlyUIElementCollection : IReadOnlyList<UIElement>
  {
    #region Fields

    private readonly Panel _panel;

    #endregion

    #region Ctors

    public ReadOnlyUIElementCollection(Panel panel)
    {
      _panel = panel;
    }

    #endregion

    #region Properties

    private UIElementCollection UICollection => _panel.Children;

    #endregion

    #region Interface Implementations

    #region IEnumerable

    IEnumerator IEnumerable.GetEnumerator()
    {
      return UICollection.GetEnumerator();
    }

    #endregion

    #region IEnumerable<UIElement>

    public IEnumerator<UIElement> GetEnumerator()
    {
      return UICollection.OfType<UIElement>().GetEnumerator();
    }

    #endregion

    #region IReadOnlyCollection<UIElement>

    public int Count => UICollection.Count;

    #endregion

    #region IReadOnlyList<UIElement>

    public UIElement this[int index] => UICollection[index];

    #endregion

    #endregion
  }
}