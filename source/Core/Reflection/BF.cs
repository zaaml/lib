// <copyright file="BF.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using SBF = System.Reflection.BindingFlags;

// ReSharper disable IdentifierTypo
// ReSharper disable UnusedType.Global
// ReSharper disable InconsistentNaming

namespace Zaaml.Core.Reflection
{
	internal static class BF
	{
		public const SBF PNP = SBF.Public | SBF.NonPublic;
		public const SBF IPNP = SBF.Instance | PNP;
		public const SBF SPNP = SBF.Static | PNP;
		public const SBF GF = SBF.GetField;
		public const SBF SF = SBF.SetField;
	}
}