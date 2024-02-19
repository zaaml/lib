// <copyright file="TestContainerControl.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.PresentationCore.Test
{
	public class TestContainerControl : Control, INotifyPropertyChanged
	{
		public static readonly DependencyProperty Input1Property = DPM.Register<int, TestContainerControl>
			("Input1", t => t.OnInput1Changed);

		public static readonly DependencyProperty Input2Property = DPM.Register<int, TestContainerControl>
			("Input2", t => t.OnInput2Changed);

		public static readonly DependencyProperty Input3Property = DPM.Register<int, TestContainerControl>
			("Input3", t => t.OnInput3Changed);

		public static readonly DependencyProperty Input4Property = DPM.Register<int, TestContainerControl>
			("Input4", t => t.OnInput4Changed);

		public static readonly DependencyProperty Output1Property = DPM.Register<int, TestContainerControl>
			("Output1", t => t.OnOutput1Changed);

		public static readonly DependencyProperty Output2Property = DPM.Register<int, TestContainerControl>
			("Output2", t => t.OnOutput2Changed);

		public static readonly DependencyProperty Output3Property = DPM.Register<int, TestContainerControl>
			("Output3", t => t.OnOutput3Changed);

		public static readonly DependencyProperty Output4Property = DPM.Register<int, TestContainerControl>
			("Output4", t => t.OnOutput4Changed);

		private TestChildControl _childControl1;
		private TestChildControl _childControl2;
		private TestChildControl _childControl3;
		private TestChildControl _childControl4;

		public event EventHandler Input1Changed;
		public event EventHandler Input2Changed;
		public event EventHandler Input3Changed;
		public event EventHandler Input4Changed;

		public event EventHandler Output1Changed;
		public event EventHandler Output2Changed;
		public event EventHandler Output3Changed;
		public event EventHandler Output4Changed;

		public TestContainerControl()
		{
			DefaultStyleKey = typeof(TestContainerControl);
		}

		public TestChildControl ChildControl1
		{
			get { return _childControl1; }
			set
			{
				if (_childControl1 != null)
					RemoveLogicalChild(_childControl1);

				_childControl1 = value;

				if (_childControl1 != null)
					AddLogicalChild(_childControl1);

				OnPropertyChanged(nameof(ChildControl1));
			}
		}

		public TestChildControl ChildControl2
		{
			get { return _childControl2; }
			set
			{
				if (_childControl2 != null)
					RemoveLogicalChild(_childControl2);

				_childControl2 = value;

				if (_childControl2 != null)
					AddLogicalChild(_childControl2);

				OnPropertyChanged(nameof(ChildControl2));
			}
		}

		public TestChildControl ChildControl3
		{
			get { return _childControl3; }
			set
			{
				if (_childControl3 != null)
					RemoveLogicalChild(_childControl3);

				_childControl3 = value;

				if (_childControl3 != null)
					AddLogicalChild(_childControl3);

				OnPropertyChanged(nameof(ChildControl3));
			}
		}

		public TestChildControl ChildControl4
		{
			get { return _childControl4; }
			set
			{
				if (_childControl4 != null)
					RemoveLogicalChild(_childControl4);

				_childControl4 = value;

				if (_childControl4 != null)
					AddLogicalChild(_childControl4);

				OnPropertyChanged(nameof(ChildControl4));
			}
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

		public Grid Root { get; private set; }

		public void FindChildControls()
		{
			_childControl1 = (TestChildControl)GetTemplateChild("ChildControl1");
			_childControl2 = (TestChildControl)GetTemplateChild("ChildControl2");
			_childControl3 = (TestChildControl)GetTemplateChild("ChildControl3");
			_childControl4 = (TestChildControl)GetTemplateChild("ChildControl4");

			Root = (Grid)GetTemplateChild("Root");
		}

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			FindChildControls();
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

		protected virtual void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		public event PropertyChangedEventHandler PropertyChanged;
	}
}