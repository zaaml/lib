// <copyright file="DockItemLayoutCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using Zaaml.PresentationCore;

namespace Zaaml.UI.Controls.Docking
{
	public sealed class DockItemLayoutCollection : InheritanceContextDependencyObjectCollection<DockItemLayout>
	{
		private readonly Dictionary<string, DockItemLayout> _dictionary = new(StringComparer.OrdinalIgnoreCase);

		internal DockItemLayoutCollection():this(null)
		{
		}
		
		internal DockItemLayoutCollection(IDockItemLayoutCollectionOwner itemsOwner)
		{
			ItemsOwner = itemsOwner ?? NullOwner.Instance;
		}

		public DockItemLayout this[string name] => _dictionary[name];

		internal IDockItemLayoutCollectionOwner ItemsOwner { get; }

		internal bool Contains(string name)
		{
			return name != null && _dictionary.ContainsKey(name);
		}

		internal DockItemLayout GetItemLayout(string name)
		{
			return name != null ? Zaaml.Core.Extensions.IDictionaryExtensions.GetValueOrDefault(_dictionary, name) : null;
		}

		protected override void OnItemAdded(DockItemLayout dockItemLayout)
		{
			base.OnItemAdded(dockItemLayout);

			try
			{
				VerifyName(dockItemLayout);

				if (dockItemLayout.ItemLayoutCollections.Add(this) == false)
					throw new InvalidOperationException();

				if (string.IsNullOrEmpty(dockItemLayout.ItemNameInternal) == false)
					_dictionary[dockItemLayout.ItemNameInternal] = dockItemLayout;
			}
			finally
			{
				ItemsOwner.OnItemAdded(dockItemLayout);
			}
		}

		protected override void OnItemRemoved(DockItemLayout dockItemLayout)
		{
			base.OnItemRemoved(dockItemLayout);

			try
			{
				if (string.IsNullOrEmpty(dockItemLayout.ItemNameInternal) == false)
					_dictionary.Remove(dockItemLayout.ItemNameInternal);

				if (dockItemLayout.ItemLayoutCollections.Remove(this) == false)
					throw new InvalidOperationException();
			}
			finally
			{
				ItemsOwner.OnItemRemoved(dockItemLayout);
			}
		}

		internal void Replace(DockItemLayout oldDockItemLayout, DockItemLayout newDockItemLayout)
		{
			var index = IndexOf(oldDockItemLayout);

			if (index == -1)
				return;

			this[index] = newDockItemLayout;
		}

		internal bool TryGetItemLayout(string name, out DockItemLayout value)
		{
			return _dictionary.TryGetValue(name, out value);
		}

		private void VerifyName(DockItemLayout dockItemLayout)
		{
			if (string.IsNullOrEmpty(dockItemLayout.ItemNameInternal))
				return;

			if (_dictionary.ContainsKey(dockItemLayout.ItemNameInternal))
				throw new InvalidOperationException("DockItemLayout with the same Name already exists in this collection");
		}

		private sealed class NullOwner : IDockItemLayoutCollectionOwner
		{
			public static readonly NullOwner Instance = new();
			
			public DockItemLayoutCollection Items { get; }
			
			public void OnItemAdded(DockItemLayout dockItemLayout)
			{
			}

			public void OnItemRemoved(DockItemLayout dockItemLayout)
			{
			}
		}

		internal IEnumerable<DockItemLayout> GetByDockState(DockItemState dockItemState)
		{
			return this.Where(d => d.DockState == dockItemState);
		}
	}
}