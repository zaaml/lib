// <copyright file="TemplateValidationException.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.PresentationCore.TemplateCore
{
  public class TemplateValidationException : Exception
  {
    #region Ctors

    internal TemplateValidationException(string partName) : base($"Template does not have required part: \"{partName}\"")
    {
    }

    #endregion
  }
}