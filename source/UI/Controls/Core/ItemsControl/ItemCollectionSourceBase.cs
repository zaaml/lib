// <copyright file="ItemCollectionSourceBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;

namespace Zaaml.UI.Controls.Core
{
	internal abstract class ItemCollectionSourceBase<TControl, TItem> : IDisposable
		where TItem : System.Windows.Controls.Control
		where TControl : System.Windows.Controls.Control
	{
		#region Fields

		private ItemGenerator<TItem> _generator;

		#endregion

		#region Ctors

		protected ItemCollectionSourceBase(IItemsHost<TItem> itemsHost, ItemCollectionBase<TControl, TItem> itemCollection)
		{
			ItemsHost = itemsHost;
			ItemCollection = itemCollection;
		}

		#endregion

		#region Properties

		public abstract IEnumerable<TItem> ActualItems { get; }

		public abstract int Count { get; }

		public ItemGenerator<TItem> Generator
		{
			get => _generator;
			set
			{
				if (ReferenceEquals(_generator, value))
					return;

				if (_generator != null)
					DetachGenerator();

				_generator = value;

				if (_generator != null)
					AttachGenerator();

				OnGeneratorChanged();
			}
		}

		protected int GeneratorVersion { get; private set; }

		public ItemCollectionBase<TControl, TItem> ItemCollection { get; }

		public IItemsHost<TItem> ItemsHost { get; }

		public abstract IEnumerable Source { get; set; }

		#endregion

		#region  Methods

		private void AttachGenerator()
		{
			Generator.GeneratorChangedCore += OnGeneratorChanged;

			OnGeneratorAttached();
		}

		private void DetachGenerator()
		{
			Generator.GeneratorChangedCore -= OnGeneratorChanged;

			OnGeneratorDetached();
		}

		public abstract TItem EnsureItem(int index);

		protected abstract int GetIndexFromItem(TItem item);

		internal int GetIndexFromItemInt(TItem item)
		{
			return GetIndexFromItem(item);
		}

		protected abstract int GetIndexFromItemSource(object itemSource);

		internal int GetIndexFromItemSourceInt(object itemSource)
		{
			return GetIndexFromItemSource(itemSource);
		}

		protected abstract TItem GetItemFromIndex(int index);

		internal TItem GetItemFromIndexInt(int index)
		{
			return GetItemFromIndex(index);
		}

		protected abstract object GetItemSourceFromIndex(int index);

		internal object GetItemSourceFromIndexInt(int index)
		{
			return GetItemSourceFromIndex(index);
		}

		public abstract void LockItem(TItem item);

		protected virtual void OnGeneratorAttached()
		{
		}

		protected virtual void OnGeneratorChanged()
		{
		}

		private void OnGeneratorChanged(object sender, EventArgs e)
		{
			GeneratorVersion++;

			OnGeneratorVersionChanged();
		}

		protected virtual void OnGeneratorDetached()
		{
		}

		protected virtual void OnGeneratorVersionChanged()
		{
		}

		public abstract void UnlockItem(TItem item);

		#endregion

		#region Interface Implementations

		#region IDisposable

		public virtual void Dispose()
		{
		}

		#endregion

		#endregion
	}
}