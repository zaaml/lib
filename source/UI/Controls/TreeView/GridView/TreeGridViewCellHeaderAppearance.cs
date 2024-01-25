// <copyright file="TreeGridViewCellHeaderAppearance.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.UI.Controls.Core.GridView;

namespace Zaaml.UI.Controls.TreeView
{
	public abstract class TreeGridViewCellHeaderAppearance : InheritanceContextObject
	{
		private static readonly DependencyPropertyKey ActualGridLinesPropertyKey = DPM.RegisterReadOnly<GridViewLines, TreeGridViewCellHeaderAppearance>
			("ActualGridLines", GridViewLines.Both);

		public static readonly DependencyProperty ActualGridLinesProperty = ActualGridLinesPropertyKey.DependencyProperty;

		public static readonly DependencyProperty ShowVerticalLinesProperty = DPM.Register<bool, TreeGridViewCellHeaderAppearance>
			("ShowVerticalLines", true, d => d.OnShowVerticalLinesPropertyChangedPrivate);

		public static readonly DependencyProperty ShowHorizontalLinesProperty = DPM.Register<bool, TreeGridViewCellHeaderAppearance>
			("ShowHorizontalLines", true, d => d.OnShowHorizontalLinesPropertyChangedPrivate);

		public GridViewLines ActualGridLines
		{
			get => (GridViewLines)GetValue(ActualGridLinesProperty);
			private set => this.SetReadOnlyValue(ActualGridLinesPropertyKey, value);
		}

		public bool ShowHorizontalLines
		{
			get => (bool)GetValue(ShowHorizontalLinesProperty);
			set => SetValue(ShowHorizontalLinesProperty, value);
		}

		public bool ShowVerticalLines
		{
			get => (bool)GetValue(ShowVerticalLinesProperty);
			set => SetValue(ShowVerticalLinesProperty, value);
		}

		protected virtual void OnAppearanceChanged()
		{
		}

		protected virtual void OnShowHorizontalLinesPropertyChanged(bool oldValue, bool newValue)
		{
		}

		private void OnShowHorizontalLinesPropertyChangedPrivate(bool oldValue, bool newValue)
		{
			UpdateActualLines();
			OnShowHorizontalLinesPropertyChanged(oldValue, newValue);
			OnAppearanceChanged();
		}

		protected virtual void OnShowVerticalLinesPropertyChanged(bool oldValue, bool newValue)
		{
		}

		private void OnShowVerticalLinesPropertyChangedPrivate(bool oldValue, bool newValue)
		{
			UpdateActualLines();
			OnShowVerticalLinesPropertyChanged(oldValue, newValue);
			OnAppearanceChanged();
		}

		private void UpdateActualLines()
		{
			var lines = GridViewLines.None;

			if (ShowHorizontalLines)
				lines |= GridViewLines.Horizontal;

			if (ShowVerticalLines)
				lines |= GridViewLines.Vertical;

			ActualGridLines = lines;
		}
	}
}