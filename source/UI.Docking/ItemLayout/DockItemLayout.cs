// <copyright file="DockItemLayout.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Xml.Linq;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.PropertyCore;

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

		internal static readonly DependencyProperty IndexProperty = DPM.RegisterAttached<int, DockItemGroupLayout>
			("Index");

		private static readonly DependencyProperty[] Properties =
		{
			ItemNameProperty,
			DockStateProperty
		};

		public DockItemLayout(DockItem item)
		{
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

		public DockItemState DockState
		{
			get => (DockItemState) GetValue(DockStateProperty);
			set => SetValue(DockStateProperty, value);
		}

		internal virtual IEnumerable<DependencyProperty> FullLayoutProperties => FullLayout.LayoutProperties;

		internal HashSet<DockItemLayoutCollection> ItemLayoutCollections { get; } = new HashSet<DockItemLayoutCollection>();

		public string ItemName
		{
			get => (string) GetValue(ItemNameProperty);
			set => SetValue(ItemNameProperty, value);
		}

		internal virtual DockControlLayout Layout { get; set; }

		internal bool SkipSerializeId { get; set; }

		internal XElement Xml
		{
			get
			{
				var elementName = XName.Get(GetType().Name, XamlConstants.XamlZMNamespace);
				var layoutXml = new XElement(elementName, new XAttribute(XNamespace.Xmlns.GetName(XamlConstants.XamlZMPrefix), XamlConstants.XamlZMNamespace));

				foreach (var property in GetProperties())
				{
					if (ReferenceEquals(DependencyProperty.UnsetValue, ReadLocalValue(property)))
						continue;

					var value = GetValue(property);

					if (value == null)
						continue;

					if (property == ItemNameProperty && SkipSerializeId)
						continue;

					var propertyName = XName.Get($"{property.GetName()}");

					layoutXml.Add(new XAttribute(propertyName, value));
				}

				foreach (var layoutKind in FullLayout.EnumerateLayoutKinds())
					BaseLayout.GetLayoutSerializer(FullLayout.GetLayoutType(layoutKind)).WriteProperties(this, layoutXml);

				return layoutXml;
			}
		}

		internal DockItemLayout Clone()
		{
			return CloneCore(DockItemLayoutCloneMode.Full);
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

		internal static int GetIndex(DependencyObject element)
		{
			return (int) element.GetValue(IndexProperty);
		}

		internal void GetLayout(DockItem item)
		{
			LayoutSettings.CopySettings(item, this, FullLayout.LayoutProperties);
		}

		internal virtual IEnumerable<DependencyProperty> GetProperties()
		{
			return Properties;
		}

		private void OnDockStatePropertyChangedPrivate()
		{
			OnLayoutPropertyChanged();
		}

		private void OnItemNamePropertyChangedPrivate()
		{
			OnLayoutPropertyChanged();
		}

		protected virtual void OnLayoutPropertyChanged()
		{
			Layout?.OnLayoutChanged();
		}

		internal static void SetIndex(DependencyObject element, int value)
		{
			element.SetValue(IndexProperty, value);
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
}