// <copyright file="ClassStyle.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.ComponentModel;
using Zaaml.PresentationCore.Theming;

namespace Zaaml.PresentationCore
{
	[TypeConverter(typeof(ClassStyleTypeConverter))]
	public sealed class ClassStyle
	{
		public ClassStyle(string classStyleString)
		{
		}
	}
}