// <copyright file="MaskThickness.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.PresentationCore.Assets
{
	public sealed class MaskThickness : ThicknessAssetBase
	{
		public static readonly DependencyProperty LeftProperty = DPM.Register<bool, MaskThickness>
			("Left", true, b => b.OnLeftChanged);

		public static readonly DependencyProperty TopProperty = DPM.Register<bool, MaskThickness>
			("Top", true, b => b.OnTopChanged);

		public static readonly DependencyProperty RightProperty = DPM.Register<bool, MaskThickness>
			("Right", true, b => b.OnRightChanged);

		public static readonly DependencyProperty BottomProperty = DPM.Register<bool, MaskThickness>
			("Bottom", true, b => b.OnBottomChanged);

		public static readonly DependencyProperty EnabledThicknessProperty = DPM.Register<Thickness, MaskThickness>
			("EnabledThickness", b => b.UpdateActualThickness);

		public static readonly DependencyProperty DisabledThicknessProperty = DPM.Register<Thickness, MaskThickness>
			("DisabledThickness", b => b.UpdateActualThickness);

		public static readonly DependencyProperty InvertProperty = DPM.Register<bool, MaskThickness>
			("Invert", false, b => b.UpdateActualThickness);

		public static readonly DependencyProperty FlagsProperty = DPM.Register<MaskThicknessFlags, MaskThickness>
			("Flags", MaskThicknessFlags.All, b => b.OnFlagsChanged);

		private bool _suspend;


		public bool Bottom
		{
			get => (bool) GetValue(BottomProperty);
			set => SetValue(BottomProperty, value);
		}

		public Thickness DisabledThickness
		{
			get => (Thickness) GetValue(DisabledThicknessProperty);
			set => SetValue(DisabledThicknessProperty, value);
		}

		public Thickness EnabledThickness
		{
			get => (Thickness) GetValue(EnabledThicknessProperty);
			set => SetValue(EnabledThicknessProperty, value);
		}

		public MaskThicknessFlags Flags
		{
			get => (MaskThicknessFlags) GetValue(FlagsProperty);
			set => SetValue(FlagsProperty, value);
		}

		public bool Invert
		{
			get => (bool) GetValue(InvertProperty);
			set => SetValue(InvertProperty, value);
		}

		public bool Left
		{
			get => (bool) GetValue(LeftProperty);
			set => SetValue(LeftProperty, value);
		}

		public bool Right
		{
			get => (bool) GetValue(RightProperty);
			set => SetValue(RightProperty, value);
		}

		public bool Top
		{
			get => (bool) GetValue(TopProperty);
			set => SetValue(TopProperty, value);
		}

		private void DisableFlag(MaskThicknessFlags flag)
		{
			Flags &= ~flag;
		}

		private void EnableFlag(MaskThicknessFlags flag)
		{
			Flags |= flag;
		}

		private void OnBottomChanged(bool oldBottom, bool newBottom)
		{
			if (_suspend == false)
				SetFlag(newBottom, MaskThicknessFlags.Bottom);
		}

		private void OnFlagsChanged(MaskThicknessFlags oldFlags, MaskThicknessFlags newFlags)
		{
			if (_suspend)
				return;

			try
			{
				_suspend = true;

				Left = (newFlags & MaskThicknessFlags.Left) != 0;
				Top = (newFlags & MaskThicknessFlags.Top) != 0;
				Right = (newFlags & MaskThicknessFlags.Right) != 0;
				Bottom = (newFlags & MaskThicknessFlags.Bottom) != 0;
			}
			finally
			{
				_suspend = false;

				UpdateActualThickness();
			}
		}

		private void OnLeftChanged(bool oldLeft, bool newLeft)
		{
			if (_suspend == false)
				SetFlag(newLeft, MaskThicknessFlags.Left);
		}

		private void OnRightChanged(bool oldRight, bool newRight)
		{
			if (_suspend == false)
				SetFlag(newRight, MaskThicknessFlags.Right);
		}

		private void OnTopChanged(bool oldTop, bool newTop)
		{
			if (_suspend == false)
				SetFlag(newTop, MaskThicknessFlags.Top);
		}

		private void UpdateActualThickness()
		{
			var disabledThickness = DisabledThickness;
			var enabledThickness = EnabledThickness;
			var invert = Invert;
			var trueThickness = invert ? disabledThickness : enabledThickness;
			var falseThickness = invert ? enabledThickness : disabledThickness;

			if (Left == false)
				trueThickness.Left = falseThickness.Left;

			if (Top == false)
				trueThickness.Top = falseThickness.Top;

			if (Right == false)
				trueThickness.Right = falseThickness.Right;

			if (Bottom == false)
				trueThickness.Bottom = falseThickness.Bottom;

			if (ActualThickness != trueThickness)
				ActualThickness = trueThickness;
		}

		private void SetFlag(bool value, MaskThicknessFlags flag)
		{
			if (value)
				EnableFlag(flag);
			else
				DisableFlag(flag);
		}
	}

	[Flags]
	public enum MaskThicknessFlags
	{
		None = 0,
		Left = 1,
		Top = 2,
		Right = 4,
		Bottom = 8,
		All = Left | Top | Right | Bottom
	}
}