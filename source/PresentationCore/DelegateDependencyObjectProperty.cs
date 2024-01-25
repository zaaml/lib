// <copyright file="DelegateDependencyObjectProperty.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using System.Windows.Data;
using Zaaml.PresentationCore.Data;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.Theming;

namespace Zaaml.PresentationCore
{
	internal sealed class DelegateDependencyObjectProperty<T> : DependencyObject
	{
		public static readonly DependencyProperty ValueProperty = DPM.Register<T, DelegateDependencyObjectProperty<T>>
			("Value", default, d => d.OnPropertyPropertyChangedPrivate);

		public static readonly PropertyPath PropertyPath = new(ValueProperty);

		private readonly Action<T, T> _change;

		public DelegateDependencyObjectProperty()
		{
			_change = (_, _) => { };
		}

		public DelegateDependencyObjectProperty(Action<T, T> change)
		{
			_change = change;
		}

		public T Value
		{
			get => (T)GetValue(ValueProperty);
			set => SetValue(ValueProperty, value);
		}

		public Binding CreateBinding()
		{
			return new Binding
			{
				Source = this,
				Path = DelegateDependencyObjectProperty<SkinDictionary>.PropertyPath
			};
		}

		public OwnedBinding<TOwner> CreateOwnedBinding<TOwner>(TOwner owner)
		{
			return new OwnedBinding<TOwner>(owner)
			{
				Source = this,
				Path = DelegateDependencyObjectProperty<SkinDictionary>.PropertyPath
			};
		}

		private void OnPropertyPropertyChangedPrivate(T oldValue, T newValue)
		{
			_change(oldValue, newValue);
		}
	}
}