// <copyright file="IXamlRootOwner.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.PresentationCore.Interactivity
{
  internal interface IXamlRootOwner
  {
    #region Properties

    object ActualXamlRoot { get; }
    object XamlRoot { get; set; }

    #endregion
  }
}
