// <copyright file="BackstageViewItem.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Zaaml.Core.Utils;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Controls.Core;
using Zaaml.UI.Controls.Interfaces;

namespace Zaaml.UI.Controls.BackstageView
{
	public partial class BackstageViewItem : HeaderedIconContentControl, ISelectable, ISelectionStateControl
	{
		#region Static Fields and Constants

		public static readonly DependencyProperty IsSelectedProperty = DPM.Register<bool, BackstageViewItem>
			("IsSelected", b => b.OnIsSelectedChanged);

		private static readonly DependencyPropertyKey BackstageViewControlPropertyKey = DPM.RegisterReadOnly<BackstageViewControl, BackstageViewItem>
			("BackstageViewControl", t => t.OnBackstageViewControlChanged);

		public static readonly DependencyProperty BackstageViewControlProperty = BackstageViewControlPropertyKey.DependencyProperty;

		#endregion

		#region Ctors

		static BackstageViewItem()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<BackstageViewItem>();
		}

		public BackstageViewItem()
		{
			this.OverrideStyleKey<BackstageViewItem>();

			ContentHost.SetBinding(System.Windows.Controls.ContentPresenter.ContentTemplateProperty, new Binding { Path = new PropertyPath(ContentTemplateProperty), Source = this });

			AddLogicalChild(ContentHost);
		}

		#endregion

		#region Properties

		public BackstageViewControl BackstageViewControl
		{
			get => (BackstageViewControl) GetValue(BackstageViewControlProperty);
			internal set => this.SetReadOnlyValue(BackstageViewControlPropertyKey, value);
		}

		internal ContentPresenter ContentHost { get; } = new ContentPresenter();

		protected override IEnumerator LogicalChildren => EnumeratorUtils.Concat(ContentHost, base.LogicalChildren);

		#endregion

		#region  Methods

		private void Activate()
		{
			SetIsSelectedInt(true);
			BackstageViewControl?.Activate(this);
		}

		private void OnBackstageViewControlChanged(BackstageViewControl oldBackstageView, BackstageViewControl newBackstageView)
		{
		}

		protected override void OnContentChanged(object oldContent, object newContent)
		{
			base.OnContentChanged(oldContent, newContent);

			ContentHost.Content = newContent;
		}

		protected virtual void OnIsSelectedChanged()
		{
			var selected = IsSelected;

			if (selected)
				BackstageViewControl?.SelectItemInternal(this);

			UpdateVisualState(true);

			if (selected)
				RaiseSelectedEvent();
			else
				RaiseUnselectedEvent();

			if (selected == IsSelected)
				IsSelectedChanged?.Invoke(this, EventArgs.Empty);
		}

		protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
		{
			if (e.Handled)
				return;

			e.Handled = true;
			Activate();
		}

		private void SetIsSelectedInt(bool isSelected)
		{
			this.SetCurrentValueInternal(IsSelectedProperty, isSelected ? KnownBoxes.BoolTrue : KnownBoxes.BoolFalse);
		}

		#endregion

		#region Interface Implementations

		#region ISelectable

		public event EventHandler IsSelectedChanged;

		public bool IsSelected
		{
			get => (bool) GetValue(IsSelectedProperty);
			set => SetValue(IsSelectedProperty, value);
		}

		#endregion

		#endregion
	}
}
