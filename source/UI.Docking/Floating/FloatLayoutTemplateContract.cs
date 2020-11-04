// <copyright file="FloatLayoutTemplateContract.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.PresentationCore.TemplateCore;

namespace Zaaml.UI.Controls.Docking
{
  internal sealed class FloatLayoutTemplateContract : TemplateContract
  {
    #region Ctors

    public FloatLayoutTemplateContract(GetTemplateChild templateDiscovery)
      : base(templateDiscovery)
    {
    }

    #endregion
  }
}