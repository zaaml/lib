// <copyright file="RefactoringException.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.Core
{
  internal class RefactoringException : Exception
  {
    #region Ctors

    public RefactoringException()
    {
    }

    public RefactoringException(string message) : base(message)
    {
    }

    #endregion
  }
}