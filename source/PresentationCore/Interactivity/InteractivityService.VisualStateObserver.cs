// <copyright file="InteractivityService.VisualStateObserver.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Zaaml.Core.Extensions;
using Zaaml.Core.Interfaces;
using ZaamlVisualStateGroup = Zaaml.PresentationCore.Interactivity.VSM.VisualStateGroup;

namespace Zaaml.PresentationCore.Interactivity
{
  internal sealed partial class InteractivityService : IVisualStateObserver
  {
    #region Static Fields and Constants

    private static readonly List<IVisualStateListener> DeadListeners = new List<IVisualStateListener>();

    #endregion

    #region Fields

    private readonly List<VisualStateListenerCollection> _stateListenerCollections = new List<VisualStateListenerCollection>();
    private readonly List<VisualStateGroupObserverBase> _visualStateGroupObservers = new List<VisualStateGroupObserverBase>();

    #endregion

    #region  Methods

    private void CleanupVisualStateListeners()
    {
      foreach (var listenerCollection in _stateListenerCollections)
      {
        foreach (var listener in listenerCollection.Listeners)
        {
          if (IsAliveListener(listener) == false)
            DeadListeners.Add(listener);
        }
      }

      foreach (var deadListener in DeadListeners)
        DetachListener(deadListener);

      DeadListeners.Clear();
    }

    private void CreateGroupObservers()
    {
      var implementationRoot = ImplementationRoot;
      if (implementationRoot == null)
        return;

      var advisor = implementationRoot as IVisualStateManagerAdvisor;
      var visualStateGroups = advisor != null ? advisor.VisualStateGroups : VisualStateManager.GetInstance(implementationRoot)?.Groups;

      if (visualStateGroups == null)
        return;

      foreach (var stateGroup in visualStateGroups)
      {
        if (FindGroupObserver(stateGroup.Name) != null)
          continue;

        var visualGroupListener = new VisualGroupListener();

        foreach (var visualState in stateGroup.States)
        {
          var listeners = FindListenerCollection(visualState.Name) ?? new VisualStateListenerCollection(visualState.Name);

          // Template changed. Keep previous visual states.
          if (listeners.IsActive)
            visualGroupListener.GoToState(listeners.StateName, false);

          visualGroupListener.AddStateListenersCollection(listeners);
        }

        _visualStateGroupObservers.Add(new DelegateVisualStateGroupObserver(stateGroup.Name, stateGroup.States.Select(v => v.Name), visualGroupListener));
      }
    }

    private void CreateNativeGroupListeners()
    {
      foreach (var visualStateGroup in EnumerateVisualStateGroups())
      {
        VisualGroupListener groupListener = null;

        foreach (VisualState visualState in visualStateGroup.States)
        {
          var stateListeners = FindListenerCollection(visualState.Name);

          if (stateListeners == null)
            continue;

          if (groupListener == null)
            groupListener = new VisualGroupListener();

          groupListener.AddStateListenersCollection(stateListeners);
        }

        if (groupListener == null)
          continue;

        _visualStateGroupObservers.Add(new VisualStateGroupObserver(visualStateGroup, groupListener));
      }
    }

    private IEnumerable<VisualStateGroup> EnumerateVisualStateGroups()
    {
      var implementationRoot = ImplementationRoot;
      if (implementationRoot == null)
        return Enumerable.Empty<VisualStateGroup>();

      return System.Windows.VisualStateManager.GetVisualStateGroups(implementationRoot)?.Cast<VisualStateGroup>() ?? Enumerable.Empty<VisualStateGroup>();
    }

    private VisualStateGroupObserverBase FindGroupObserver(string name)
    {
      // ReSharper disable once LoopCanBeConvertedToQuery
      foreach (var groupObserver in _visualStateGroupObservers)
        if (groupObserver.Name.Equals(name, StringComparison.Ordinal))
          return groupObserver;

      return null;
    }

    private VisualStateGroupObserverBase FindGroupObserverByStateName(string stateName)
    {
      // ReSharper disable once LoopCanBeConvertedToQuery
      foreach (var groupObserver in _visualStateGroupObservers)
        if (groupObserver.HasVisualState(stateName))
          return groupObserver;

      return null;
    }

    private VisualStateListenerCollection FindListenerCollection(string name)
    {
      // ReSharper disable once LoopCanBeConvertedToQuery
      foreach (var listener in _stateListenerCollections)
        if (listener.StateName.Equals(name, StringComparison.Ordinal))
          return listener;

      return null;
    }

    private VisualStateListenerCollection GetListenerCollection(string name, bool create)
    {
      var listenerCollection = FindListenerCollection(name);

      if (listenerCollection == null && create == false)
        return null;

      var groupObserver = FindGroupObserverByStateName(name);

      if (groupObserver != null)
        listenerCollection = groupObserver.GroupListener.GetListenerCollection(name, create);

      if (listenerCollection != null)
      {
        _stateListenerCollections.Add(listenerCollection);
        return listenerCollection;
      }

      listenerCollection = new VisualStateListenerCollection(name);
      _stateListenerCollections.Add(listenerCollection);

      var visualStateGroup = EnumerateVisualStateGroups().SingleOrDefault(g => g.States.Cast<VisualState>().Any(s => string.Equals(s.Name, name, StringComparison.OrdinalIgnoreCase)));

      if (visualStateGroup == null)
        return listenerCollection;

      var groupListener = new VisualGroupListener();
      groupListener.AddStateListenersCollection(listenerCollection);
      groupObserver = new VisualStateGroupObserver(visualStateGroup, groupListener);
      _visualStateGroupObservers.Add(groupObserver);

      return listenerCollection;
    }

    public bool GoToState(string stateName, VisualStateGroup group, VisualState state, bool useTransitions)
    {
      if (ImplementationRoot == null)
        UpdateImplementationRoot();

      if (group == null || state == null || group.Name.IsNullOrEmpty() || state.Name.IsNullOrEmpty())
      {
        var groupObserver = FindGroupObserverByStateName(stateName) as DelegateVisualStateGroupObserver;

        if (groupObserver == null)
          return false;

        groupObserver.GoToState(stateName, useTransitions);
        return true;
      }

      var groupListener = FindGroupObserver(group.Name)?.GroupListener;

      if (groupListener == null) return false;

      groupListener.GoToState(state.Name, useTransitions);

      return true;
    }

    private static bool IsAliveListener(IVisualStateListener listener)
    {
      var weakListener = listener as IWeakReference;
      return weakListener == null || weakListener.IsAlive;
    }

    private void UpdateGroupListeners()
    {
      foreach (var visualStateGroupObserver in _visualStateGroupObservers)
        visualStateGroupObserver.Dispose();

      _visualStateGroupObservers.Clear();

      CreateNativeGroupListeners();
      CreateGroupObservers();
    }

    #endregion

    #region Interface Implementations

    #region IVisualStateObserver

    public void AttachListener(IVisualStateListener listener)
    {
      EnsureLayoutUpdatedHandler();
      GetListenerCollection(listener.VisualStateName, true).AttachListener(listener);
    }

    public void DetachListener(IVisualStateListener listener)
    {
      EnsureLayoutUpdatedHandler();
      GetListenerCollection(listener.VisualStateName, false)?.DetachListener(listener);
    }

    #endregion

    #endregion
  }

  internal interface IVisualStateManagerAdvisor
  {
    #region Properties

    IEnumerable<ZaamlVisualStateGroup> VisualStateGroups { get; }

    #endregion
  }
}