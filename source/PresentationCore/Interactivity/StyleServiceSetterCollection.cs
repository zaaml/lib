// <copyright file="StyleServiceSetterCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using Zaaml.PresentationCore.Theming;

namespace Zaaml.PresentationCore.Interactivity
{
  internal sealed class StyleServiceSetterCollection : SetterCollectionBase
  {
    #region Ctors

    internal StyleServiceSetterCollection(StyleService styleService) : base(null)
    {
      Service = styleService;
    }

    #endregion

    #region Properties

    protected override bool IsApplied => false;

    public StyleService Service { get; }

    #endregion

    #region  Methods

    internal override InteractivityCollection<SetterBase> CreateInstance(IInteractivityObject parent)
    {
      throw new NotSupportedException();
    }

    #endregion
  }
}