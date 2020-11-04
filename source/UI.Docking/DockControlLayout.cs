// <copyright file="DockControlLayout.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Markup;
using System.Xml;
using System.Xml.Linq;
using Zaaml.Core.Extensions;
using Zaaml.Core.Trees;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.PropertyCore.Extensions;

#pragma warning disable 108

namespace Zaaml.UI.Controls.Docking
{
	internal interface IDockItemLayoutCollectionOwner
	{
		DockItemLayoutCollection Items { get; }

		void OnItemAdded(DockItemLayout dockItemLayout);

		void OnItemRemoved(DockItemLayout dockItemLayout);
	}

	[ContentProperty(nameof(Items))]
	[DebuggerDisplay("{" + nameof(DebuggerDisplay) + "}")]
	public class DockControlLayout : InheritanceContextObject, IDockItemLayoutCollectionOwner
	{
		private static readonly ITreeEnumeratorAdvisor<DockItemLayout> TreeEnumeratorAdvisor = new DelegateTreeEnumeratorAdvisor<DockItemLayout>(GetChildrenEnumerator);

		private static readonly DependencyPropertyKey ItemsPropertyKey = DPM.RegisterReadOnly<DockItemLayoutCollection, DockControlLayout>
			("ItemsPrivate");

		public static readonly DependencyProperty ItemsProperty = ItemsPropertyKey.DependencyProperty;

		internal event EventHandler LayoutChanged;

		private string DebuggerDisplay => ToString();

		private DockControlLayout AsSerializable()
		{
			return GetNormalized();
		}

		public XElement AsXElement()
		{
			var elementName = XName.Get(GetType().Name, XamlConstants.XamlZMNamespace);
			var layoutXml = new XElement(elementName, new XAttribute(XNamespace.Xmlns.GetName(XamlConstants.XamlZMPrefix), XamlConstants.XamlZMNamespace));
			var serializableLayout = AsSerializable();

			WriteToXElement(serializableLayout.Items, layoutXml);

			return layoutXml;
		}

		private static string Beautify(string xml)
		{
#if SILVERLIGHT
			return xml;
#else
			var doc = new XmlDocument();

			doc.LoadXml(xml);

			var settings = new XmlWriterSettings
			{
				Indent = true,
				IndentChars = "  ",
				NewLineChars = Environment.NewLine,
				NewLineHandling = NewLineHandling.Entitize,
				NewLineOnAttributes = true,
				WriteEndDocumentOnClose = true,
				Encoding = new UTF8Encoding(false)
			};

			using var ms = new MemoryStream();
			using var writer = XmlWriter.Create(ms, settings);

			doc.Save(writer);

			var xmlString = Encoding.UTF8.GetString(ms.ToArray());

			return xmlString;
#endif
		}

		private void BuildIndices()
		{
			BuildIndices(Items);
		}

		private static void BuildIndices(DockItemLayoutCollection itemLayoutCollection)
		{
			var index = 0;

			foreach (var item in itemLayoutCollection)
			{
				DockItemLayout.SetIndex(item, index++);

				if (item is DockItemGroupLayout groupItemLayout)
					BuildIndices(groupItemLayout.Items);
			}
		}

		internal void CopyFrom(DockControlLayout source)
		{
			Items.Clear();
			Items.AddRange(Items.Select(l => l.Clone()));
		}

		private DockItemLayoutCollection CreateDockItemLayoutCollection()
		{
			return new DockItemLayoutCollection(this);
		}

		public static DockControlLayout FromXElement(XElement element)
		{
			return (DockControlLayout) XamlReader.Parse(element.ToString());
		}

		private static IEnumerator<DockItemLayout> GetChildrenEnumerator(DockItemLayout w)
		{
			return (w.IsSimple() ? Enumerable.Empty<DockItemLayout>() : ((DockItemGroupLayout) w).Items).GetEnumerator();
		}

		internal DockControlLayout GetNormalized()
		{
			var layout = new DockControlLayout();
			var flatDictionary = new Dictionary<string, DockItemState>();

			TreeEnumeratorAdvisor.Visit(Items, (s, e) => layout.MergeLayout(s, e));

			TreeEnumeratorAdvisor.ReverseVisit(Items, w =>
			{
				if (w.ItemName != null && w.HasLocalValue(DockItemLayout.DockStateProperty))
					flatDictionary[w.ItemName] = w.DockState;
			});

			TreeEnumeratorAdvisor.ReverseVisit(layout.Items, w =>
			{
				if (w.ItemName != null && flatDictionary.TryGetValue(w.ItemName, out var state))
					w.DockState = state;
			});

			layout.BuildIndices();

			return layout;
		}

		private void MergeLayout(DockItemLayout source, AncestorsEnumerator<DockItemLayout> ancestors)
		{
			var mergeLayout = Items.GetItemLayout(source.ItemName);

			if (mergeLayout == null)
			{
				mergeLayout = source.Clone(DockItemLayoutCloneMode.BaseProperties);

				Items.Add(mergeLayout);
			}

			if (source.HasLocalValue(DockItemLayout.DockStateProperty))
				mergeLayout.DockState = source.DockState;
			else
			{
				var rootLayout = ancestors.Enumerate().FirstOrDefault(a => a.HasLocalValue(DockItemLayout.DockStateProperty));

				if (rootLayout != null)
					mergeLayout.DockState = rootLayout.DockState;
			}

			foreach (var property in source.FullLayoutProperties)
			{
				if (source.HasLocalValue(property) == false)
					continue;

				mergeLayout.SetValue(property, source.GetValue(property));
			}

			if (!(source is DockItemGroupLayout containerLayout))
				return;

			var mergeContainer = (DockItemGroupLayout) mergeLayout;

			foreach (var childItemLayout in containerLayout.Items)
			{
				if (mergeContainer.Items.Contains(childItemLayout.ItemName))
					continue;

				var childLink = childItemLayout.Clone(DockItemLayoutCloneMode.Instance);

				childLink.ItemName = childItemLayout.ItemName;
				mergeContainer.Items.Add(childLink);
			}
		}

		internal void OnLayoutChanged()
		{
			LayoutChanged?.Invoke(this, EventArgs.Empty);
		}

		internal void Sort()
		{
			Items.Sort();

			foreach (var dockItemGroupLayout in TreeEnumeratorAdvisor.GetEnumerator(Items).Enumerate().OfType<DockItemGroupLayout>().ToList())
				dockItemGroupLayout.Items.Sort();
		}

		public override string ToString()
		{
			return Beautify(AsXElement().ToString(SaveOptions.OmitDuplicateNamespaces));
		}

		private XElement WriteToXElement(DockItemLayout layout)
		{
			var layoutXml = layout.Xml;

			if (layout is DockItemGroupLayout groupLayout)
				WriteToXElement(groupLayout.Items, layoutXml);

			return layoutXml;
		}

		private void WriteToXElement(DockItemLayoutCollection dockItemLayoutCollection, XElement layoutXml)
		{
			foreach (var layout in dockItemLayoutCollection)
			{
				if (layout.IsSimple() == false || string.IsNullOrEmpty(layout.ItemName) == false)
					layoutXml.Add(WriteToXElement(layout));
			}
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
	}
}