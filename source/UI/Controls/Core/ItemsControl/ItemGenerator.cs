// <copyright file="ItemGenerator.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using Zaaml.PresentationCore;

namespace Zaaml.UI.Controls.Core
{
	public abstract class ItemGenerator<T> : InheritanceContextObject, IItemGenerator<T> where T : FrameworkElement
	{
		internal event EventHandler GeneratorChangedCore;
		internal event EventHandler GeneratorChangingCore;

		protected virtual bool SupportsRecycling => false;

		internal bool SupportsRecyclingInternal => SupportsRecycling;

		protected abstract void AttachItem(T item, object itemSource);

		internal virtual void AttachItemCore(T item, object itemSource)
		{
			AttachItem(item, itemSource);
		}

		protected abstract T CreateItem(object itemSource);

		internal virtual T CreateItemCore(object itemSource)
		{
			return CreateItem(itemSource);
		}

		protected abstract void DetachItem(T item, object itemSource);

		internal virtual void DetachItemCore(T item, object itemSource)
		{
			DetachItem(item, itemSource);
		}

		protected abstract void DisposeItem(T item, object itemSource);

		internal virtual void DisposeItemCore(T item, object itemSource)
		{
			DisposeItem(item, itemSource);
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

		void IItemGenerator<T>.AttachItem(T item, object itemSource)
		{
			AttachItemCore(item, itemSource);
		}

		T IItemGenerator<T>.CreateItem(object itemSource)
		{
			return CreateItemCore(itemSource);
		}

		void IItemGenerator<T>.DetachItem(T item, object itemSource)
		{
			DetachItemCore(item, itemSource);
		}

		void IItemGenerator<T>.DisposeItem(T item, object itemSource)
		{
			DisposeItemCore(item, itemSource);
		}
	}
}