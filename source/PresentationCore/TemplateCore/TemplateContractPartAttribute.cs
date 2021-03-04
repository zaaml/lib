// <copyright file="TemplateContractPartAttribute.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.PresentationCore.TemplateCore
{
  [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
  public class TemplateContractPartAttribute : Attribute
  {
    #region Properties

    public string Name { get; set; }

    public bool Required { get; set; }

    #endregion
  }
}