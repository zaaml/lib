// <copyright file="CommonVisualStateGroups.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using Zaaml.PresentationCore.MarkupExtensions;

namespace Zaaml.PresentationCore.Interactivity.VSM
{
  public sealed class CommonVisualStateGroupsExtension : MarkupExtensionBase
  {
    #region Properties

    public VisualStateGroupKind Groups { get; set; }

    #endregion

    #region  Methods

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
      var collection = new VisualStateGroupCollection();
      collection.AddRange(VisualGroups.EnumerateGroups(Groups));
      return collection;
    }

    #endregion
  }
}
