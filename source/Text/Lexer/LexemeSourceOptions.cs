// <copyright file="LexemeSourceOptions.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Text
{
	public readonly struct LexemeSourceOptions
	{
		public LexemeSourceOptions(bool skipTrivia)
		{
			SkipTrivia = skipTrivia;
		}

		public bool SkipTrivia { get; }
	}
}