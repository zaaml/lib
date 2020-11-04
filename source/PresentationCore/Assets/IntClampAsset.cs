// <copyright file="IntClampAsset.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.Core.Extensions;

namespace Zaaml.PresentationCore.Assets
{
	public sealed class IntClampAsset : ClampAsset<int>
	{
		protected override int Clamp()
		{
			return Value.Clamp(Minimum, Maximum);
		}
	}
}