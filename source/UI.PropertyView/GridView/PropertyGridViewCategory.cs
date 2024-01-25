// <copyright file="PropertyViewCategory.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.Theming;

namespace Zaaml.UI.Controls.PropertyView
{
	public class PropertyGridViewCategory : PropertyTreeViewItem
	{
		private static readonly DependencyPropertyKey CategoryPropertyKey = DPM.RegisterReadOnly<PropertyCategory, PropertyGridViewCategory>
			("Category", default, d => d.OnCategoryPropertyChangedPrivate);

		public static readonly DependencyProperty CategoryProperty = CategoryPropertyKey.DependencyProperty;

		static PropertyGridViewCategory()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<PropertyGridViewCategory>();
		}

		public PropertyGridViewCategory(PropertyViewControl propertyViewControl) : base(propertyViewControl)
		{
			this.OverrideStyleKey<PropertyGridViewCategory>();
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