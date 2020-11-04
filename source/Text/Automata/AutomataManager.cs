// <copyright file="AutomataManager.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Reflection;

namespace Zaaml.Text
{
	internal sealed class AutomataManager
	{
		#region Static Fields and Constants

		private static int _instanceCount;
		private static readonly AutomataManager Instance = new AutomataManager();
		private static readonly Dictionary<Type, Automata> AutomataDictionary = new Dictionary<Type, Automata>();

		#endregion

		#region Ctors

		private AutomataManager()
		{
			if (_instanceCount++ > 0)
				throw new InvalidOperationException("AutomataManager must be a singleton.");
		}

		#endregion

		#region Methods

		public static Automata Get(Type automataType)
		{
			{
				if (AutomataDictionary.TryGetValue(automataType, out var automata))
					return automata;
			}

			lock (AutomataDictionary)
			{
				if (AutomataDictionary.TryGetValue(automataType, out var automata))
					return automata;

				automata = (Automata) Activator.CreateInstance(automataType, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new object[] {Instance}, null);

				AutomataDictionary.Add(automataType, automata);

				return automata;
			}
		}

		public static TAutomata Get<TAutomata>() where TAutomata : Automata
		{
			return (TAutomata) Get(typeof(TAutomata));
		}

		#endregion
	}
}