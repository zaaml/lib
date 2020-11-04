// <copyright file="DoubleClampAsset.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.Core.Extensions;

namespace Zaaml.PresentationCore.Assets
{
	public sealed class DoubleClampAsset : ClampAsset<double>
	{
		protected override double Clamp()
		{
			return Value.Clamp(Minimum, Maximum);
		}
	}
}