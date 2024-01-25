// <copyright file="PreviewPresenter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Data;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.UI.Controls.Docking
{
	internal partial class PreviewPresenter
	{
		public static readonly DependencyProperty PreviewProperty = DPM.Register<Preview, PreviewPresenter>
			("Preview", p => p.OnPreviewChanged);

		public PreviewPresenter()
		{
			PlatformCtor();

			PreviewElement.SetBinding(DockItemPreviewElement.GeometryProperty, new Binding("Preview.Geometry") {Source = this});
		}

		public Preview Preview
		{
			set => SetValue(PreviewProperty, value);
			get => (Preview) GetValue(PreviewProperty);
		}

		private DockItemPreviewElement PreviewElement { get; } = new();

		partial void HideImpl();

		public void HidePresenter()
		{
			HideImpl();
		}

		private void OnPreviewChanged(Preview oldPreview, Preview newPreview)
		{
		}

		partial void PlatformCtor();

		partial void ShowImpl();

		public void ShowPresenter()
		{
			ShowImpl();
		}
	}
}