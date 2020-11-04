// <copyright file="StyleServiceTriggerCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using Zaaml.PresentationCore.Theming;

namespace Zaaml.PresentationCore.Interactivity
{
  internal sealed class StyleServiceTriggerCollection : TriggerCollectionBase
  {
    #region Ctors

    internal StyleServiceTriggerCollection(StyleService styleService) : base(null)
    {
      Service = styleService;
    }

    #endregion

    #region Properties

    public StyleService Service { get; }

    #endregion

    #region  Methods

    internal override InteractivityCollection<TriggerBase> CreateInstance(IInteractivityObject parent)
    {
      throw new NotSupportedException();
    }

    #endregion
  }
}