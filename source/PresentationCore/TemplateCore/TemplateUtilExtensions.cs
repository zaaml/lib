// <copyright file="TemplateUtilExtensions.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.PresentationCore.TemplateCore
{
	internal static class TemplateUtilExtensions
	{
		private static readonly DependencyProperty ElementProperty = DPM.RegisterAttached<object>
			("Element", typeof(TemplateUtilExtensions));

		private static readonly Binding TemplatedParentBinding = new Binding {RelativeSource = XamlConstants.TemplatedParent, BindsDirectlyToSource = true, Mode = BindingMode.OneTime};

		public static FrameworkElement GetImplementationRoot(this FrameworkElement control)
		{
			if (control is UserControl userControl)
				return userControl.Content as FrameworkElement;

			if (control is IImplementationRootProvider implementationRootProvider)
				return implementationRootProvider.ImplementationRoot;

			var childrenCount = VisualTreeHelper.GetChildrenCount(control);

			return childrenCount == 1 ? (FrameworkElement) VisualTreeHelper.GetChild(control, 0) : null;
		}

		public static T GetImplementationRoot<T>(this FrameworkElement control) where T : FrameworkElement
		{
			return (T) GetImplementationRoot(control);
		}

		public static T GetTemplatedParent<T>(this DependencyObject dependencyObject) where T : FrameworkElement
		{
			return (T) dependencyObject.GetTemplatedParent();
		}

		public static FrameworkElement GetTemplatedParent(this DependencyObject dependencyObject)
		{
#if !SILVERLIGHT
			if (dependencyObject is FrameworkElement fre)
				return (FrameworkElement) fre.TemplatedParent;
#endif

			BindingOperations.SetBinding(dependencyObject, ElementProperty, TemplatedParentBinding);

			var element = dependencyObject.GetValue(ElementProperty);

			dependencyObject.ClearValue(ElementProperty);

			return (FrameworkElement) element;
		}

		public static T GetTemplateElement<T>(this DependencyObject dependencyObject, string elementName) where T : DependencyObject
		{
			return (T) dependencyObject.GetTemplateElement(elementName);
		}

		public static DependencyObject GetTemplateElement(this DependencyObject dependencyObject, string elementName)
		{
			if (elementName == null)
				return null;

			dependencyObject.SetBinding(ElementProperty, new Binding {ElementName = elementName, BindsDirectlyToSource = true, Mode = BindingMode.OneTime});

			var element = dependencyObject.GetValue(ElementProperty);

			dependencyObject.ClearValue(ElementProperty);

			return (DependencyObject) element;
		}

		public static T GetTemplateRoot<T>(this DependencyObject dependencyObject) where T : FrameworkElement
		{
			return (T) dependencyObject.GetTemplateRoot();
		}

		public static FrameworkElement GetTemplateRoot(this DependencyObject dependencyObject)
		{
			var templatedParent = dependencyObject.GetTemplatedParent<FrameworkElement>();

			if (templatedParent != null)
			{
				var implementationRoot = templatedParent.GetImplementationRoot();

				return implementationRoot ?? dependencyObject.GetVisualAncestors().OfType<FrameworkElement>().LastOrDefault();
			}

			return null;
		}
	}
}