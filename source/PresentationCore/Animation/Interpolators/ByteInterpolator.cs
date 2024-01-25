// <copyright file="ByteInterpolator.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.PresentationCore.Animation
{
	public sealed class ByteInterpolator : PrimitiveInterpolator<byte>
	{
		public static readonly ByteInterpolator Instance = new();

		private ByteInterpolator()
		{
		}

		protected internal override byte EvaluateCore(byte start, byte end, double progress)
		{
			return (byte)(start + (int)((end - start + 0.5) * progress));
		}
	}
}