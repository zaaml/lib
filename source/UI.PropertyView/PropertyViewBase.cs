// <copyright file="PropertyViewBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Controls;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.PropertyView
{
	public abstract class PropertyViewBase : ControlViewBase
	{
		private static readonly DependencyPropertyKey PropertyViewControlPropertyKey = DPM.RegisterReadOnly<PropertyViewControl, PropertyViewBase>
			("PropertyViewControl", d => d.OnPropertyViewControlPropertyChangedPrivate);

		public static readonly DependencyProperty PropertyViewControlProperty = PropertyViewControlPropertyKey.DependencyProperty;

		public PropertyViewControl PropertyViewControl
		{
			get => (PropertyViewControl)GetValue(PropertyViewControlProperty);
			internal set => this.SetReadOnlyValue(PropertyViewControlPropertyKey, value);
		}

		protected abstract ControlTemplate GetTemplateCore(FrameworkElement frameworkElement);

		internal ControlTemplate GetTemplateInternal(FrameworkElement frameworkElement)
		{
			return GetTemplateCore(frameworkElement);
		}

		protected virtual void OnPropertyViewControlChanged(PropertyViewControl oldValue, PropertyViewControl newValue)
		{
		}

		private void OnPropertyViewControlPropertyChangedPrivate(PropertyViewControl oldValue, PropertyViewControl newValue)
		{
			OnPropertyViewControlChanged(oldValue, newValue);
		}

		protected virtual void OnTemplateContractAttached()
		{
		}

		protected virtual void OnTemplateContractDetaching()
		{
		}

		internal void OnTemplateContractAttachedInternal()
		{
			OnTemplateContractAttached();
		}

		internal void OnTemplateContractDetachingInternal()
		{
			OnTemplateContractDetaching();
		}
	}
}