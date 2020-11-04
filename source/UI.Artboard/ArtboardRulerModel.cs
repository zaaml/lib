// <copyright file="ArtboardRulerModel.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.UI.Controls.Artboard
{
	public class ArtboardRulerModel : GridLineModel<TickMarkDefinitionCollection, TickMarkDefinition>
	{
		protected override TickMarkDefinitionCollection CreateGridLineCollection()
		{
			return new TickMarkDefinitionCollection(this);
		}
	}
}