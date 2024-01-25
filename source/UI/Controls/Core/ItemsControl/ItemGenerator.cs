// <copyright file="ItemGenerator.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using System.Windows.Data;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.Extensions;

namespace Zaaml.UI.Controls.Core
{
	public abstract class ItemGenerator<T> : InheritanceContextObject, IItemGenerator<T> where T : FrameworkElement
	{
		internal event EventHandler GeneratorChangedCore;
		internal event EventHandler GeneratorChangingCore;

		protected virtual bool SupportsRecycling => false;

		internal bool SupportsRecyclingInternal => SupportsRecycling;

		protected abstract void AttachItem(T item, object source);

		internal virtual void AttachItemCore(T item, object source)
		{
			AttachItem(item, source);
		}

		protected abstract T CreateItem(object source);

		internal virtual T CreateItemCore(object source)
		{
			return CreateItem(source);
		}

		protected abstract void DetachItem(T item, object source);

		internal virtual void DetachItemCore(T item, object source)
		{
			DetachItem(item, source);
		}

		protected abstract void DisposeItem(T item, object source);

		internal virtual void DisposeItemCore(T item, object source)
		{
			DisposeItem(item, source);
		}

		internal static void InstallBinding(T item, DependencyProperty property, Binding binding)
		{
			if (binding != null)
				item.SetBinding(property, binding);
		}

		protected virtual void OnGeneratorChanged()
		{
			GeneratorChangedCore?.Invoke(this, EventArgs.Empty);
		}

		internal void OnGeneratorChangedInt()
		{
			OnGeneratorChanged();
		}

		protected virtual void OnGeneratorChanging()
		{
			GeneratorChangingCore?.Invoke(this, EventArgs.Empty);
		}

		internal void OnGeneratorChangingInt()
		{
			OnGeneratorChanging();
		}

		internal static void UninstallBinding(T item, DependencyProperty property, Binding binding)
		{
			if (binding != null && ReferenceEquals(item.ReadLocalBinding(property), binding))
				item.ClearValue(property);
		}

		void IItemGenerator<T>.AttachItem(T item, object source)
		{
			AttachItemCore(item, source);
		}

		T IItemGenerator<T>.CreateItem(object source)
		{
			return CreateItemCore(source);
		}

		void IItemGenerator<T>.DetachItem(T item, object source)
		{
			DetachItemCore(item, source);
		}

		void IItemGenerator<T>.DisposeItem(T item, object source)
		{
			DisposeItemCore(item, source);
		}
	}
}