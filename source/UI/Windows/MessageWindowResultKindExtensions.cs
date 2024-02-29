// <copyright file="MessageWindowResultKindExtensions.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.UI.Windows
{
	internal static class MessageWindowResultKindExtensions
	{
		public static MessageWindowButton Create(this MessageWindowResultKind result)
		{
			return new MessageWindowButton { Result = result };
		}
	}
}