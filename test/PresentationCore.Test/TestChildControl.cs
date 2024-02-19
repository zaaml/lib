// <copyright file="TestChildControl.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using System.Windows.Controls;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.PresentationCore.Test
{
	public class TestChildControl : Control
	{
		public static readonly DependencyProperty Input1Property = DPM.Register<int, TestChildControl>
			("Input1", t => t.OnInput1Changed);

		public static readonly DependencyProperty Input2Property = DPM.Register<int, TestChildControl>
			("Input2", t => t.OnInput2Changed);

		public static readonly DependencyProperty Input3Property = DPM.Register<int, TestChildControl>
			("Input3", t => t.OnInput3Changed);

		public static readonly DependencyProperty Input4Property = DPM.Register<int, TestChildControl>
			("Input4", t => t.OnInput4Changed);

		public static readonly DependencyProperty Output1Property = DPM.Register<int, TestChildControl>
			("Output1", t => t.OnOutput1Changed);

		public static readonly DependencyProperty Output2Property = DPM.Register<int, TestChildControl>
			("Output2", t => t.OnOutput2Changed);

		public static readonly DependencyProperty Output3Property = DPM.Register<int, TestChildControl>
			("Output3", t => t.OnOutput3Changed);

		public static readonly DependencyProperty Output4Property = DPM.Register<int, TestChildControl>
			("Output4", t => t.OnOutput4Changed);

		public event EventHandler Input1Changed;
		public event EventHandler Input2Changed;
		public event EventHandler Input3Changed;
		public event EventHandler Input4Changed;

		public event EventHandler Output1Changed;
		public event EventHandler Output2Changed;
		public event EventHandler Output3Changed;
		public event EventHandler Output4Changed;

		public TestChildControl()
		{
			DefaultStyleKey = typeof(TestChildControl);
		}

		public int Input1
		{
			get { return (int)GetValue(Input1Property); }
			set { SetValue(Input1Property, value); }
		}

		public int Input2
		{
			get { return (int)GetValue(Input2Property); }
			set { SetValue(Input2Property, value); }
		}

		public int Input3
		{
			get { return (int)GetValue(Input3Property); }
			set { SetValue(Input3Property, value); }
		}

		public int Input4
		{
			get { return (int)GetValue(Input4Property); }
			set { SetValue(Input4Property, value); }
		}

		public int Output1
		{
			get { return (int)GetValue(Output1Property); }
			set { SetValue(Output1Property, value); }
		}

		public int Output2
		{
			get { return (int)GetValue(Output2Property); }
			set { SetValue(Output2Property, value); }
		}

		public int Output3
		{
			get { return (int)GetValue(Output3Property); }
			set { SetValue(Output3Property, value); }
		}


		public int Output4
		{
			get { return (int)GetValue(Output4Property); }
			set { SetValue(Output4Property, value); }
		}

		private void OnInput1Changed()
		{
			Input1Changed?.Invoke(this, EventArgs.Empty);
		}

		private void OnInput2Changed()
		{
			Input2Changed?.Invoke(this, EventArgs.Empty);
		}

		private void OnInput3Changed()
		{
			Input3Changed?.Invoke(this, EventArgs.Empty);
		}

		private void OnInput4Changed()
		{
			Input4Changed?.Invoke(this, EventArgs.Empty);
		}

		private void OnOutput1Changed()
		{
			Output1Changed?.Invoke(this, EventArgs.Empty);
		}

		private void OnOutput2Changed()
		{
			Output2Changed?.Invoke(this, EventArgs.Empty);
		}

		private void OnOutput3Changed()
		{
			Output3Changed?.Invoke(this, EventArgs.Empty);
		}

		private void OnOutput4Changed()
		{
			Output4Changed?.Invoke(this, EventArgs.Empty);
		}
	}
}