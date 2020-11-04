// <copyright file="AppDomainObserver.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Zaaml.Core.Extensions;

namespace Zaaml.Core
{
	internal sealed class AppDomainObserver
	{
		#region Fields

		private readonly Action<IEnumerable<Assembly>> _assemblyAction;
		private readonly AppDomain _domain;
		private readonly HashSet<Assembly> _processedAssemblies = new HashSet<Assembly>();
		private int _prevCount = -1;

		#endregion

		#region Ctors

		public AppDomainObserver(Action<IEnumerable<Assembly>> assemblyAction) : this(AppDomain.CurrentDomain, assemblyAction)
		{
		}

		public AppDomainObserver(AppDomain domain, Action<IEnumerable<Assembly>> assemblyAction)
		{
			_domain = domain;
			_assemblyAction = assemblyAction;
		}

		#endregion

		#region Methods

		public void Update()
		{
			var assemblies = _domain.GetAssemblies();

			if (assemblies.Length == _prevCount)
				return;

			var newAssemblies = assemblies.Except(_processedAssemblies).ToList();

			_processedAssemblies.AddRange(newAssemblies);
			_assemblyAction(newAssemblies);
			_prevCount = assemblies.Length;
		}

		#endregion
	}
}