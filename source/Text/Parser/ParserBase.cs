// <copyright file="ParserBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.Text
{
	internal abstract class ParserBase
	{
		protected sealed class VisitorAttribute : Attribute
		{
		}
	}
}