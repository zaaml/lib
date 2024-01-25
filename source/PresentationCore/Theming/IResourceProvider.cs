// <copyright file="IResourceProvider.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.PresentationCore.Theming
{
	internal interface ISkinResourceProvider
	{
		bool TryGetValue(string key, out object value);
	}
}