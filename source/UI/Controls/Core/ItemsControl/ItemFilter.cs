// <copyright file="ItemFilter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.UI.Controls.Core
{
	public abstract class ItemTextFilter : InheritanceContextObject
	{
		public static readonly DependencyProperty FilterTextProperty = DPM.Register<string, ItemTextFilter>
			("FilterText", string.Empty, i => i.OnFilterTextChangedPrivate);

		public static readonly DependencyProperty IsEnabledProperty = DPM.Register<bool, ItemTextFilter>
			("IsEnabled", true, i => i.OnIsEnabledChangedPrivate);

		public static readonly DependencyProperty DelayProperty = DPM.Register<TimeSpan, ItemTextFilter>
			("Delay", TimeSpan.Zero);

		private readonly DelayAction<string, string> _delayFilterTextChanged;
		private bool _forceNextUpdate;
		protected event EventHandler ChangedPrivate;

		protected ItemTextFilter()
		{
			_delayFilterTextChanged = new DelayAction<string, string>(OnFilterTextChanged);
		}

		public TimeSpan Delay
		{
			get => (TimeSpan) GetValue(DelayProperty);
			set => SetValue(DelayProperty, value);
		}

		public string FilterText
		{
			get => (string) GetValue(FilterTextProperty);
			set => SetValue(FilterTextProperty, value);
		}

		private protected string FilterTextCache { get; private set; }

		public bool IsEnabled
		{
			get => (bool) GetValue(IsEnabledProperty);
			set => SetValue(IsEnabledProperty, value);
		}

		private protected bool IsEnabledCache { get; private set; }

		protected virtual bool UseDelay => true;

		internal void ForceUpdate()
		{
			if (_delayFilterTextChanged.InvokeQueried)
				_delayFilterTextChanged.ForceDelayComplete();
			else
			{
				_forceNextUpdate = true;

				this.ReadLocalBindingExpression(FilterTextProperty)?.UpdateTarget();
			}
		}

		protected virtual void OnFilterTextChanged(string oldFilterText, string newFilterText)
		{
			RaiseChangedCore();
		}

		private void OnFilterTextChangedPrivate(string oldFilterText, string newFilterText)
		{
			try
			{
				FilterTextCache = FilterText;
				IsEnabledCache = string.IsNullOrEmpty(FilterTextCache) == false;

				if (UseDelay && _forceNextUpdate == false)
					_delayFilterTextChanged.Invoke(oldFilterText, newFilterText, Delay);
				else
					OnFilterTextChanged(oldFilterText, newFilterText);
			}
			finally
			{
				_forceNextUpdate = false;
			}
		}

		private void OnIsEnabledChangedPrivate()
		{
			IsEnabledCache = string.IsNullOrEmpty(FilterTextCache) == false;

			RaiseChangedCore();
		}

		protected virtual void RaiseChangedCore()
		{
			ChangedPrivate?.Invoke(this, EventArgs.Empty);
		}
	}

	public abstract class ItemTextFilter<TItem> : ItemTextFilter, IItemFilter
	{
		protected abstract bool Pass(TItem item);

		bool IItemFilter.Pass(object item)
		{
			return Pass((TItem) item);
		}

		bool IItemFilter.IsEnabled => IsEnabledCache && string.IsNullOrEmpty(FilterTextCache) == false;

		event EventHandler IItemFilter.Changed
		{
			add => ChangedPrivate += value;
			remove => ChangedPrivate -= value;
		}
	}
}