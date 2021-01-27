// <copyright file="PropertyViewControl.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.Core;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.PresentationCore.Utils;
using Zaaml.UI.Controls.Core;
using Zaaml.UI.Controls.TreeView;

namespace Zaaml.UI.Controls.PropertyView
{
	[TemplateContractType(typeof(PropertyViewControlTemplateContract))]
	public class PropertyViewControl : TemplateContractControl
	{
		public static readonly DependencyProperty SelectedObjectProperty = DPM.Register<object, PropertyViewControl>
			("SelectedObject", default, d => d.OnSelectedObjectPropertyChangedPrivate);

		public static readonly DependencyProperty SortingProperty = DPM.Register<PropertyViewSorting, PropertyViewControl>
			("Sorting", default, d => d.OnSortingPropertyChangedPrivate);

		public static readonly DependencyProperty FilterProperty = DPM.Register<string, PropertyViewControl>
			("Filter", default, d => d.OnFilterPropertyChangedPrivate);

		private readonly PropertyViewItemGenerator _generator;

		private PropertyViewController _controller;

		static PropertyViewControl()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<PropertyViewControl>();
			UIElementUtils.OverrideFocusable<PropertyViewControl>(false);
		}

		public PropertyViewControl()
		{
			this.OverrideStyleKey<PropertyViewControl>();

			_generator = new PropertyViewItemGenerator(this);

			ItemGridController = new PropertyViewItemGridController(this);
		}

		protected PropertyViewController Controller => _controller ??= CreateController();

		internal PropertyViewController ControllerInternal => Controller;

		public string Filter
		{
			get => (string) GetValue(FilterProperty);
			set => SetValue(FilterProperty, value);
		}

		internal PropertyViewItemGridController ItemGridController { get; }

		public object SelectedObject
		{
			get => GetValue(SelectedObjectProperty);
			set => SetValue(SelectedObjectProperty, value);
		}

		public PropertyViewSorting Sorting
		{
			get => (PropertyViewSorting) GetValue(SortingProperty);
			set => SetValue(SortingProperty, value);
		}

		private PropertyViewControlTemplateContract TemplateContract => (PropertyViewControlTemplateContract) TemplateContractInternal;

		private TreeViewControl TreeView => TemplateContract.TreeView;

		internal Size TreeViewPanelAvailableSize { get; set; }

		private void ApplyFilter()
		{
		}

		protected virtual PropertyViewController CreateController()
		{
			return new PropertyViewController(this);
		}

		private void OnFilterPropertyChangedPrivate(string oldValue, string newValue)
		{
			ApplyFilter();
		}

		private void OnSelectedObjectPropertyChangedPrivate(object oldValue, object newValue)
		{
			Controller.SelectedObject = newValue;
		}

		private void OnSortingPropertyChangedPrivate(PropertyViewSorting oldValue, PropertyViewSorting newValue)
		{
			Controller.PropertyViewSorting = newValue;
		}

		protected override void OnTemplateContractAttached()
		{
			base.OnTemplateContractAttached();

			TreeView.ItemGenerator = _generator;
			TreeView.SourceCollection = Controller.PropertyCategories;
		}

		protected override void OnTemplateContractDetaching()
		{
			TreeView.SourceCollection = null;
			TreeView.ItemGenerator = null;

			base.OnTemplateContractDetaching();
		}

		internal void UpdateProperties()
		{
			if (TreeView != null)
				TreeView.SourceCollection = Controller.PropertyCategories;
		}
	}

	public class PropertyViewControlTemplateContract : TemplateContract
	{
		[TemplateContractPart(Required = true)]
		public TreeViewControl TreeView { get; [UsedImplicitly] private set; }
	}
}