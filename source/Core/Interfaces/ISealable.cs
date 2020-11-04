// <copyright file="ISealable.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Core.Interfaces
{
  internal interface ISealable
  {
    #region Properties

    bool IsSealed { get; }

    #endregion

    #region  Methods

    void Seal();

    #endregion
  }
}