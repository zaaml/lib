// <copyright file="SpyVisualTreeDataItem.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using Zaaml.Core;
using Zaaml.Core.Trees;
using Zaaml.PresentationCore.Extensions;

namespace Zaaml.UI.Controls.Spy
{
	public sealed class SpyVisualTreeDataItem : INotifyPropertyChanged
	{
		internal static readonly DelegateTreeEnumeratorAdvisor<SpyVisualTreeDataItem> EagerTreeAdvisor = new(d => d.EagerChildren.GetEnumerator());
		internal static readonly DelegateTreeEnumeratorAdvisor<SpyVisualTreeDataItem> LazyTreeAdvisor = new(d => d.LazyChildren.GetEnumerator());

		private readonly ObservableCollection<SpyVisualTreeDataItem> _children = new();
		private readonly SpyVisualTreeDataItemPool _pool;
		private bool _childrenDirty;
		private UIElement _element;

		internal SpyVisualTreeDataItem(SpyVisualTreeDataItemPool pool)
		{
			_pool = pool;
		}

		public ObservableCollection<SpyVisualTreeDataItem> Children
		{
			get
			{
				UpdateChildren();

				return _children;
			}
		}

		private ObservableCollection<SpyVisualTreeDataItem> EagerChildren => Children;

		public UIElement Element
		{
			get => _element;
			set
			{
				if (Equals(value, _element))
					return;

				_element = value;
				_childrenDirty = true;

				OnPropertyChanged();
				OnPropertyChanged(nameof(TypeName));
				OnPropertyChanged(nameof(Children));
			}
		}

		private ObservableCollection<SpyVisualTreeDataItem> LazyChildren => _children;

		public string Name => Element is FrameworkElement fre ? fre.Name : string.Empty;

		public string TypeName => Element?.GetType().Name;

		public bool IsControl => Element is Control;

		[NotifyPropertyChangedInvocator]
		private void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		public void Release()
		{
			ReleaseChildren();

			_element = null;
			_childrenDirty = true;

			_pool.ReleaseItem(this);
		}

		private void ReleaseChildren()
		{
			if (_children.Count == 0)
				return;

			foreach (var child in _children)
				child.Release();

			_children.Clear();
		}

		public bool TryFind(UIElement element, bool loadChildren, out SpyVisualTreeDataItem dataItem)
		{
			var advisor = loadChildren ? EagerTreeAdvisor : LazyTreeAdvisor;

			dataItem = TreeEnumerator.Find(this, advisor, t => ReferenceEquals(t.Element, element));

			return dataItem != null;
		}

		private void UpdateChildren()
		{
			if (_childrenDirty == false)
				return;

			try
			{
				ReleaseChildren();

				if (_element == null)
					return;

				foreach (var child in _element.GetVisualChildren().OfType<UIElement>())
					_children.Add(_pool.GetItem(child));
			}
			finally
			{
				_childrenDirty = false;
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;
	}
}