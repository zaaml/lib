using System;

namespace Zaaml.UI.Controls.Docking
{
	[Flags]
	public enum DropGuideAction
	{
		None = 0,

		SplitLeft = 0x1,
		SplitTop = 0x2,
		SplitRight = 0x4,
		SplitBottom = 0x8,

		SplitAll = SplitLeft | SplitTop | SplitRight | SplitBottom,

		SplitDocumentLeft = 0x10,
		SplitDocumentTop = 0x20,
		SplitDocumentRight = 0x40,
		SplitDocumentBottom = 0x80,

		SplitDocumentHorizontal = SplitDocumentLeft | SplitDocumentRight,
		SplitDocumentVertical = SplitDocumentTop | SplitDocumentBottom,

		SplitDocumentAll = SplitDocumentLeft | SplitDocumentTop | SplitDocumentRight | SplitDocumentBottom,

		AutoHideLeft = 0x100,
		AutoHideTop = 0x200,
		AutoHideRight = 0x400,
		AutoHideBottom = 0x800,

		AutoHideAll = AutoHideLeft | AutoHideTop | AutoHideRight | AutoHideBottom,

		TabLeft = 0x1000,
		TabTop = 0x2000,
		TabRight = 0x4000,
		TabBottom = 0x8000,
		TabCenter = 0x10000,

		TabAll = TabLeft | TabTop | TabRight | TabBottom | TabCenter,

		DockLeft = 0x100000,
		DockTop = 0x200000,
		DockRight = 0x400000,
		DockBottom = 0x800000,

		DockAll = DockLeft | DockTop | DockRight | DockBottom,

		All = SplitAll | SplitDocumentAll | AutoHideAll | TabAll | DockAll
	}
}