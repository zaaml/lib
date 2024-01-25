// <copyright file="SkinContext.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.PresentationCore.Theming
{
	internal struct SkinContext
	{
		#region Fields

		private readonly SkinResourceManager _skinResourceManager;
		private readonly string _themeResourceKey;

		#endregion

		#region Ctors

		public static readonly SkinContext Empty = new(null, null);
		public static readonly object EmptyBoxed = Empty;

		public bool IsEmpty => _skinResourceManager == null;

		public SkinContext(SkinResourceManager skinResourceManager, string themeResourceKey)
		{
			_skinResourceManager = skinResourceManager;
			_themeResourceKey = themeResourceKey;
		}

		#endregion

		#region Methods

		public object GetValue(string skinKey)
		{
			if (IsEmpty)
				return null;

			var baseUri = new Uri($"skin://host/{_themeResourceKey.Replace('.', '/')}");
			var relativeUri = new Uri(skinKey.Replace("../", "$").Replace('.', '/').Replace("$", "../").Trim('/').ToLowerInvariant(), UriKind.Relative);
			
			if (Uri.TryCreate(baseUri, relativeUri, out var resultUri))
			{
				var actualKey = resultUri.AbsolutePath.Trim('/').Replace('/', '.');
				var themeResource = _skinResourceManager.GetResource(actualKey);
			
				return themeResource?.Value;
			}

			return null;
		}

		#endregion

		public bool Equals(SkinContext other)
		{
			return Equals(_skinResourceManager, other._skinResourceManager) && string.Equals(_themeResourceKey, other._themeResourceKey);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			
			return obj is SkinContext context && Equals(context);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return ((_skinResourceManager != null ? _skinResourceManager.GetHashCode() : 0) * 397) ^ (_themeResourceKey != null ? _themeResourceKey.GetHashCode() : 0);
			}
		}
	}
}