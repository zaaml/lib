// <copyright file="ColorTextEditor.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows.Media;
using Zaaml.PresentationCore.Utils;
using Zaaml.UI.Controls.Editors.Text;

namespace Zaaml.UI.Controls.ColorEditor
{
	public class ColorTextEditor : TextEditorBase<Color>
	{
		protected override bool StaysEditingOnCommit => true;

		protected override string FormatValue(Color value)
		{
			return value.ToString();
		}

		protected override bool TryParse(string text, out Color value)
		{
			return ColorUtils.TryConvertFromString(text, out value);
		}
	}
}