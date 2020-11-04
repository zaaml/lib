// <copyright file="InteractivityCollectionRoot.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace Zaaml.PresentationCore.Interactivity
{
  internal abstract class InteractivityCollectionRoot<TCollection, TItem> : IInteractivityObject, IXamlRootOwner where TCollection : InteractivityCollection<TItem> where TItem : InteractivityObject
  {
    #region Fields

    private object _xamlRoot;

    #endregion

    #region Properties

    public TCollection Collection { get; set; }

    #endregion

    #region  Methods

    protected abstract void OnDescendantApiPropertyChanged(Stack<InteractivityObject> descendants, string propertyName);

    #endregion

    #region Interface Implementations

    #region IInteractivityObject

    IInteractivityObject IInteractivityObject.Parent => null;
		
    void IInteractivityObject.OnDescendantApiPropertyChanged(Stack<InteractivityObject> descendants, string propertyName)
    {
      OnDescendantApiPropertyChanged(descendants, propertyName);
    }

    #endregion

    #region IXamlRootOwner

    object IXamlRootOwner.ActualXamlRoot => _xamlRoot;

    object IXamlRootOwner.XamlRoot
    {
      get => _xamlRoot;
      set => _xamlRoot = value;
    }

    #endregion

    #endregion
  }
}