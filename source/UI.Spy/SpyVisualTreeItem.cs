// <copyright file="SpyVisualTreeItem.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using Zaaml.Core;
using Zaaml.PresentationCore.Extensions;

namespace Zaaml.UI.Controls.Spy
{
	internal sealed class SpyVisualTreeItem : INotifyPropertyChanged
	{
		private readonly ObservableCollection<SpyVisualTreeItem> _children = new();
		private readonly SpyVisualTreeItemPool _pool;
		private bool _childrenDirty;
		private UIElement _element;

		public SpyVisualTreeItem(SpyVisualTreeItemPool pool)
		{
			_pool = pool;
		}

		public ObservableCollection<SpyVisualTreeItem> Children
		{
			get
			{
				UpdateChildren();

				return _children;
			}
		}

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
				OnPropertyChanged(nameof(Type));
				OnPropertyChanged(nameof(Children));
			}
		}

		public string Name => Element is FrameworkElement fre ? fre.Name : string.Empty;

		public string Type => Element?.GetType().Name;

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

		public bool TryFind(UIElement element, bool loadChildren, out SpyVisualTreeItem item)
		{
			item = null;

			if (element == null)
				return false;

			if (ReferenceEquals(_element, element))
			{
				item = this;

				return true;
			}

			if (loadChildren)
				UpdateChildren();

			foreach (var childItem in _children)
			{
				if (childItem.TryFind(element, loadChildren, out item))
					return true;
			}

			return false;
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