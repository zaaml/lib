// <copyright file="ScrollInfoChangedEventArgs.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.UI.Controls.ScrollView
{
	public sealed class ScrollInfoChangedEventArgs : EventArgs
	{
		public ScrollInfoChangedEventArgs(ScrollInfo oldInfo, ScrollInfo newInfo)
		{
			OldInfo = oldInfo;
			NewInfo = newInfo;
		}

		public bool ExtentChanged => OldInfo.Extent.Equals(NewInfo.Extent) == false;

		public ScrollInfo NewInfo { get; }

		public bool OffsetChanged => OldInfo.Offset.Equals(NewInfo.Offset) == false;

		public ScrollInfo OldInfo { get; }

		public bool ViewportChanged => OldInfo.Viewport.Equals(NewInfo.Viewport) == false;

		private bool Equals(ScrollInfoChangedEventArgs other)
		{
			return NewInfo.Equals(other.NewInfo) && OldInfo.Equals(other.OldInfo);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;

			return obj is ScrollInfoChangedEventArgs a && Equals(a);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (NewInfo.GetHashCode() * 397) ^ OldInfo.GetHashCode();
			}
		}
	}
}