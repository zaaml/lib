// <copyright file="MessageWindowOptions.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) zaaml. All rights reserved.
// </copyright>

using System.Windows.Media;

namespace Zaaml.UI.Windows
{
	public sealed class MessageWindowOptions
	{
		#region Ctors

		public MessageWindowOptions(string message, string caption, MessageWindowButtons buttons, MessageBoxImage image, MessageWindowResultKind defaultResult)
		{
			MessageText = message;
			Caption = caption;
			Buttons = buttons;
			Image = image;
			DefaultResult = defaultResult;
		}

		public MessageWindowOptions(string message, string caption, MessageWindowButtons buttons, ImageSource image, MessageWindowResultKind defaultResult)
		{
			MessageText = message;
			Caption = caption;
			Buttons = buttons;
			ImageSource = image;
			DefaultResult = defaultResult;
		}

		public MessageWindowOptions()
		{
		}

		#endregion

		#region Properties

		public MessageWindowButtons Buttons { get; set; }
		public string Caption { get; set; }
		public MessageWindowResultKind DefaultResult { get; set; }
		public MessageBoxImage Image { get; set; }
		public ImageSource ImageSource { get; set; }
		public string MessageText { get; set; }
		public SkipCheckBoxOptions SkipCheckBoxOptions { get; set; }

		#endregion
	}
}