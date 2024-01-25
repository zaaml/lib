// <copyright file="CombinatorialExtensions.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;

namespace Zaaml.Core.Extensions
{
	internal static class CombinatorialExtensions
	{
		public static void VisitCartesianCoordinates(this IReadOnlyList<int> cartesianAxes, Action<IReadOnlyList<int>> visitor)
		{
			if (cartesianAxes.Count == 0)
				return;

			var counterBuffer = new int[cartesianAxes.Count];
			var elementBuffer = new int[cartesianAxes.Count];
			var counterHead = 0;
			var elementHead = 0;
			var counter = -1;
			var index = cartesianAxes.Count - 1;

			while (true)
			{
				if (++counter < cartesianAxes[elementHead])
				{
					elementBuffer[counterHead++] = counter;

					if (index == 0)
					{
						visitor(elementBuffer);

						counterHead--;
					}
					else
					{
						counterBuffer[elementHead++] = counter;
						counter = -1;
						index--;
					}
				}
				else
				{
					if (++index == cartesianAxes.Count)
						break;

					counterHead--;
					counter = counterBuffer[--elementHead];
				}
			}
		}

		public static void VisitCartesianProduct<T>(this IReadOnlyList<IReadOnlyList<T>> input, Action<IReadOnlyList<T>> visitor)
		{
			var axes = new int[input.Count];
			var elements = new T[input.Count];

			for (var i = 0; i < input.Count; i++)
				axes[i] = input[i].Count;

			void ProductVisitor(IReadOnlyList<int> c)
			{
				for (var i = 0; i < c.Count; i++)
					elements[i] = input[i][c[i]];

				visitor(elements);
			}

			VisitCartesianCoordinates(axes, ProductVisitor);
		}

		public static void VisitPermutations<T>(this IReadOnlyList<T> input, Action<IReadOnlyList<T>> visitor)
		{
			if (input.Count == 0)
				return;

			var output = new T[input.Count];
			var sequence = new int[input.Count - 1];
			var factorials = new long[input.Count + 1];

			factorials[0] = 0L;
			factorials[1] = 1L;

			for (var i = 2L; i < factorials.Length; i++)
				factorials[i] = factorials[i - 1] * i;

			for (var i = 0L; i < factorials[input.Count]; i++)
			{
				var number = i;

				for (var j = 0; j < sequence.Length; j++)
				{
					var factorial = ((IReadOnlyList<long>)factorials)[sequence.Length - j];

					sequence[j] = (int)(number / factorial);
					number = (int)(number % factorial);
				}

				for (var j = 0; j < input.Count; j++)
					output[j] = input[j];

				for (var j = 0; j < input.Count - 1; j++)
				{
					ref var a = ref output[j];
					ref var b = ref output[j + sequence[j]];

					(a, b) = (b, a);
				}

				visitor(output);
			}
		}
	}
}