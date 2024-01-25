// <copyright file="ArtboardComponentControlBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.Artboard
{
	public abstract class ArtboardComponentControlBase<TPanel> : FixedTemplateControl<TPanel>, IArtboardComponentControl where TPanel : ArtboardPanel
	{
		private ArtboardControl _artboard;
		private double _scrollOffsetX;
		private double _scrollOffsetY;
		private double _zoom;

		public ArtboardControl Artboard
		{
			get => _artboard;
			internal set
			{
				if (ReferenceEquals(_artboard, value))
					return;

				_artboard = value;

				if (TemplateRoot != null)
					TemplateRoot.ArtboardControl = value;
			}
		}

		internal TPanel Panel => TemplateRoot;

		private double ScrollOffsetX
		{
			get => _scrollOffsetX;
			set
			{
				_scrollOffsetX = value;

				if (Panel != null)
					Panel.ScrollOffsetX = value;
			}
		}

		private double ScrollOffsetY
		{
			get => _scrollOffsetY;
			set
			{
				_scrollOffsetY = value;

				if (Panel != null)
					Panel.ScrollOffsetY = value;
			}
		}

		private double Zoom
		{
			get => _zoom;
			set
			{
				_zoom = value;

				if (Panel != null)
					Panel.Zoom = value;
			}
		}

		protected override void ApplyTemplateOverride()
		{
			base.ApplyTemplateOverride();

			Panel.Zoom = Zoom;
			Panel.ScrollOffsetX = ScrollOffsetX;
			Panel.ScrollOffsetY = ScrollOffsetY;

			Panel.ArtboardControl = Artboard;
		}

		protected override void UndoTemplateOverride()
		{
			TemplateRoot.ArtboardControl = null;

			base.UndoTemplateOverride();
		}

		double IArtboardComponentControl.Zoom
		{
			get => Zoom;
			set => Zoom = value;
		}

		double IArtboardComponentControl.ScrollOffsetX
		{
			get => ScrollOffsetX;
			set => ScrollOffsetX = value;
		}

		double IArtboardComponentControl.ScrollOffsetY
		{
			get => ScrollOffsetY;
			set => ScrollOffsetY = value;
		}

		ArtboardControl IArtboardComponentControl.Artboard
		{
			get => Artboard;
			set => Artboard = value;
		}
	}
}