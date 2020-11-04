using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Xml.Linq;
using Zaaml.PresentationCore.Utils;

namespace Zaaml.PresentationCore.Theming
{
	internal class ResourceDictionaryLoader
	{



		private static XElement GetResourceDictionary(Uri uriBase)
		{
			var fullUri = uriBase.IsAbsoluteUri ? uriBase : new Uri(ResourceDictionaryUtils.PackUri, uriBase);

			var rel = ResourceDictionaryUtils.PackUri.MakeRelativeUri(fullUri);

			var streamResourceInfo = Application.GetResourceStream(rel);

			if (streamResourceInfo == null)
				return null;

			var xnode = XElement.Load(streamResourceInfo.Stream);

			TransformElements(xnode);
			RemoveIgnorableElements(xnode, new List<XNamespace>());
			ReplaceControlTemplates(xnode);

			var nodeNamespace = xnode.Name.Namespace;
			var mergedDictionariesNode = xnode.Element(XName.Get(xnode.Name.LocalName + "." + "MergedDictionaries", nodeNamespace.NamespaceName));
			if (mergedDictionariesNode != null)
			{
				foreach (var referencedDictionary in mergedDictionariesNode.Elements().ToList())
				{
					var source = referencedDictionary.Attribute("Source");
					if (source != null)
					{
						var sourceUri = new Uri(fullUri, source.Value);
						referencedDictionary.ReplaceWith(GetResourceDictionary(sourceUri));
					}
				}
			}

			return xnode;
		}

		private static void ReplaceControlTemplates(XElement xnode)
		{
			foreach (var xmControlTemplate in xnode.Descendants(XName.Get("ControlTemplate", XamlConstants.XamlZMNamespace)).ToList())
			{
				var windowsTemplateNode = new XElement(XName.Get("ControlTemplate", XamlConstants.XamlNamespace));

				xmControlTemplate.ReplaceWith(windowsTemplateNode);

				windowsTemplateNode.Add(xmControlTemplate.Attributes());

				xmControlTemplate.Element(XName.Get("ControlTemplate.Triggers", XamlConstants.XamlZMNamespace))?.Remove();

				var controlTemplateRoot = xmControlTemplate.Elements().FirstOrDefault();
				if (controlTemplateRoot != null)
				{
					if (xmControlTemplate.Element(XName.Get("ControlTemplate.Triggers", XamlConstants.XamlZMNamespace)) != null)
						controlTemplateRoot.Add(new XElement(XName.Get("Extension.Triggers", XamlConstants.XamlZMNamespace), xmControlTemplate.Element(XName.Get("ControlTemplate.Triggers", XamlConstants.XamlZMNamespace)).Elements()));
					windowsTemplateNode.Add(controlTemplateRoot);
				}
			}
		}

		private static readonly XName IgnorableName = XName.Get("Ignorable", XamlConstants.XamlMCNamespace);

		private static void TransformElements(XElement element)
		{
			const string transformNamespace = "http://schemas.zaaml.com/xaml/transform";
			foreach (var child in element.Descendants().ToList())
			{
				var name = child.Name;
				if (name.NamespaceName == transformNamespace)
				{
					var actualNamespaceName = "";
					switch (XamlConstants.Framework)
					{
						case FrameworkType.WPF:
							actualNamespaceName = child.Attribute(XName.Get("WpfNameSpace", transformNamespace)).Value;
							break;
						case FrameworkType.Silverlight:
							actualNamespaceName = child.Attribute(XName.Get("SilverlightNameSpace", transformNamespace)).Value;
							break;
						default:
							throw new ArgumentOutOfRangeException();
					}

					child.ReplaceWith(new XElement(XName.Get(name.LocalName, actualNamespaceName), child.Nodes(), child.Attributes()));
				}
			}
		}

		private static void RemoveIgnorableElements(XElement element, List<XNamespace> ignorableNamespaces)
		{
			var ignorableAttribute = element.Attribute(IgnorableName);
			List<XNamespace> nodeIgnorable = null;
			if (ignorableAttribute != null)
			{
				nodeIgnorable = ignorableAttribute.Value.Split(' ').Select(element.GetNamespaceOfPrefix).ToList();
				ignorableNamespaces.AddRange(nodeIgnorable);
			}

			foreach (var attribute in element.Attributes().Where(a => ignorableNamespaces.Contains(a.Name.Namespace)).ToList())
				attribute.Remove();

			foreach (var childElement in element.Elements().Where(e => ignorableNamespaces.Contains(e.Name.Namespace)).ToList())
				childElement.Remove();

			foreach (var childElement in element.Elements())
				RemoveIgnorableElements(childElement, ignorableNamespaces);

			if (ignorableAttribute == null) return;

			for (var i = 0; i < nodeIgnorable.Count; i++)
				ignorableNamespaces.RemoveAt(ignorableNamespaces.Count - 1);

			ignorableAttribute.Remove();
		}

		public static string GetResourceDictionaryFullText(Uri uriBase)
		{
			var rdNode = GetResourceDictionary(uriBase);
			var xKeyName = XName.Get("Key", XamlConstants.XamlXNamespace);

			foreach (var descendant in rdNode.Descendants(XName.Get("Style", XamlConstants.XamlNamespace)).Where(descendant => descendant.Attribute(xKeyName) == null))
				descendant.Add(new XAttribute(xKeyName, "implStyle" + Guid.NewGuid()));

			return rdNode.ToString();
		}
	}
}