// <copyright file="PanelUIElementPool.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Zaaml.Core.Extensions;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.UI.Utils
{
  internal class PanelUIElementPool<T> where T : UIElement
  {
    #region Fields

    private readonly Func<T> _builder;
    private readonly List<T> _busyElements = new List<T>();
    private readonly Action<T> _cleaner;

    private readonly List<T> _createdElements = new List<T>();
    private readonly Panel _panel;
    private readonly List<T> _releasedElements = new List<T>();

    #endregion

    #region Ctors

    public PanelUIElementPool(Panel panel, Func<T> builder, Action<T> cleaner)
    {
      _panel = panel;
      _builder = builder;
      _cleaner = cleaner;
    }

    #endregion

    #region  Methods

    public IEnumerable<T> AllElements()
    {
      return _createdElements.AsEnumerable();
    }

    public IEnumerable<T> BusyElements()
    {
      return _busyElements.AsEnumerable();
    }

    private T CreateElement()
    {
      var newElement = _builder();

      newElement.Opacity = 0.0;
      _panel.Children.Add(newElement);
      _releasedElements.Add(newElement);
      _createdElements.Add(newElement);

      return newElement;
    }

    public IEnumerable<T> FreeElements()
    {
      return _releasedElements.AsEnumerable();
    }

    public T GetElement()
    {
      var element = _releasedElements.FirstOrDefault() ?? CreateElement();
      _releasedElements.Remove(element);
      _busyElements.Add(element);
      element.Opacity = 1.0;

      return element;
    }

    public void ReleaseElement(T element)
    {
      //element.Opacity = 0.0;
      _busyElements.Remove(element);
      _releasedElements.Add(element);
    }

    public void Reset()
    {
      foreach (var createdElement in _createdElements)
      {
        _cleaner(createdElement);
        _panel.Children.Remove(createdElement);
      }

      _busyElements.Clear();
      _releasedElements.Clear();
      _createdElements.Clear();
    }

    #endregion
  }

  internal static class CompositePanelUIElementPool
  {
    #region Static Fields and Constants

    public static readonly DependencyProperty PoolIDProperty = DependencyPropertyManager.RegisterAttached
    ("PoolID", typeof(int), typeof(CompositePanelUIElementPool),
      new PropertyMetadata(default(int)));

    #endregion
  }

  internal class CompositePanelUIElementPool<T> where T : UIElement
  {
    #region Fields

    private readonly Func<int, T> _builder;
    private readonly Action<T> _cleaner;
    private readonly Panel _panel;
    private readonly Dictionary<int, PanelUIElementPool<T>> _poolDict = new Dictionary<int, PanelUIElementPool<T>>();

    #endregion

    #region Ctors

    public CompositePanelUIElementPool(Panel panel, Func<int, T> builder, Action<T> cleaner)
    {
      _panel = panel;
      _builder = builder;
      _cleaner = cleaner;
    }

    #endregion

    #region  Methods

    public IEnumerable<T> AllElements(int id)
    {
      return GetPool(id).AllElements();
    }

    public IEnumerable<T> AllElements()
    {
      return _poolDict.Values.SelectMany(p => p.AllElements());
    }

    private static T AssignID(T uie, int id)
    {
      uie.SetValue(CompositePanelUIElementPool.PoolIDProperty, id);
      return uie;
    }

    public IEnumerable<T> BusyElements(int id)
    {
      return GetPool(id).BusyElements();
    }

    public IEnumerable<T> BusyElements()
    {
      return _poolDict.Values.SelectMany(p => p.BusyElements());
    }

    public IEnumerable<T> FreeElements(int id)
    {
      return GetPool(id).FreeElements();
    }

    public IEnumerable<T> FreeElements()
    {
      return _poolDict.Values.SelectMany(p => p.FreeElements());
    }

    public T GetElement(int id)
    {
      return GetPool(id).GetElement();
    }

    private static int GetID(T uie)
    {
      return (int) uie.GetValue(CompositePanelUIElementPool.PoolIDProperty);
    }

    private PanelUIElementPool<T> GetPool(int id)
    {
      return _poolDict.GetValueOrCreate(id, () => new PanelUIElementPool<T>(_panel, () => AssignID(_builder(id), id), _cleaner));
    }

    public void ReleaseElement(T element)
    {
      GetPool(GetID(element)).ReleaseElement(element);
    }

    public void Reset()
    {
      foreach (var pool in _poolDict.Values)
        pool.Reset();

      _poolDict.Clear();
    }

    #endregion
  }
}