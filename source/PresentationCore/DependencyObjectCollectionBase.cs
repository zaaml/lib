// <copyright file="DependencyObjectCollectionBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.Core;
using Zaaml.PresentationCore.ObservableCollections;

namespace Zaaml.PresentationCore
{
#if SILVERLIGHT
  public class DependencyObjectCollectionRaw<T> : DependencyObjectCollection<T> where T : DependencyObject
#else
	public class DependencyObjectCollectionRaw<T> : FreezableCollection<T> where T : DependencyObject
#endif
	{
		#region Ctors

		internal DependencyObjectCollectionRaw()
		{
		}

		#endregion
	}


	public class DependencyObjectCollectionBase<T> : DependencyObjectCollectionRaw<T> where T : DependencyObject
	{
		#region Fields

		[UsedImplicitly] private readonly ObservableCollectionDispatcher<T> _dispatcher;

		#endregion

		#region Ctors

		public DependencyObjectCollectionBase()
		{
			_dispatcher = this.Dispatch(OnItemAdded, OnItemRemoved);
		}

		#endregion

		#region  Methods

		protected virtual void OnItemAdded(T obj)
		{
		}

		protected virtual void OnItemRemoved(T obj)
		{
		}

		#endregion
	}
}