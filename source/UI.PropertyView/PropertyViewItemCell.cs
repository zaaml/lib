// <copyright file="PropertyViewItemCell.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Markup;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.PropertyView
{
	[ContentProperty(nameof(Child))]
	[TemplateContractType(typeof(PropertyViewItemCellTemplateContract))]
	public sealed class PropertyViewItemCell
		: GridCell<PropertyViewItemCellsPresenter,
			PropertyViewItemCellsPanel,
			PropertyViewItemCellCollection,
			PropertyViewItemCell>
	{
		public static readonly DependencyProperty ChildProperty = DPM.Register<UIElement, PropertyViewItemCell>
			("Child", p => p.OnChildPropertyChangedPrivate);

		static PropertyViewItemCell()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<PropertyViewItemCell>();
		}

		public PropertyViewItemCell()
		{
			this.OverrideStyleKey<PropertyViewItemCell>();
		}

		public UIElement Child
		{
			get => (UIElement) GetValue(ChildProperty);
			set => SetValue(ChildProperty, value);
		}

		private void OnChildPropertyChangedPrivate()
		{
			ChildCore = Child;
		}
	}
	public sealed class PropertyViewItemCellTemplateContract : GridCellTemplateContract
	{
	}
}