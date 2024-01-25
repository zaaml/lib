// <copyright file="AssemblyUtils.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Threading;
using System.Windows;
using Zaaml.Core.Extensions;
using Zaaml.Core.Monads;

namespace Zaaml.PresentationCore.Utils
{
	public static class AssemblyUtils
	{
		public static IEnumerable<string> EnumerateEmbeddedResources(this Assembly assembly)
		{
			var culture = Thread.CurrentThread.CurrentUICulture;
			var manifestResourceNames = assembly.GetManifestResourceNames();

			foreach (IDictionaryEnumerator enumerator in manifestResourceNames
				         .Select(r => new ResourceManager(r.Replace(".resources", ""), assembly))
				         .WithAction(r => r.SafeDo(rm => rm.GetObject("dummyCall")))
				         .Select(rm => rm.SafeWith(r => r.GetResourceSet(culture, false, true)))
				         .SkipNull()
				         .Select(r => r.GetEnumerator()))
			{
				while (enumerator.MoveNext())
					yield return (string)enumerator.Key;
			}
		}

		internal static Uri GetResourcePathUri(this Assembly assembly, string resourcePath)
		{
			var assemblyName = new AssemblyName(assembly.FullName).Name;

			if (string.IsNullOrEmpty(resourcePath))
				return new Uri($"{assemblyName};component/".ToLowerInvariant(), UriKind.RelativeOrAbsolute);

			return new Uri($"{assemblyName};component/{resourcePath.Trim('/')}/".ToLowerInvariant(), UriKind.RelativeOrAbsolute);
		}

		public static Stream GetResourceStream(this Assembly assembly, string resourcePath)
		{
			return Application.GetResourceStream(assembly.GetResourceUri(resourcePath))?.Stream;
		}

		public static Uri GetResourceUri(this Assembly assembly, string resourcePath)
		{
			var assemblyName = new AssemblyName(assembly.FullName).Name;

			return new Uri($"{assemblyName};component/{resourcePath.TrimStart('/')}".ToLowerInvariant(), UriKind.RelativeOrAbsolute);
		}
	}
}