// <copyright file="IContentControl.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Controls;
using Zaaml.PresentationCore.Interfaces;
using Zaaml.UI.Controls.Primitives.ContentPrimitives;

namespace Zaaml.UI.Controls.Interfaces
{
  internal interface IContentControl : IControl
  {
    #region Properties

    object Content { get; set; }

    DependencyProperty ContentProperty { get; }

    string ContentStringFormat { get; set; }

    DependencyProperty ContentStringFormatProperty { get; }

    DataTemplate ContentTemplate { get; set; }

    DependencyProperty ContentTemplateProperty { get; }

    DataTemplateSelector ContentTemplateSelector { get; set; }

    DependencyProperty ContentTemplateSelectorProperty { get; }

    #endregion
  }
  
  internal interface IIconContentControl : IContentControl
	{
		IconBase Icon { get; set; }
		
		DependencyProperty IconProperty { get; }
	}
}