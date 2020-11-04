// <copyright file="StrongReferenceBinding.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.PresentationCore.Data.MarkupExtensions
{
  internal class StrongReferenceBinding : System.Windows.Data.Binding
  {
    public StrongReferenceBinding()
    {
    }

    public StrongReferenceBinding(string path) : base(path)
    {
    }

    #region Properties

    public object Reference { get; set; }

    #endregion
  }
}