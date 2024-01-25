// <copyright file="SelectionScope.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Diagnostics;
using System.Windows;
using Zaaml.Core;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.PresentationCore.Behaviors.Selectable
{
	[DebuggerDisplay("{" + nameof(SelectedItem) + "}")]
	internal class SelectionScope : DependencyObject, ISelectionScope<DependencyObject>
	{
		public static readonly DependencyProperty SelectedItemProperty = DPM.Register<DependencyObject, SelectionScope>
			("SelectedItem", s => s.OnSelectedItemChanged);

		private readonly BoolSuspender _skipChangedSuspender = new();

		public event EventHandler<SelectedItemChangedEventArgs> SelectedItemChanged;

		protected virtual void OnSelectedItemChanged(DependencyObject oldValue, DependencyObject newValue)
		{
			if (_skipChangedSuspender.IsSuspended)
				return;

			if (oldValue != null)
				SelectableBehavior.SetIsSelectedInt(oldValue, false);

			if (newValue != null)
				SelectableBehavior.SetIsSelectedInt(newValue, true);

			SelectedItemChanged?.Invoke(this, new SelectedItemChangedEventArgs(oldValue, newValue));
		}

		internal void SetSelectedObjectInt(DependencyObject value)
		{
			using (BoolSuspender.Suspend(_skipChangedSuspender))
				SelectedItem = value;
		}

		public DependencyObject SelectedItem
		{
			get => (DependencyObject)GetValue(SelectedItemProperty);
			set => SetValue(SelectedItemProperty, value);
		}
	}

	[DebuggerDisplay("{" + nameof(SelectedItem) + "}")]
	internal sealed class SelectionScope<T> : DependencyObject, ISelectionScope<T> where T : DependencyObject
	{
		private readonly SelectionScope _selectionScope = new();

		public event EventHandler<SelectedItemChangedEventArgs<T>> SelectedItemChanged;

		public SelectionScope()
		{
			_selectionScope.SelectedItemChanged += SelectionScopeOnSelectedItemChanged;
		}

		private void SelectionScopeOnSelectedItemChanged(object sender, SelectedItemChangedEventArgs e)
		{
			SelectedItemChanged?.Invoke(this, new SelectedItemChangedEventArgs<T>((T)e.OldItem, (T)e.NewItem));
		}

		public T SelectedItem
		{
			get => (T)_selectionScope.SelectedItem;
			set => _selectionScope.SelectedItem = value;
		}
	}
}