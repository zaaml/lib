// <copyright file="SharedItemHelper.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;

namespace Zaaml.PresentationCore
{
	internal static class SharedItemHelper
	{
		public static void Share<T>(FrameworkElement owner, T oldSharedItem, T newSharedItem) where T : ISharedItem
		{
			oldSharedItem?.Owners.Remove(owner);
			newSharedItem?.Owners.Add(owner);
		}
	}
}