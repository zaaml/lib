// <copyright file="PropertyGridView.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Controls;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.UI.Controls.PropertyView
{
	public class PropertyGridView : PropertyViewBase
	{
		public static readonly DependencyProperty TemplateProperty = DPM.RegisterAttached<ControlTemplate, PropertyGridView>
			("Template", OnTemplatePropertyChanged);

		private readonly PropertyViewItemGenerator _generator;

		public PropertyGridView()
		{
			_generator = new PropertyViewItemGenerator();
		}

		public static ControlTemplate GetTemplate(DependencyObject element)
		{
			return (ControlTemplate)element.GetValue(TemplateProperty);
		}

		protected override ControlTemplate GetTemplateCore(FrameworkElement frameworkElement)
		{
			return GetTemplate(frameworkElement);
		}

		protected override void OnPropertyViewControlChanged(PropertyViewControl oldValue, PropertyViewControl newValue)
		{
			base.OnPropertyViewControlChanged(oldValue, newValue);

			_generator.PropertyViewControl = PropertyViewControl;
		}

		protected override void OnTemplateContractAttached()
		{
			base.OnTemplateContractAttached();

			var treeView = PropertyViewControl.TreeViewInternal;

			treeView.ItemGenerator = _generator;
			treeView.SourceCollection = PropertyViewControl.ControllerInternal.PropertyCategories;
		}

		protected override void OnTemplateContractDetaching()
		{
			var treeView = PropertyViewControl.TreeViewInternal;

			treeView.SourceCollection = null;
			treeView.ItemGenerator = null;

			base.OnTemplateContractDetaching();
		}

		private static void OnTemplatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (d is PropertyViewControl propertyViewControl)
				propertyViewControl.UpdateViewTemplate();
		}

		public static void SetTemplate(DependencyObject element, ControlTemplate value)
		{
			element.SetValue(TemplateProperty, value);
		}
	}
}