// <copyright file="ResourceKey.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.PresentationCore.MarkupExtensions
{
	public class ResourceKey : MarkupExtensionBase
	{
		#region Properties

		public object Value { get; set; }

		#endregion

		#region Methods

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			return Value;
		}

		#endregion
	}
}