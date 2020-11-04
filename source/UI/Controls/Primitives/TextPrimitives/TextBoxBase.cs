// <copyright file="TextBoxBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows.Controls;

namespace Zaaml.UI.Controls.Primitives.TextPrimitives
{
	public partial class TextBoxBase : TextBox
	{
		#region Ctors

		public TextBoxBase()
		{
			PlatformCtor();
		}

		#endregion

		#region  Methods

		partial void PlatformCtor();

		#endregion
	}
}