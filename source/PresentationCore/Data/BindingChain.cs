// <copyright file="BindingChain.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.PresentationCore.Data
{
	public class BindingChainElement : ValueAsset
	{
		private Binding _binding;
		private CommonBindingProperties _commonBindingProperties;
		private object _source;

		public object ActualValue => GetValue(ValueProperty);

		public Binding Binding
		{
			get => _binding;
			set
			{
				_binding = value ?? throw new InvalidOperationException("Binding can not be null");

				if (_binding.RelativeSource != null)
					throw new Exception();

				if (_binding.Source != null)
					throw new Exception();

				if (_binding.Source != null)
					throw new Exception();

				_commonBindingProperties = new CommonBindingProperties();
				_commonBindingProperties.CopyFromBinding(_binding);
			}
		}

		internal object Source
		{
			get => _source;
			set
			{
				if (ReferenceEquals(_source, value))
					return;

				_source = value;

				if (_binding == null)
					throw new InvalidOperationException("Binding can not be null");

				if (_source != null)
				{
					var binding = new Binding
					{
						Path = _binding.Path,
						Source = _source
					};

					_commonBindingProperties.InitBinding(binding);

					this.SetBinding(ValueProperty, binding);
				}
				else
					ClearValue(ValueProperty);
			}
		}
	}

	[ContentProperty("ChainElements")]
	public class BindingChain : AssetBase
	{
		public static readonly DependencyProperty SourceProperty = DPM.Register<object, BindingChain>
			("Source", c => c.OnSourceChanged);

		private static readonly DependencyProperty ValueProperty = DPM.Register<object, BindingChain>
			("Value", c => c.OnValueChanged);

		private readonly DependencyObjectCollectionBase<BindingChainElement> _bindingChainElements = new();

		public event PropertyChangedEventHandler PropertyChanged;

		public object ActualValue => GetValue(ValueProperty);

		public DependencyObjectCollectionBase<BindingChainElement> ChainElements => _bindingChainElements;

		public object Source
		{
			get => GetValue(SourceProperty);
			set => SetValue(SourceProperty, value);
		}

		protected override void EndInitCore()
		{
			base.EndInitCore();

			if (Source != null)
				InitChain();
		}

		private void InitChain()
		{
			var currentSource = Source;

			foreach (var element in _bindingChainElements)
			{
				element.Source = currentSource;
				currentSource = element;
			}

			var lastChainElement = _bindingChainElements.LastOrDefault();

			if (lastChainElement != null)
				this.SetBinding(ValueProperty, new Binding { Path = new PropertyPath(ValueAsset.ValueProperty), Source = lastChainElement });
		}

		private void OnSourceChanged()
		{
			if (Initializing)
				InitChain();
		}

		private void OnValueChanged()
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ActualValue"));
		}
	}
}