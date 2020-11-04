// <copyright file="IItemGenerator.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;

namespace Zaaml.UI.Controls.Core
{
	internal interface IItemGenerator<T> where T : FrameworkElement
	{
		#region  Methods

		void AttachItem(T item, object itemSource);

		T CreateItem(object itemSource);

		void DetachItem(T item, object itemSource);

		void DisposeItem(T item, object itemSource);

		#endregion
	}
}