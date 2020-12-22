// <copyright file="IndexedSelectorBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.UI.Panels.Core;
using NativeControl = System.Windows.Controls.Control;

namespace Zaaml.UI.Controls.Core
{
	public abstract class IndexedSelectorBase<TControl, TItem, TCollection, TPresenter, TPanel> : SelectorBase<TControl, TItem, TCollection, TPresenter, TPanel>, ISelector<TItem>
		where TItem : NativeControl
		where TCollection : ItemCollectionBase<TControl, TItem>
		where TPresenter : ScrollableItemsPresenterBase<TControl, TItem, TCollection, TPanel>
		where TPanel : ItemsPanel<TItem>
		where TControl : SelectorBase<TControl, TItem, TCollection, TPresenter, TPanel>
	{
		public static readonly DependencyProperty SelectedIndexProperty = DPM.Register<int, IndexedSelectorBase<TControl, TItem, TCollection, TPresenter, TPanel>>
			("SelectedIndex", -1, s => s.SelectorController.OnSelectedIndexPropertyChanged, s => s.SelectorController.CoerceSelectedIndex);

		public int SelectedIndex
		{
			get => (int) GetValue(SelectedIndexProperty);
			set => SetValue(SelectedIndexProperty, value);
		}

		internal override int SelectedIndexInternal => SelectedIndex;

		DependencyProperty ISelector<TItem>.SelectedIndexProperty => SelectedIndexProperty;
	}


	public abstract class IndexedSelectorBaseTemplateContract<TPresenter> : SelectorBaseTemplateContract<TPresenter>
		where TPresenter : ItemsPresenterBase
	{
	}
}