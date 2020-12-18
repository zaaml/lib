// <copyright file="MaskThicknessConverter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Globalization;
using System.Windows;
using Zaaml.Core.Packed;

namespace Zaaml.PresentationCore.Converters
{
	public sealed class MaskThicknessConverter : BaseValueConverter
	{
		#region Fields

		private byte _packedValue;

		#endregion

		#region Ctors

		public MaskThicknessConverter()
		{
			_packedValue = 0x0;
		}

		#endregion

		#region Properties

		public bool Bottom
		{
			get => PackedDefinition.Bottom.GetValue(_packedValue);
			set => PackedDefinition.Bottom.SetValue(ref _packedValue, value);
		}

		public bool Left
		{
			get => PackedDefinition.Left.GetValue(_packedValue);
			set => PackedDefinition.Left.SetValue(ref _packedValue, value);
		}

		public bool Right
		{
			get => PackedDefinition.Right.GetValue(_packedValue);
			set => PackedDefinition.Right.SetValue(ref _packedValue, value);
		}

		public bool Top
		{
			get => PackedDefinition.Top.GetValue(_packedValue);
			set => PackedDefinition.Top.SetValue(ref _packedValue, value);
		}

		#endregion

		#region  Methods

		protected override object ConvertBackCore(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}

		protected override object ConvertCore(object value, Type targetType, object parameter, CultureInfo culture)
		{
			Thickness thickness;

			if (value is double doubleValue)
				thickness = new Thickness(doubleValue);
			else if (value is int intValue)
				thickness = new Thickness(intValue);
			else if (value is Thickness thicknessValue)
				thickness = thicknessValue;
			else
				return value;

			if (Left == false)
				thickness.Left = 0;

			if (Top == false)
				thickness.Top = 0;

			if (Right == false)
				thickness.Right = 0;

			if (Bottom == false)
				thickness.Bottom = 0;

			return thickness;
		}

		#endregion

		#region  Nested Types

		private static class PackedDefinition
		{
			#region Static Fields and Constants

			public static readonly PackedBoolItemDefinition Left;
			public static readonly PackedBoolItemDefinition Top;
			public static readonly PackedBoolItemDefinition Right;
			public static readonly PackedBoolItemDefinition Bottom;

			#endregion

			#region Ctors

			static PackedDefinition()
			{
				var allocator = new PackedValueAllocator();

				Left = allocator.AllocateBoolItem();
				Top = allocator.AllocateBoolItem();
				Right = allocator.AllocateBoolItem();
				Bottom = allocator.AllocateBoolItem();
			}

			#endregion
		}

		#endregion
	}
}