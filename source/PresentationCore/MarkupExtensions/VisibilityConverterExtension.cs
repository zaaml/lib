// <copyright file="VisibilityConverterExtension.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using Zaaml.PresentationCore.Converters;

namespace Zaaml.PresentationCore.MarkupExtensions
{
  public sealed class VisibilityConverterExtension : MarkupExtensionBase
  {
    #region  Methods

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
      return VisibilityConverter.FalseToCollapsedVisibility;
    }

    #endregion
  }

  public sealed class IsSubclassOfVisibilityConverterExtension : MarkupExtensionBase
  {
	  public bool Self { get; set; } = true;

	  public Type Type { get; set; }

	  public Visibility TrueVisibility { get; set; } = Visibility.Visible;

	  public Visibility FalseVisibility { get; set; } = Visibility.Collapsed;

		#region  Methods

		public override object ProvideValue(IServiceProvider serviceProvider)
	  {
		  return new IsSubclassOfVisibilityConverter
		  {
				Self = Self,
				Type = Type,
				TrueVisibility = TrueVisibility,
				FalseVisibility = FalseVisibility
		  };
	  }

	  #endregion
  }
}