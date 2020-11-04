// <copyright file="QuantifierKind.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Text
{
	internal enum QuantifierKind
	{
		Generic,
		ZeroOrOne,
		ZeroOrMore,
		OneOrMore,
	}

	internal enum QuantifierMode
	{
		Greedy,
		Lazy
	}
}