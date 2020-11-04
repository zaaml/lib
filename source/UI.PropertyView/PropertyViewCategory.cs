// <copyright file="PropertyViewCategory.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.Theming;

namespace Zaaml.UI.Controls.PropertyView
{
	public class PropertyViewCategory : PropertyTreeViewItem
	{
		private static readonly DependencyPropertyKey CategoryPropertyKey = DPM.RegisterReadOnly<PropertyCategory, PropertyViewCategory>
			("Category", default, d => d.OnCategoryPropertyChangedPrivate);

		public static readonly DependencyProperty CategoryProperty = CategoryPropertyKey.DependencyProperty;

		static PropertyViewCategory()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<PropertyViewCategory>();
		}

		public PropertyViewCategory(PropertyViewControl propertyView) : base(propertyView)
		{
			this.OverrideStyleKey<PropertyViewCategory>();
		}

		public PropertyCategory Category
		{
			get => (PropertyCategory) GetValue(CategoryProperty);
			internal set => this.SetReadOnlyValue(CategoryPropertyKey, value);
		}

		private void OnCategoryPropertyChangedPrivate(PropertyCategory oldValue, PropertyCategory newValue)
		{
			Content = newValue?.DisplayName;
		}
	}
}