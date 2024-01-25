// <copyright file="IItemGenerator.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;

namespace Zaaml.UI.Controls.Core
{
	internal interface IItemGenerator<T> where T : FrameworkElement
	{
		void AttachItem(T item, object source);

		T CreateItem(object source);

		void DetachItem(T item, object source);

		void DisposeItem(T item, object source);
	}
}