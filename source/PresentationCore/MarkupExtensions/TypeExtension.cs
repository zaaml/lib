// <copyright file="TypeExtension.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows.Markup;

namespace Zaaml.PresentationCore.MarkupExtensions
{
  public class TypeExtension : MarkupExtensionBase
  {
    #region Properties

    public string TypeName { get; set; }

    #endregion

    #region  Methods

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
      var typeResolver = (IXamlTypeResolver) serviceProvider.GetService(typeof(IXamlTypeResolver));
      var type = typeResolver?.Resolve(TypeName);

      return type;
    }

    #endregion
  }
}