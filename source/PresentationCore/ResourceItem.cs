// <copyright file="ResourceItem.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.PresentationCore
{
	public sealed class ResourceItem<T> : ResourceItemBase<T>
	{
	}

	public sealed class ResourceItem : ResourceItemBase<object>
	{
	}
}