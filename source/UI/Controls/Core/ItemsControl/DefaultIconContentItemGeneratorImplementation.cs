// <copyright file="DefaultIconContentItemGeneratorImpl.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Zaaml.UI.Controls.Interfaces;
using Zaaml.UI.Controls.Primitives.ContentPrimitives;

namespace Zaaml.UI.Controls.Core
{
	internal class DefaultIconContentItemGeneratorImplementation<TItem, TGenerator> : DefaultContentItemGeneratorImplementation<TItem, TGenerator>
		where TItem : FrameworkElement, IIconContentControl, new()
		where TGenerator : ItemGenerator<TItem>, IDelegatedGenerator<TItem>, new()
	{
		private string _itemIconMember;

		public DefaultIconContentItemGeneratorImplementation(string itemIconMember, string itemContentMember, DataTemplate itemContentTemplate, DataTemplateSelector itemContentTemplateSelector, string itemContentStringFormat)
			: base(itemContentMember, itemContentTemplate, itemContentTemplateSelector, itemContentStringFormat)
		{
			_itemIconMember = itemIconMember;

			CreateItemIconMemberBinding();
		}

		public string ItemIconMember
		{
			get => _itemIconMember;
			set
			{
				if (string.Equals(_itemIconMember, value, StringComparison.OrdinalIgnoreCase))
					return;

				Generator.OnGeneratorChangingInt();

				_itemIconMember = value;
				CreateItemIconMemberBinding();

				Generator.OnGeneratorChangedInt();
			}
		}

		private Binding ItemIconMemberBinding { get; set; }

		internal Binding ItemIconMemberBindingInternal => ItemIconMemberBinding;

		public override void AttachItem(TItem item, object source)
		{
			base.AttachItem(item, source);

			ItemGenerator<TItem>.InstallBinding(item, item.IconProperty, ItemIconMemberBinding);
		}

		private void CreateItemIconMemberBinding()
		{
			ItemIconMemberBinding = _itemIconMember != null ? new Binding(_itemIconMember) { Converter = IconConverter.Instance } : null;
		}

		public override void DetachItem(TItem item, object source)
		{
			ItemGenerator<TItem>.UninstallBinding(item, item.IconProperty, ItemIconMemberBinding);

			base.DetachItem(item, source);
		}
	}
}