// <copyright file="StyleTriggerCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using Zaaml.PresentationCore.Theming;

#pragma warning disable 108

namespace Zaaml.PresentationCore.Interactivity
{
  internal sealed class StyleTriggerCollection : TriggerCollectionBase
  {
    #region Ctors

    internal StyleTriggerCollection(StyleBase style) : base(null)
    {
      Style = style;
    }

    #endregion

    #region Properties

    internal StyleBase Style { get; }

    #endregion

    #region  Methods

    internal override InteractivityCollection<TriggerBase> CreateInstance(IInteractivityObject parent)
    {
      throw new NotSupportedException();
    }

    #endregion
  }
}