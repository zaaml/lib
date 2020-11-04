// <copyright file="WindowButtonCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.PresentationCore;

namespace Zaaml.UI.Windows
{
  public sealed class WindowButtonCollection : DependencyObjectCollectionBase<WindowButton>
  {
    #region Fields

    private readonly IWindowButtonCollectionOwner _owner;

    #endregion

    #region Ctors

    internal WindowButtonCollection(IWindowButtonCollectionOwner owner)
    {
      _owner = owner;
    }

    #endregion

    #region  Methods

    protected override void OnItemAdded(WindowButton button)
    {
      base.OnItemAdded(button);

      _owner.OnButtonAdded(button);
    }

    protected override void OnItemRemoved(WindowButton button)
    {
      _owner.OnButtonRemoved(button);
      base.OnItemRemoved(button);
    }

    #endregion
  }

  internal interface IWindowButtonCollectionOwner
  {
    #region  Methods

    void OnButtonAdded(WindowButton button);
    void OnButtonRemoved(WindowButton button);

    #endregion
  }
}