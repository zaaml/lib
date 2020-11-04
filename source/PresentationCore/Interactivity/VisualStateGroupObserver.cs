// <copyright file="VisualStateGroupObserver.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Zaaml.PresentationCore.Interactivity
{
  internal abstract class VisualStateGroupObserverBase : IDisposable
  {
    #region Ctors

    protected VisualStateGroupObserverBase(VisualGroupListener groupListener)
    {
      GroupListener = groupListener;
    }

    #endregion

    #region Properties

    public VisualGroupListener GroupListener { get; }
    public abstract string Name { get; }

    #endregion

    #region  Methods

    public abstract bool HasVisualState(string visualState);

    public override string ToString()
    {
      return $"{Name}";
    }

    #endregion

    #region Interface Implementations

    #region IDisposable

    public abstract void Dispose();

    #endregion

    #endregion
  }

  internal class VisualStateGroupObserver : VisualStateGroupObserverBase
  {
    #region Fields

    private readonly VisualStateGroup _visualGroup;
    private readonly HashSet<string> _visualStates;

    #endregion

    #region Ctors

    public VisualStateGroupObserver(VisualStateGroup visualGroup, VisualGroupListener groupListener) : base(groupListener)
    {
      _visualGroup = visualGroup;
      _visualStates = new HashSet<string>(visualGroup.States.Cast<VisualState>().Select(s => s.Name), StringComparer.OrdinalIgnoreCase);
      _visualGroup.CurrentStateChanged += VisualGroupOnCurrentStateChanged;

      Update();
    }

    #endregion

    #region Properties

    public override string Name => _visualGroup.Name;

    #endregion

    #region  Methods

    public void Detach()
    {
      _visualGroup.CurrentStateChanged -= VisualGroupOnCurrentStateChanged;
    }

    public override void Dispose()
    {
      Detach();
    }

    public override bool HasVisualState(string visualState) => _visualStates.Contains(visualState);

    private void Update(VisualState state = null)
    {
      GroupListener.GoToState((state ?? _visualGroup.CurrentState)?.Name, false);
    }

    private void VisualGroupOnCurrentStateChanged(object sender, VisualStateChangedEventArgs e)
    {
      Update(e.NewState);
    }

    #endregion
  }

  internal class DelegateVisualStateGroupObserver : VisualStateGroupObserverBase
  {
    #region Fields

    private readonly List<string> _states;

    #endregion

    #region Ctors

    public DelegateVisualStateGroupObserver(string groupName, IEnumerable<string> states, VisualGroupListener groupListener) : base(groupListener)
    {
      _states = states.ToList();
      Name = groupName;
    }

    #endregion

    #region Properties

    public override string Name { get; }

    #endregion

    #region  Methods

    public override void Dispose()
    {
    }

    public void GoToState(string state, bool useTransitions)
    {
      GroupListener.GoToState(state, useTransitions);
    }

    public override bool HasVisualState(string visualState)
    {
      return _states.Contains(visualState);
    }

    #endregion
  }
}