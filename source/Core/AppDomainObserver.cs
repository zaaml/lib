// <copyright file="AppDomainObserver.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Zaaml.Core
{
	internal sealed class AppDomainObserver
	{
		private readonly Action<IEnumerable<Assembly>> _assemblyAction;
		private readonly AppDomain _domain;
		private readonly HashSet<Assembly> _processedAssemblies = new();
		private readonly HashSet<Assembly> _unprocessedAssemblies = new();

		public AppDomainObserver(Action<IEnumerable<Assembly>> assemblyAction) : this(AppDomain.CurrentDomain, assemblyAction)
		{
		}

		public AppDomainObserver(AppDomain domain, Action<IEnumerable<Assembly>> assemblyAction)
		{
			_domain = domain;
			_assemblyAction = assemblyAction;
			_domain.AssemblyLoad += DomainOnAssemblyLoad;
			_unprocessedAssemblies.AddRange(_domain.GetAssemblies());
		}

		private void DomainOnAssemblyLoad(object sender, AssemblyLoadEventArgs args)
		{
			_unprocessedAssemblies.Add(args.LoadedAssembly);
		}

		public void Update()
		{
			if (_unprocessedAssemblies.Count == 0)
				return;

			var newAssemblies = _unprocessedAssemblies.Except(_processedAssemblies).ToList();

			_processedAssemblies.UnionWith(_unprocessedAssemblies);
			_unprocessedAssemblies.Clear();
			_assemblyAction(newAssemblies);
		}
	}
}