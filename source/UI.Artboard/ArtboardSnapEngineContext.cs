// <copyright file="ArtboardSnapEngineContext.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Zaaml.UI.Controls.Artboard
{
	public class ArtboardSnapEngineContext : IDisposable
	{
		public ArtboardSnapEngineContext(ArtboardSnapEngine engine, ArtboardSnapEngineContextParameters parameters)
		{
			Engine = engine;
			Parameters = parameters;
			SourceSnapPrimitives = new ReadOnlyCollection<ArtboardSnapSourcePrimitive>(Engine.GetSourcesInternal(Parameters.Element).SelectMany(snapSource => snapSource.GetSnapPrimitives(Parameters)).ToList());
			TargetSnapPrimitives = new ReadOnlyCollection<ArtboardSnapTargetPrimitive>(Engine.GetTargetsInternal(Parameters.Element).SelectMany(snapTarget => snapTarget.GetSnapPrimitives(Parameters)).ToList());
			DynamicTargets = new ReadOnlyCollection<ArtboardSnapTargetPrimitive>(TargetSnapPrimitives.Where(p => p.IsFixed == false).ToList());
		}

		internal ReadOnlyCollection<ArtboardSnapTargetPrimitive> DynamicTargets { get; }

		public ArtboardSnapEngine Engine { get; }

		public ArtboardSnapEngineContextParameters Parameters { get; }

		public ReadOnlyCollection<ArtboardSnapSourcePrimitive> SourceSnapPrimitives { get; }

		public ReadOnlyCollection<ArtboardSnapTargetPrimitive> TargetSnapPrimitives { get; }

		public void Dispose()
		{
		}
	}
}