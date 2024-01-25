// <copyright file="ResourceSkinBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Windows;

namespace Zaaml.PresentationCore.Theming
{
	public abstract class ResourceSkinBase : DependencyObject, IResourceValue
	{
		private string _actualKey;
		private SkinResourceManager _manager;
		internal event EventHandler ResourceChanged;

		internal string ActualKey
		{
			get => _actualKey;
			set
			{
				if (string.Equals(_actualKey, value, StringComparison.OrdinalIgnoreCase))
					return;

				_actualKey = value;
			}
		}

		protected abstract object FrozenValue { get; }

		internal object FrozenValueInternal => FrozenValue;

		internal SkinResourceManager Manager
		{
			get => _manager;
			set
			{
				if (ReferenceEquals(_manager, value))
					return;

				_manager = value;
			}
		}

		protected abstract IEnumerable<DependencyProperty> Properties { get; }

		protected abstract object Value { get; }

		protected virtual void OnResourceChanged()
		{
			ResourceChanged?.Invoke(this, EventArgs.Empty);
		}

		string IResourceValue.Key
		{
			get => ActualKey;
			set => ActualKey = value;
		}
	}
}