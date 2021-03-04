// <copyright file="ColorProcessor.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows.Media;

namespace Zaaml.PresentationCore
{
	public abstract class ColorProcessor : ValueProcessor<Color>
	{
		public Color Process(Color color)
		{
			return ProcessValueCore(color);
		}
	}
}