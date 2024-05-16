// <copyright file="EnumSourceBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.PresentationCore.Data
{
	public abstract class EnumSourceBase
	{
		protected EnumSourceBase(string display)
		{
			Display = display;
		}

		public string Display { get; }
	}
}