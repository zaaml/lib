// <copyright file="AutomataManager.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using Zaaml.Core.Reflection;

namespace Zaaml.Text
{
	internal sealed class AutomataManager
	{
		private static int _instanceCount;
		private static readonly AutomataManager Instance = new();
		private static readonly Dictionary<Type, Automata> AutomataDictionary = new();

		private AutomataManager()
		{
			if (_instanceCount++ > 0)
				throw new InvalidOperationException("AutomataManager must be a singleton.");
		}

		public static Automata Get(Type automataType)
		{
			lock (AutomataDictionary)
			{
				if (AutomataDictionary.TryGetValue(automataType, out var automata))
					return automata;

				automata = (Automata)Activator.CreateInstance(automataType, BF.IPNP, null, new object[] { Instance }, null);

				AutomataDictionary.Add(automataType, automata);

				return automata;
			}
		}

		public static TAutomata Get<TAutomata>() where TAutomata : Automata
		{
			return (TAutomata)Get(typeof(TAutomata));
		}
	}
}