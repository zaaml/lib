// <copyright file="StyleSetterCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using Zaaml.PresentationCore.Theming;

#pragma warning disable 108

namespace Zaaml.PresentationCore.Interactivity
{
  internal sealed class StyleSetterCollection : SetterCollectionBase
  {
    #region Ctors

    internal StyleSetterCollection(StyleBase style) : base(null)
    {
      Style = style;
    }

    #endregion

    #region Properties

    protected override bool IsApplied => false;

    internal StyleBase Style { get; }

    #endregion

    #region  Methods

    internal override InteractivityCollection<SetterBase> CreateInstance(IInteractivityObject parent)
    {
      throw new NotSupportedException();
    }

    protected override void OnItemAddedCore(SetterBase setter)
    {
      base.OnItemAddedCore(setter);

      if (Style.Source == null)
        Style.Source = setter.Uri;
      else if (Style.Source.Equals(setter.Uri) == false)
        throw new InvalidOperationException();
    }

    #endregion
  }
}