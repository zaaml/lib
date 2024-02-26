// <copyright file="Setter.Context.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Windows;
using Zaaml.PresentationCore.Theming;

namespace Zaaml.PresentationCore.Interactivity
{
	public sealed partial class Setter
	{
		internal class Context : IDisposable
		{
			private static readonly List<Context> Pool = [];
			private IInteractivityRoot _interactivityRoot;
			private bool _interactivityRootDirty;
			private DependencyProperty _property;
			private bool _propertyDirty;
			private Setter _setter;
			private DependencyObject _target;
			private bool _targetDirty;
			private object _value;
			private bool _valueDirty;
			private ThemeResourceKey _valueKey;
			private bool _valueKeyDirty;
			private ValuePathSource _valuePathSource;
			private bool _valuePathSourceDirty;

			public IInteractivityRoot InteractivityRoot
			{
				get
				{
					if (_interactivityRootDirty == false)
						return _interactivityRoot;

					_interactivityRootDirty = false;
					_interactivityRoot = _setter.Root;

					return _interactivityRoot;
				}
			}

			public DependencyProperty Property
			{
				get
				{
					if (_propertyDirty == false)
						return _property;

					_propertyDirty = false;
					_property = _setter.ActualProperty;

					return _property;
				}
			}

			public object RuntimeValue => _setter.GetRuntimeValue(this);

			public ISetterValueProvider RuntimeValueProvider => _setter.GetRuntimeValueProvider(this);

			private Setter Setter
			{
				set
				{
					_setter = value;

					_valueDirty = true;
					_valuePathSourceDirty = true;
					_valueKeyDirty = true;
					_targetDirty = true;
					_propertyDirty = true;
					_interactivityRootDirty = true;
					_property = null;
					_target = null;
					_value = null;
					_valueKey = ThemeResourceKey.Empty;
					_interactivityRoot = null;
				}
			}

			public DependencyObject Target
			{
				get
				{
					if (_targetDirty == false)
						return _target;

					_targetDirty = false;
					_target = _setter.ActualTarget;

					return _target;
				}
			}

			public object Value
			{
				get
				{
					if (_valueDirty == false)
						return _value;

					_valueDirty = false;
					_value = _setter.ActualValue;

					return _value;
				}
			}

			public ThemeResourceKey ValueKey
			{
				get
				{
					if (_valueKeyDirty == false)
						return _valueKey;

					_valueKeyDirty = false;
					_valueKey = _setter.ActualThemeResourceKey;

					return _valueKey;
				}
			}

			public string ValuePath
			{
				get
				{
					var valueKey = ValueKey;

					return valueKey.IsEmpty ? null : valueKey.Key;
				}
			}

			public ValuePathSource ValuePathSource
			{
				get
				{
					if (_valuePathSourceDirty == false)
						return _valuePathSource;

					_valuePathSourceDirty = false;
					_valuePathSource = _setter.ActualValuePathSource;

					return _valuePathSource;
				}
			}

			public static Context Get(Setter setter)
			{
				if (Pool.Count == 0)
					return new Context { Setter = setter };

				var index = Pool.Count - 1;
				var valueInfo = Pool[index];

				valueInfo.Setter = setter;
				Pool.RemoveAt(index);

				return valueInfo;
			}

			public void Dispose()
			{
				Setter = null;

				Pool.Add(this);
			}
		}
	}
}