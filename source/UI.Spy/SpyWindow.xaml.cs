// <copyright file="SpyWindow.xaml.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows.Input;

namespace Zaaml.UI.Controls.Spy
{
	public partial class SpyWindow
	{
		public SpyWindow()
		{
			InitializeComponent();
		}

		protected override void OnMouseDown(MouseButtonEventArgs e)
		{
			base.OnMouseDown(e);
		}
	}
}