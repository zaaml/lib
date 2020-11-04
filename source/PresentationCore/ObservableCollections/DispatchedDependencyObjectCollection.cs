// <copyright file="DispatchedDependencyObjectCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;

namespace Zaaml.PresentationCore.ObservableCollections
{
	public class DispatchedDependencyObjectCollection<T> : DependencyObjectCollectionBase<T> where T : DependencyObject
	{
	}
}