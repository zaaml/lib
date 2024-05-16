// <copyright file="Constants.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Specialized;

namespace Zaaml.PresentationCore
{
	internal static class Constants
	{
		public static readonly NotifyCollectionChangedEventArgs NotifyCollectionChangedReset = new(NotifyCollectionChangedAction.Reset);
	}
}