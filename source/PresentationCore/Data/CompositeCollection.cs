// <copyright file="CompositeCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Markup;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.PresentationCore.Data
{
	[ContentProperty(nameof(Containers))]
	public sealed class CompositeCollection : InheritanceContextObject, IReadOnlyList<object>, INotifyCollectionChanged, INotifyPropertyChanged
	{
		private static readonly DependencyPropertyKey ContainersPropertyKey = DPM.RegisterReadOnly<CompositeCollectionContainerCollection, CompositeCollection>
			("ContainersInternal");

		public static readonly DependencyProperty ContainersProperty = ContainersPropertyKey.DependencyProperty;

		private readonly List<object> _innerList;

		public CompositeCollection()
		{
			_innerList = new List<object>();
		}

		public CompositeCollectionContainerCollection Containers
		{
			get => this.GetValueOrCreate(ContainersPropertyKey, () => new CompositeCollectionContainerCollection(this));
		}

		private void BuildInnerCollection()
		{
			_innerList.Clear();

			foreach (var source in Containers)
			{
				var count = source.Count;

				for (var i = 0; i < count; i++)
					_innerList.Add(source[i]);
			}

			CollectionChanged?.Invoke(this, Constants.NotifyCollectionChangedReset);
		}

		private void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		internal void OnSourceChangedInternal(CompositeCollectionContainer container)
		{
			BuildInnerCollection();
			OnPropertyChanged(nameof(Count));
		}

		public event NotifyCollectionChangedEventHandler CollectionChanged;

		public event PropertyChangedEventHandler PropertyChanged;

		public IEnumerator<object> GetEnumerator()
		{
			return _innerList.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable)_innerList).GetEnumerator();
		}

		public int Count => _innerList.Count;

		public object this[int index] => _innerList[index];
	}
}