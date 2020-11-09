// <copyright file="ArtboardComponentControlBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.Artboard
{
	public abstract class ArtboardComponentControlBase<TPanel> : FixedTemplateControl<TPanel>, IArtboardComponentControl where TPanel : ArtboardPanel
	{
		private ArtboardControl _artboard;
		private double _designHeight;
		private double _designWidth;
		private double _offsetX;
		private double _offsetY;
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
					TemplateRoot.Artboard = value;
			}
		}

		public double DesignHeight
		{
			get => _designHeight;
			internal set
			{
				_designHeight = value;

				if (Panel != null)
					Panel.DesignHeight = value;
			}
		}

		public double DesignWidth
		{
			get => _designWidth;
			internal set
			{
				_designWidth = value;

				if (Panel != null)
					Panel.DesignWidth = value;
			}
		}

		public double OffsetX
		{
			get => _offsetX;
			internal set
			{
				_offsetX = value;

				if (Panel != null)
					Panel.OffsetX = value;
			}
		}

		public double OffsetY
		{
			get => _offsetY;
			internal set
			{
				_offsetY = value;

				if (Panel != null)
					Panel.OffsetY = value;
			}
		}

		internal TPanel Panel => TemplateRoot;

		public double Zoom
		{
			get => _zoom;
			internal set
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
			Panel.OffsetX = OffsetX;
			Panel.OffsetY = OffsetY;
			Panel.DesignWidth = DesignWidth;
			Panel.DesignHeight = DesignHeight;

			Panel.Artboard = Artboard;
		}

		protected override void UndoTemplateOverride()
		{
			TemplateRoot.Artboard = null;

			base.UndoTemplateOverride();
		}

		double IArtboardComponentControl.DesignHeight
		{
			get => DesignHeight;
			set => DesignHeight = value;
		}

		double IArtboardComponentControl.DesignWidth
		{
			get => DesignWidth;
			set => DesignWidth = value;
		}

		double IArtboardComponentControl.Zoom
		{
			get => Zoom;
			set => Zoom = value;
		}

		double IArtboardComponentControl.OffsetX
		{
			get => OffsetX;
			set => OffsetX = value;
		}

		double IArtboardComponentControl.OffsetY
		{
			get => OffsetY;
			set => OffsetY = value;
		}

		ArtboardControl IArtboardComponentControl.Artboard
		{
			get => Artboard;
			set => Artboard = value;
		}
	}
}