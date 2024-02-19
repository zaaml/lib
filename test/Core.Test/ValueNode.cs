// <copyright file="ValueNode.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Core.Test
{
	internal class ValueNode
	{
		#region Ctors

		public ValueNode(int value)
		{
			Value = value;
		}

		#endregion

		#region Properties

		public int Value { get; }

		#endregion

		#region Methods

		public override string ToString()
		{
			return Value == -1 ? "Dead" : Value.ToString();
		}

		#endregion

		protected bool Equals(ValueNode other)
		{
			return Value == other.Value;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != GetType()) return false;
			return Equals((ValueNode)obj);
		}

		public override int GetHashCode()
		{
			return Value;
		}
	}
}