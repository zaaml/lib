// <copyright file="ArtboardSnapSource.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using Zaaml.PresentationCore;

namespace Zaaml.UI.Controls.Artboard
{
	public abstract class ArtboardSnapSource : InheritanceContextObject
	{
		public abstract IEnumerable<ArtboardSnapSourcePrimitive> GetSnapPrimitives(ArtboardSnapEngineContextParameters parameters);
	}
}