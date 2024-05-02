// <copyright file="ListViewItem.Selection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Controls;
using Zaaml.Core.Runtime;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.UI.Controls.Interfaces;

namespace Zaaml.UI.Controls.ListView
{
	public partial class ListViewItem
	{
		public static readonly DependencyProperty IsSelectedProperty = DPM.Register<bool, ListViewItem>
			("IsSelected", i => i.OnIsSelectedPropertyChangedPrivate, i => i.OnCoerceSelection);

		public static readonly DependencyProperty IsSelectableProperty = DPM.Register<bool, ListViewItem>
			("IsSelectable", true, d => d.OnIsSelectablePropertyChangedPrivate);

		public event EventHandler IsSelectedChanged;

		internal bool ActualCanSelect => CanSelect && ListViewControl?.CanSelectItemInternal(this) != false;

		protected virtual bool CanSelect => IsSelectable;

		public bool IsSelectable
		{
			get => (bool) GetValue(IsSelectableProperty);
			set => SetValue(IsSelectableProperty, value.Box());
		}

		public bool IsSelected
		{
			get => (bool) GetValue(IsSelectedProperty);
			set => SetValue(IsSelectedProperty, value.Box());
		}

		private object OnCoerceSelection(object arg)
		{
			var isSelected = (bool) arg;

			if (isSelected && ActualCanSelect == false)
				return BooleanBoxes.False;

			return arg;
		}

		private void OnIsSelectablePropertyChangedPrivate(bool oldValue, bool newValue)
		{
			if (newValue == false && IsSelected)
				SetIsSelectedInternal(false);
		}

		protected virtual void OnIsSelectedChanged()
		{
			var selected = IsSelected;

			if (selected)
				RaiseSelectedEvent();
			else
				RaiseUnselectedEvent();

			if (selected == IsSelected)
				IsSelectedChanged?.Invoke(this, EventArgs.Empty);
		}

		private void OnIsSelectedPropertyChangedPrivate()
		{
			var selected = IsSelected;

			if (selected)
				ListViewControl?.Select(this);
			else
				ListViewControl?.Unselect(this);

			OnIsSelectedChanged();
			UpdateZIndex();
			UpdateVisualState(true);
		}

		internal void SelectInternal()
		{
			SetIsSelectedInternal(true);
		}

		internal void SetIsSelectedInternal(bool value)
		{
			if (IsSelected != value)
				this.SetCurrentValueInternal(IsSelectedProperty, value.Box());
		}

		internal void UnselectInternal()
		{
			SetIsSelectedInternal(false);
		}

		private void UpdateZIndex()
		{
			Panel.SetZIndex(this, IsMouseOver ? 30000 : IsSelected ? 20000 : 10000);
		}

		DependencyProperty ISelectableItem.SelectionProperty => IsSelectedProperty;
	}
}