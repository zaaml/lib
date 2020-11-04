// <copyright file="ReadOnlyDependencyObjectCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;

namespace Zaaml.PresentationCore
{
	public class ReadOnlyDependencyObjectCollection<T> : ReadOnlyCollection<T>, INotifyCollectionChanged, INotifyPropertyChanged where T : DependencyObject
	{
		#region Fields

		protected virtual event NotifyCollectionChangedEventHandler CollectionChanged;

		protected virtual event PropertyChangedEventHandler PropertyChanged;

		#endregion

		#region Ctors

		public ReadOnlyDependencyObjectCollection(DependencyObjectCollectionBase<T> list)
			: base(list)
		{
			((INotifyCollectionChanged) Items).CollectionChanged += HandleCollectionChanged;
			((INotifyPropertyChanged) Items).PropertyChanged += HandlePropertyChanged;
		}

		#endregion

		#region  Methods

		private void HandleCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			OnCollectionChanged(e);
		}

		private void HandlePropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			OnPropertyChanged(e);
		}

		protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
		{
			CollectionChanged?.Invoke(this, args);
		}

		protected virtual void OnPropertyChanged(PropertyChangedEventArgs args)
		{
			PropertyChanged?.Invoke(this, args);
		}

		#endregion

		#region Interface Implementations

		#region INotifyCollectionChanged

		event NotifyCollectionChangedEventHandler INotifyCollectionChanged.CollectionChanged
		{
			add => CollectionChanged += value;

			remove => CollectionChanged -= value;
		}

		#endregion

		#region INotifyPropertyChanged

		event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
		{
			add => PropertyChanged += value;

			remove => PropertyChanged -= value;
		}

		#endregion

		#endregion
	}
}
