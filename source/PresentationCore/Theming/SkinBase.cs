// <copyright file="SkinBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using Zaaml.PresentationCore.Interactivity;
using Setter = Zaaml.PresentationCore.Interactivity.Setter;

namespace Zaaml.PresentationCore.Theming
{
	[TypeConverter(typeof(SkinTypeConverter))]
	public abstract class SkinBase : InheritanceContextObject, IInteractivityVisitor
	{
		internal SkinBase()
		{
		}

		internal abstract IEnumerable<KeyValuePair<string, object>> Resources { get; }

		protected abstract object GetValue(string key);

		internal object GetValueInternal(string key)
		{
			return GetValue(key);
		}

		internal object GetValueInternal(ThemeResourceKey key)
		{
			return key.IsEmpty ? null : GetValueInternal(key.Key);
		}

		protected virtual void OnAttached(DependencyObject dependencyObject)
		{
		}

		internal void OnAttachedInternal(DependencyObject dependencyObject)
		{
			OnAttached(dependencyObject);
		}

		protected virtual void OnDetached(DependencyObject dependencyObject)
		{
		}

		internal void OnDetachedInternal(DependencyObject dependencyObject)
		{
			OnDetached(dependencyObject);
		}

		void IInteractivityVisitor.Visit(InteractivityObject interactivityObject)
		{
			var setter = interactivityObject as Setter;

			setter?.UpdateSkin(this);
		}
	}
}