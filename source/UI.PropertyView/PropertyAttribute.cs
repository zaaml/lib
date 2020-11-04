// <copyright file="PropertyAttribute.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.UI.Controls.PropertyView
{
	public class PropertyAttribute : Attribute
	{
		public string Category { get; set; }

		public string Description { get; set; }

		public string DisplayName { get; set; }

		public Type EditorType { get; set; }

		public Type LocalizationType { get; set; }
	}
}