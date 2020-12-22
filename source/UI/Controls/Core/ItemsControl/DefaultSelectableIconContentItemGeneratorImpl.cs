// <copyright file="DefaultSelectableIconContentItemGeneratorImpl.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using System.Windows.Data;
using Zaaml.UI.Controls.Interfaces;

namespace Zaaml.UI.Controls.Core
{
	internal class DefaultSelectableIconContentItemGeneratorImpl<TItem>
		where TItem : FrameworkElement, ISelectableItem, new()
	{
		private string _itemSelectionMember;
		private string _itemValueMember;

		public DefaultSelectableIconContentItemGeneratorImpl(string itemValueMember, string itemSelectionMember, IDelegatedItemGenerator<TItem> delegatedItemGenerator)
		{
			_itemValueMember = itemValueMember;
			_itemSelectionMember = itemSelectionMember;

			CreateValueMemberBinding();
			CreateItemSelectionMemberBinding();

			DelegatedItemGenerator = delegatedItemGenerator;
		}

		public IDelegatedItemGenerator<TItem> DelegatedItemGenerator { get; }

		public ItemGenerator<TItem> Generator => DelegatedItemGenerator.Implementation;

		public string ItemSelectionMember
		{
			get => _itemSelectionMember;
			set
			{
				if (string.Equals(_itemSelectionMember, value, StringComparison.OrdinalIgnoreCase))
					return;

				Generator.OnGeneratorChangingInt();

				_itemSelectionMember = value;

				CreateItemSelectionMemberBinding();

				Generator.OnGeneratorChangedInt();
			}
		}

		private Binding ItemSelectionMemberBinding { get; set; }

		public string ItemValueMember
		{
			get => _itemValueMember;
			set
			{
				if (string.Equals(_itemValueMember, value, StringComparison.OrdinalIgnoreCase))
					return;

				Generator.OnGeneratorChangingInt();

				_itemValueMember = value;

				CreateValueMemberBinding();

				Generator.OnGeneratorChangedInt();
			}
		}

		private Binding ItemValueMemberBinding { get; set; }

		public void AttachItem(TItem item, object source)
		{
			// ValueMember
			ItemGenerator<TItem>.InstallBinding(item, item.ValueProperty, ItemValueMemberBinding);

			// SelectionMember
			ItemGenerator<TItem>.InstallBinding(item, item.SelectionProperty, ItemSelectionMemberBinding);
		}

		private void CreateItemSelectionMemberBinding()
		{
			ItemSelectionMemberBinding = _itemSelectionMember != null ? new Binding(_itemSelectionMember) : null;
		}

		private void CreateValueMemberBinding()
		{
			ItemValueMemberBinding = _itemValueMember != null ? new Binding(_itemValueMember) : null;
		}

		public void DetachItem(TItem item, object source)
		{
			// ValueMember
			ItemGenerator<TItem>.UninstallBinding(item, item.ValueProperty, ItemValueMemberBinding);

			// SelectionMember
			ItemGenerator<TItem>.UninstallBinding(item, item.SelectionProperty, ItemSelectionMemberBinding);
		}

		public void OnItemValueMemberChanged(string oldValueMember, string newValueMember)
		{
			ItemValueMember = newValueMember;
		}
		
		public void OnItemSelectionMemberChanged(string oldSelectionMember, string newSelectionMember)
		{
			ItemSelectionMember = newSelectionMember;
		}
	}
}