// <copyright file="DockControlLayout.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Markup;
using System.Xml;
using System.Xml.Linq;
using Zaaml.Core.Trees;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.UI.Controls.Docking
{
	[ContentProperty(nameof(Items))]
	[DebuggerDisplay("{" + nameof(DebuggerDisplay) + "}")]
	public sealed partial class DockControlLayout : InheritanceContextObject, IDockItemLayoutCollectionOwner, ISupportInitialize
	{
		private static readonly DependencyPropertyKey DockControlPropertyKey = DPM.RegisterReadOnly<DockControl, DockControlLayout>
			("DockControlPrivate", d => d.OnDockControlPropertyChangedPrivate);

		public static readonly DependencyProperty DockControlProperty = DockControlPropertyKey.DependencyProperty;

		private static readonly ITreeEnumeratorAdvisor<DockItemLayout> TreeEnumeratorAdvisor = new DelegateTreeEnumeratorAdvisor<DockItemLayout>(GetChildrenEnumerator);

		private static readonly DependencyPropertyKey ItemsPropertyKey = DPM.RegisterReadOnly<DockItemLayoutCollection, DockControlLayout>
			("ItemsPrivate");

		public static readonly DependencyProperty ItemsProperty = ItemsPropertyKey.DependencyProperty;

		private readonly List<DockItemLayout> _attachedItems = new();

		internal event EventHandler LayoutChanged;

		public DockControlLayout()
		{
			AttachedItemsInternal = _attachedItems.AsReadOnly();
		}

		internal IReadOnlyList<DockItemLayout> AttachedItemsInternal { get; }

		private string DebuggerDisplay => ToString();

		public DockControl DockControl
		{
			get => (DockControl)GetValue(DockControlProperty);
			internal set => this.SetReadOnlyValue(DockControlPropertyKey, value);
		}

		public XElement AsXElement()
		{
			var options = new DockItemLayoutXElementOptions(true, false);
			var refOptions = new DockItemLayoutXElementOptions(false, false);
			var elementName = XName.Get(GetType().Name, XamlConstants.XamlZMNamespace);
			var layoutXml = new XElement(elementName, new XAttribute(XNamespace.Xmlns.GetName(XamlConstants.XamlZMPrefix), XamlConstants.XamlZMNamespace));
			var writeItems = new HashSet<DockItemLayout>(Items);

			void WriteElement(XElement xml, DockItemLayout itemLayout, DockItemGroupLayout parent = null)
			{
				var refOnly = parent != null && parent.DockState != itemLayout.DockState && string.IsNullOrEmpty(itemLayout.ItemName) == false;
				var itemXml = itemLayout.AsXElement(refOnly ? refOptions : options);
				
				if (itemLayout is DockItemGroupLayout groupLayout)
				{
					foreach (var childItem in groupLayout.Items) 
						WriteElement(itemXml, childItem, groupLayout);
				}

				if (refOnly == false) 
					writeItems.Remove(itemLayout);

				xml.Add(itemXml);
			}

			foreach (var itemLayout in Items) 
				WriteElement(layoutXml, itemLayout);

			foreach (var itemLayout in writeItems.ToList()) 
				WriteElement(layoutXml, itemLayout);

			return layoutXml;
		}

		internal void AttachItem(DockItemLayout dockItemLayout)
		{
			_attachedItems.Add(dockItemLayout);
		}

		private static string Beautify(string xml)
		{
			var doc = new XmlDocument();

			doc.LoadXml(xml);

			var settings = new XmlWriterSettings
			{
				Indent = true,
				IndentChars = "  ",
				NewLineChars = Environment.NewLine,
				NewLineHandling = NewLineHandling.Entitize,
				NewLineOnAttributes = false,
				WriteEndDocumentOnClose = true,
				Encoding = new UTF8Encoding(false)
			};

			using var ms = new MemoryStream();
			using var writer = XmlWriter.Create(ms, settings);

			doc.Save(writer);

			var xmlString = Encoding.UTF8.GetString(ms.ToArray());

			return xmlString;
		}

		private DockItemLayoutCollection CreateDockItemLayoutCollection()
		{
			return new DockItemLayoutCollection(this);
		}

		internal void DetachItem(DockItemLayout dockItemLayout)
		{
			_attachedItems.Remove(dockItemLayout);
		}

		public static DockControlLayout FromXElement(XElement element)
		{
			return (DockControlLayout)XamlReader.Parse(element.ToString());
		}

		private static IEnumerator<DockItemLayout> GetChildrenEnumerator(DockItemLayout w)
		{
			return (w.IsSimple() ? Enumerable.Empty<DockItemLayout>() : ((DockItemGroupLayout)w).Items).GetEnumerator();
		}

		private void OnDockControlPropertyChangedPrivate(DockControl oldValue, DockControl newValue)
		{
		}

		internal void OnLayoutChanged()
		{
			LayoutChanged?.Invoke(this, EventArgs.Empty);
		}

		public void Save(TextWriter textWriter)
		{
			textWriter.Write(Beautify(AsXElement().ToString(SaveOptions.OmitDuplicateNamespaces)));
		}

		public override string ToString()
		{
			return Beautify(AsXElement().ToString(SaveOptions.OmitDuplicateNamespaces));
		}

		public DockItemLayoutCollection Items => this.GetValueOrCreate(ItemsPropertyKey, CreateDockItemLayoutCollection);

		void IDockItemLayoutCollectionOwner.OnItemAdded(DockItemLayout dockItemLayout)
		{
			dockItemLayout.Layout = this;

			OnLayoutChanged();
		}

		void IDockItemLayoutCollectionOwner.OnItemRemoved(DockItemLayout dockItemLayout)
		{
			dockItemLayout.Layout = null;

			OnLayoutChanged();
		}

		public void BeginInit()
		{
		}

		public void EndInit()
		{
			Merge();
		}
	}
}