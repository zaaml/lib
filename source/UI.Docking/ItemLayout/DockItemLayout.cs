// <copyright file="DockItemLayout.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Xml.Linq;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.PropertyCore.Extensions;

#pragma warning disable 108

namespace Zaaml.UI.Controls.Docking
{
	[DebuggerTypeProxy(typeof(DockItemLayoutDebugView))]
	public class DockItemLayout : InheritanceContextObject, ILayoutPropertyChangeListener
	{
		public static readonly DependencyProperty ItemNameProperty = DPM.Register<string, DockItemLayout>
			("ItemName", d => d.OnItemNamePropertyChangedPrivate);

		public static readonly DependencyProperty DockStateProperty = DPM.Register<DockItemState, DockItemLayout>
			("DockState", DockItemState.Hidden, d => d.OnDockStatePropertyChangedPrivate);

		protected static readonly IReadOnlyList<DependencyProperty> BasePropertiesList = new[] { DockStateProperty }.ToList().AsReadOnly();

		private DockControlLayout _layout;

		public DockItemLayout(DockItem item)
		{
			if (item.HasLocalValue(FrameworkElement.NameProperty))
				ItemName = item.Name;

			DockState = item.DockState;
		}

		public DockItemLayout()
		{
		}

		internal DockItemLayout(DockItemLayout source, DockItemLayoutCloneMode mode)
		{
			CopyMembers(source, mode);
		}

		protected virtual IReadOnlyList<DependencyProperty> BaseProperties => BasePropertiesList;

		public DockItemState DockState
		{
			get => (DockItemState)GetValue(DockStateProperty);
			set => SetValue(DockStateProperty, value);
		}

		internal Dictionary<DockItemState, DockItemGroupLayout> ItemGroups { get; } = new();

		internal HashSet<DockItemLayoutCollection> ItemLayoutCollections { get; } = new();

		public string ItemName
		{
			get => (string)GetValue(ItemNameProperty);
			set => SetValue(ItemNameProperty, value);
		}

		internal string ItemNameInternal => ItemName ?? ItemNameUnique;

		private string ItemNameUnique { get; } = "_" + Guid.NewGuid().ToString().Replace("-", "_");

		internal DockControlLayout Layout
		{
			get => _layout;
			set
			{
				if (ReferenceEquals(_layout, value))
					return;

				if (_layout != null && value != null)
					throw new InvalidOperationException("DockItemLayout is already attached to other DockControlLayout");

				var oldLayout = _layout;

				_layout = value;

				OnLayoutPropertyChangedPrivate(oldLayout, _layout);
			}
		}

		internal XElement Xml => AsXElement(new DockItemLayoutXElementOptions());

		internal virtual XElement AsXElement(DockItemLayoutXElementOptions options)
		{
			var elementName = XName.Get(GetType().Name, XamlConstants.XamlZMNamespace);
			var layoutXml = new XElement(elementName, new XAttribute(XNamespace.Xmlns.GetName(XamlConstants.XamlZMPrefix), XamlConstants.XamlZMNamespace));

			WriteXElementProperty(layoutXml, ItemNameProperty);

			if (options.BaseProperties)
			{
				foreach (var property in BaseProperties) 
					WriteXElementProperty(layoutXml, property);
			}

			foreach (var layoutKind in FullLayout.EnumerateLayoutKinds())
				BaseLayout.GetLayoutSerializer(FullLayout.GetLayoutType(layoutKind)).WriteProperties(this, layoutXml);

			return layoutXml;
		}

		private void WriteXElementProperty(XElement layoutXml, DependencyProperty property)
		{
			if (this.HasLocalValue(property) == false)
				return;

			var value = GetValue(property);

			if (value == null)
				return;

			if (property == DockStateProperty)
			{
				var parent = GetParentGroup(DockState);

				if (parent != null && parent.DockState == DockState && ParentGroupCount == 1)
					return;
			}

			var propertyName = XName.Get($"{property.GetName()}");

			layoutXml.Add(new XAttribute(propertyName, value));
		}

		internal void AttachGroup(DockItemState state, DockItemGroupLayout dockGroup)
		{
			DetachGroup(state);

			ItemGroups[state] = dockGroup;
		}

		internal void ClearProperties()
		{
			ClearProperty(ItemNameProperty);

			foreach (var dependencyProperty in BaseProperties)
				ClearProperty(dependencyProperty);

			foreach (var dependencyProperty in FullLayout.LayoutProperties)
				ClearProperty(dependencyProperty);
		}

		internal void ClearProperty(DependencyProperty dependencyProperty)
		{
			ClearValue(dependencyProperty);
		}

		internal DockItemLayout Clone(DockItemLayoutCloneMode mode)
		{
			return CloneCore(mode);
		}

		internal virtual DockItemLayout CloneCore(DockItemLayoutCloneMode mode)
		{
			return new DockItemLayout(this, mode);
		}

		internal void CopyMembers(DockItemLayout source, DockItemLayoutCloneMode mode)
		{
			if ((mode & DockItemLayoutCloneMode.BaseProperties) != 0)
			{
				ItemName = source.ItemName;
				DockState = source.DockState;
			}

			if ((mode & DockItemLayoutCloneMode.LayoutProperties) != 0)
				LayoutSettings.CopySettings(source, this, FullLayout.LayoutProperties);
		}

		internal void DetachGroup(DockItemGroupLayout dockGroup)
		{
			foreach (var kv in ItemGroups.Where(kv => ReferenceEquals(kv.Value, dockGroup)))
			{
				ItemGroups.Remove(kv.Key);

				break;
			}
		}

		internal void DetachGroup(DockItemState state)
		{
			DetachGroup(GetParentGroup(state));
		}

		internal void GetLayout(DockItem item)
		{
			LayoutSettings.CopySettings(item, this, FullLayout.LayoutProperties);
		}

		internal DockItemGroupLayout GetParentGroup(DockItemState dockState)
		{
			ItemGroups.TryGetValue(dockState, out var dockGroup);

			return dockGroup;
		}

		internal int ParentGroupCount
		{
			get
			{
				var count = 0;

				foreach (var kv in ItemGroups)
				{
					if (kv.Value != null)
						count++;
				}

				return count;
			}
		}

		internal void MergeProperties(DockItemLayout source)
		{
			MergeProperty(source, ItemNameProperty);

			foreach (var dependencyProperty in BaseProperties)
				MergeProperty(source, dependencyProperty);

			foreach (var dependencyProperty in FullLayout.LayoutProperties)
				MergeProperty(source, dependencyProperty);
		}

		internal void MergeProperty(DockItemLayout source, DependencyProperty property)
		{
			if (source.HasLocalValue(property))
				SetValue(property, source.GetValue(property));
		}

		protected virtual void OnDockStateChanged(DockItemState oldState, DockItemState newState)
		{
		}

		private void OnDockStatePropertyChangedPrivate(DockItemState oldState, DockItemState newState)
		{
			OnDockStateChanged(oldState, newState);
			OnLayoutPropertyChanged();
		}

		private void OnItemNamePropertyChangedPrivate()
		{
			if (Layout != null)
				throw new InvalidOperationException("ItemName property cannot be changed after DockItemLayout has been attached to DockControlLayout");

			OnLayoutPropertyChanged();
		}

		protected virtual void OnLayoutChanged(DockControlLayout oldLayout, DockControlLayout newLayout)
		{
		}

		protected virtual void OnLayoutPropertyChanged()
		{
			Layout?.OnLayoutChanged();
		}

		private void OnLayoutPropertyChangedPrivate(DockControlLayout oldLayout, DockControlLayout newLayout)
		{
			oldLayout?.DetachItem(this);
			newLayout?.AttachItem(this);

			OnLayoutChanged(oldLayout, newLayout);
		}

		void ILayoutPropertyChangeListener.OnLayoutPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			Layout?.OnLayoutChanged();
		}

		internal sealed class DockItemLayoutDebugView
		{
			public DockItemLayoutDebugView(DockItemLayout dockItemLayout)
			{
				Xml = dockItemLayout.Xml;
			}

			public XElement Xml { get; }
		}
	}

	internal readonly struct DockItemLayoutXElementOptions
	{
		public DockItemLayoutXElementOptions(bool baseProperties, bool structure)
		{
			BaseProperties = baseProperties;
			Structure = structure;
		}

		public bool BaseProperties { get; }

		public bool Structure { get; }
	}
}