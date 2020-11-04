// <copyright file="ArtboardControlBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.Artboard
{
	public abstract class ArtboardControlBase<TPanel> : FixedTemplateControl<TPanel> where TPanel : ArtboardPanel
	{
		private double _designHeight;
		private double _designWidth;
		private double _offsetX;
		private double _offsetY;
		private double _zoom;

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
		}
	}
}