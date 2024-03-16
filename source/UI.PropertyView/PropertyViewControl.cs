// <copyright file="PropertyViewControl.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using System.Windows.Controls;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.PresentationCore.Utils;
using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.PropertyView
{
	[TemplateContractType<PropertyViewControlTemplateContract>]
	public class PropertyViewControl : TemplateContractControl
	{
		public static readonly DependencyProperty SelectedObjectProperty = DPM.Register<object, PropertyViewControl>
			("SelectedObject", d => d.OnSelectedObjectPropertyChangedPrivate);

		public static readonly DependencyProperty FilterProperty = DPM.Register<IPropertyViewFilter, PropertyViewControl>
			("Filter", d => d.OnFilterPropertyChangedPrivate);

		public static readonly DependencyProperty ViewProperty = DPM.Register<PropertyViewBase, PropertyViewControl>
			("View", d => d.OnViewPropertyChangedPrivate);

		private static readonly DependencyPropertyKey ActualViewTemplatePropertyKey = DPM.RegisterReadOnly<ControlTemplate, PropertyViewControl>
			("ActualViewTemplate");

		public static readonly DependencyProperty ActualViewTemplateProperty = ActualViewTemplatePropertyKey.DependencyProperty;

		private PropertyViewController _controller;

		static PropertyViewControl()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<PropertyViewControl>();
			UIElementUtils.OverrideFocusable<PropertyViewControl>(false);
		}

		public PropertyViewControl()
		{
			this.OverrideStyleKey<PropertyViewControl>();

			ViewController = new PropertyGridViewController(this);
		}

		public ControlTemplate ActualViewTemplate
		{
			get => (ControlTemplate)GetValue(ActualViewTemplateProperty);
			private set => this.SetReadOnlyValue(ActualViewTemplatePropertyKey, value);
		}

		protected PropertyViewController Controller => _controller ??= CreateController();

		internal PropertyViewController ControllerInternal => Controller;

		public string Filter
		{
			get => (string)GetValue(FilterProperty);
			set => SetValue(FilterProperty, value);
		}

		public object SelectedObject
		{
			get => GetValue(SelectedObjectProperty);
			set => SetValue(SelectedObjectProperty, value);
		}

		private PropertyViewControlTemplateContract TemplateContract => (PropertyViewControlTemplateContract)TemplateContractCore;

		private PropertyTreeViewControl TreeView => TemplateContract.TreeView;

		internal PropertyTreeViewControl TreeViewInternal => TreeView;

		public PropertyViewBase View
		{
			get => (PropertyViewBase)GetValue(ViewProperty);
			set => SetValue(ViewProperty, value);
		}

		internal PropertyGridViewController ViewController { get; }

		private void ApplyFilter()
		{
		}

		protected virtual PropertyViewController CreateController()
		{
			return new PropertyViewController(this);
		}

		private void OnFilterChanged(object sender, EventArgs e)
		{
			ApplyFilter();
		}

		private void OnFilterPropertyChangedPrivate(IPropertyViewFilter oldValue, IPropertyViewFilter newValue)
		{
			if (oldValue != null) oldValue.Changed -= OnFilterChanged;
			if (newValue != null) newValue.Changed += OnFilterChanged;

			ApplyFilter();
		}

		private void OnSelectedObjectPropertyChangedPrivate(object oldValue, object newValue)
		{
			Controller.SelectedObject = newValue;
		}

		protected override void OnTemplateContractAttached()
		{
			base.OnTemplateContractAttached();

			View?.OnTemplateContractAttachedInternal();
		}

		protected override void OnTemplateContractDetaching()
		{
			View?.OnTemplateContractDetachingInternal();

			TreeView.SourceCollection = null;
			TreeView.ItemGenerator = null;

			base.OnTemplateContractDetaching();
		}

		private void OnViewPropertyChangedPrivate(PropertyViewBase oldValue, PropertyViewBase newValue)
		{
			if (ReferenceEquals(oldValue, newValue))
				return;

			if (oldValue != null)
			{
				if (ReferenceEquals(oldValue.PropertyViewControl, this) == false)
					throw new InvalidOperationException();

				oldValue.PropertyViewControl = null;
			}

			if (newValue != null)
			{
				if (newValue.PropertyViewControl != null)
					throw new InvalidOperationException();

				newValue.PropertyViewControl = this;
			}

			UpdateViewTemplate();
		}

		internal void UpdateProperties()
		{
			if (TreeView != null)
				TreeView.SourceCollection = Controller.PropertyCategories;
		}

		internal void UpdateViewTemplate()
		{
			var actualViewTemplate = View?.GetTemplateInternal(this);

			if (ReferenceEquals(ActualViewTemplate, actualViewTemplate) == false)
				ActualViewTemplate = actualViewTemplate;
		}
	}
}