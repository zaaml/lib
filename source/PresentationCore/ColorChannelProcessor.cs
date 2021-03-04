using System;
using System.Windows;
using System.Windows.Media;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.PresentationCore
{
	public sealed class ColorChannelProcessor : ColorProcessor
	{
		public static readonly DependencyProperty ChannelProperty = DPM.Register<ColorChannel, ColorChannelProcessor>
			("Channel", ColorChannel.Alpha, d => d.OnChannelPropertyChangedPrivate);

		public static readonly DependencyProperty ValueProperty = DPM.Register<double, ColorChannelProcessor>
			("Value", 0.0, d => d.OnValuePropertyChangedPrivate);

		public static readonly DependencyProperty OperationProperty = DPM.Register<ColorChannelProcessorOperation, ColorChannelProcessor>
			("Operation", ColorChannelProcessorOperation.None, d => d.OnOperationPropertyChangedPrivate);

		public ColorChannel Channel
		{
			get => (ColorChannel) GetValue(ChannelProperty);
			set => SetValue(ChannelProperty, value);
		}

		public ColorChannelProcessorOperation Operation
		{
			get => (ColorChannelProcessorOperation) GetValue(OperationProperty);
			set => SetValue(OperationProperty, value);
		}

		public double Value
		{
			get => (double) GetValue(ValueProperty);
			set => SetValue(ValueProperty, value);
		}

		private void OnChannelPropertyChangedPrivate(ColorChannel oldValue, ColorChannel newValue)
		{
			OnProcessorChanged();
		}

		private void OnOperationPropertyChangedPrivate(ColorChannelProcessorOperation oldValue, ColorChannelProcessorOperation newValue)
		{
			OnProcessorChanged();
		}

		private void OnValuePropertyChangedPrivate(double oldValue, double newValue)
		{
			OnProcessorChanged();
		}

		protected override Color ProcessValueCore(Color color)
		{
			var channel = Channel;
			var value = Value;

			switch (Operation)
			{
				case ColorChannelProcessorOperation.None:
					return color;

				case ColorChannelProcessorOperation.Replace:
					return color.ModifyChannelValue(channel, value);

				case ColorChannelProcessorOperation.Add:
					return color.ModifyChannelValue(channel, color.GetChannelValue(channel) + value);

				case ColorChannelProcessorOperation.Subtract:
					return color.ModifyChannelValue(channel, color.GetChannelValue(channel) - value);

				case ColorChannelProcessorOperation.Multiply:
					return color.ModifyChannelValue(channel, color.GetChannelValue(channel) * value);

				case ColorChannelProcessorOperation.Divide:
					return color.ModifyChannelValue(channel, color.GetChannelValue(channel) / value);

				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}